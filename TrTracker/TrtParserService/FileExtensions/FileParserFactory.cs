using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace TrtParserService.FileExtensions
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

            switch (ext)
            {
                case ".trx":
                    return _provider.GetRequiredService<TrxParser>();
                case ".xml":
                    return null;
            }

            return null;
        }
    }
}
