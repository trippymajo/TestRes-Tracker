using TrtParserService.Implementation.ParserCore.TRX;
using TrtParserService.ParserCore;

namespace TrtParserService.Implementation.ParserCore
{
    class FileParserFactory : IFileParserFactory
    {
        private readonly IServiceProvider _provider;
        public FileParserFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IFileParser? Create(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();

            return ext switch
            {
                ".trx" => _provider.GetRequiredService<TrxParser>(),

                ".xml" => null,

                _ => null
            };
        }
    }
}
