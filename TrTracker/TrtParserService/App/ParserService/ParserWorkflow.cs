using Newtonsoft.Json;

using TrtShared.Envelope;
using TrtShared.ServiceCommunication;

using TrtParserService.ParserCore;
using TrtParserService.App.FileReader;

namespace TrtParserService
{
    public class Parser : BackgroundService
    {
        private readonly IFileParserFactory _parserFactory;
        private readonly IFileReader _fileReader;
        private readonly IParseTransport _resultTransport;
        private readonly ILogger<Parser> _logger;


        public Parser(IFileParserFactory parserFactory, IFileReader fileReader, IParseTransport transport, ILogger<Parser> logger)
        {
            _fileReader = fileReader;
            _parserFactory = parserFactory;
            _resultTransport = transport;
            _logger = logger;
        }

        /// <summary>
        /// Parsing workflow with filefactory and parsing itself
        /// </summary>
        /// <param name="fullFilePath">Full file path of parsed item</param>
        /// <returns>UniEnvelope for transfering data to DB</returns>
        // WIP
        private async Task<UniEnvelope?> ParseProcAsync(string fullFilePath)
        {
            var parser = _parserFactory.Create(fullFilePath);
            if (parser == null)
            {
                _logger.LogError("File Factory eploded, parser was not provided!");
                return null;
            }

            using var streamFile = await _fileReader.OpenFileStreamAsync(fullFilePath);

            var testRunResultDto = await parser.ParseAsync(streamFile, "TODO:BRANCH", "TODO:VERSION");
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
                    var uniEnvelope = await ParseProcAsync(fullFilePath);

                    if (uniEnvelope == null)
                        _logger.LogError("DTO was not formed after parsing");

                    // Publish result uniEnvelope to UploadService via redis
                    var dto = JsonConvert.SerializeObject(uniEnvelope);
                    await _resultTransport.PublishParsedDtoAsync(dto);
                    _logger.LogInformation("Published to Redis: {DtoJson}", dto);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing file: {Path}", fullFilePath);
                }
            }
                await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
