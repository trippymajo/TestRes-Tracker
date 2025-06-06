namespace TrtParserService.FileReader
{
    public class LocalFileReader : IFileReader
    {
        private readonly ILogger<LocalFileReader> _logger;

        public LocalFileReader(ILogger<LocalFileReader> logger)
        {
            _logger = logger;
        }

        public Task<Stream?> OpenFileStreamAsync(string keyPath)
        {
            if (string.IsNullOrEmpty(keyPath))
            {
                _logger.LogWarning("Parse failed. Document {KeyPath} is null", keyPath);
                return Task.FromResult<Stream?>(null);
            }

            try
            {
                var stream = File.OpenRead(keyPath);
                return Task.FromResult<Stream?>(stream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed creating stream to parsing file {KeyPath}", keyPath);
                return Task.FromResult<Stream?>(null);
            }
        }
    }
}
