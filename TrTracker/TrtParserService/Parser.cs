using StackExchange.Redis;
using Newtonsoft.Json.Serialization;
using TrtParserService.FileExtensions;
using TrtShared.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TrtParserService
{
    public class Parser : BackgroundService
    {
        private readonly IFileParserFactory _parserFactory;
        private readonly ILogger<Parser> _logger;
        private readonly IConnectionMultiplexer _redis;
        

        public Parser(IFileParserFactory parserFactory, ILogger<Parser> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _parserFactory = parserFactory;
            _redis = redis;
        }

        /// <summary>
        /// Parsing workflow with filefactory and parsing itself
        /// </summary>
        /// <param name="fullFilePath">Full file path of parsed item</param>
        /// <returns>TestRunDTO for transfering data to DB</returns>
        private async Task<TestRunDTO?> ParseProc(string fullFilePath)
        {
            var parser = _parserFactory.Create(fullFilePath);
            if (parser == null)
            {
                _logger.LogError("File Factory eploded, parser was not provided!");
                return null;
            }

            var testRunResultDto = await parser.Parse(fullFilePath, "TODO:BRANCH", "TODO:VERSION");
            if (testRunResultDto == null)
            {
                _logger.LogError("No DTO been created due parse error");
                return null;
            }

            return testRunResultDto;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var pubsub = _redis.GetSubscriber();

                try
                {
                    var channelSub = RedisChannel.Literal("file-uploaded");
                    var channelPub = RedisChannel.Literal("file-parsed");

                    await pubsub.SubscribeAsync(channelSub, async (channelSub, message) =>
                    {
                        try
                        {
                            // Get info from Redis
                            var fullFilePath = message.ToString();
                            if (string.IsNullOrWhiteSpace(fullFilePath))
                            {
                                _logger.LogError("Redis haven't delivered right fullFilePath");
                                return;
                            }
                            else
                                _logger.LogInformation("fullFilePath from redis: {Path}", fullFilePath);

                            // Parse file
                            var dto = await ParseProc(fullFilePath);

                            if (dto == null)
                                _logger.LogError("DTO was not formed after parsing");

                            // Publish result dto to UploadService via redis
                            var dtoJson = JsonConvert.SerializeObject(dto);
                            await pubsub.PublishAsync(channelPub, dtoJson);
                            _logger.LogInformation("Published to Redis: {dtoJson}", dtoJson);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error while processing uploaded file");
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to start ParserService's Redis subscription");
                    throw;
                }


                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }
    }
}
