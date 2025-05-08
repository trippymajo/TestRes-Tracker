using TrtParserService.FileExtensions;
using TrtShared.DTO;
using Newtonsoft.Json;
using TrtShared.ServiceCommunication;

namespace TrtParserService
{
    public class Parser : BackgroundService
    {
        private readonly IFileParserFactory _parserFactory;
        private readonly ILogger<Parser> _logger;
        private readonly IParseTransport _resultTransport;

        public Parser(IFileParserFactory parserFactory, ILogger<Parser> logger, IParseTransport transport)
        {
            _logger = logger;
            _parserFactory = parserFactory;
            _resultTransport = transport;
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
            await foreach (var fullFilePath in _resultTransport.PathReader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    var dto = await ParseProc(fullFilePath);

                    if (dto == null)
                        _logger.LogError("DTO was not formed after parsing");

                    // Publish result dto to UploadService via redis
                    var dtoJson = JsonConvert.SerializeObject(dto);
                    await _resultTransport.PublishParsedDtoAsync(dtoJson);
                    _logger.LogInformation("Published to Redis: {dtoJson}", dtoJson);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing file: {path}", fullFilePath);
                }
            }
                await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
