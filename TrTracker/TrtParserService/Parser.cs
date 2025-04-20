using StackExchange.Redis;
using TrtParserService.FileExtensions;

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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var subscriber = _redis.GetSubscriber();

                try
                {
                    var channel = RedisChannel.Literal("file-events");
                    await subscriber.SubscribeAsync(channel, async (channel, message) =>
                    {
                        // Get info from Redis
                        var fullFilePath = message.ToString();
                        if (fullFilePath == null)
                        {
                            _logger.LogError("Redis haven't delivered right fullFilePath");
                            return;
                        }
                        else
                            _logger.LogInformation("fullFilePath from redis: {Path}", fullFilePath);

                        // Parse file

                        var parser = _parserFactory.Create(fullFilePath);
                        if (parser == null)
                        {
                            _logger.LogError("File Factory eploded, parser was not provided!");
                            return;
                        }


                        var testRunResultDto = await parser.Parse(fullFilePath, "TODO:BRANCH", "TODO:VERSION");
                        if (testRunResultDto == null)
                        {
                            _logger.LogError("No DTO been created due parse error");
                            return;
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed executing parsing. File was not parsed!");
                    throw;
                }


                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
