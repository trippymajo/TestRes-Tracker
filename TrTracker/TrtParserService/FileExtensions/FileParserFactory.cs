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
        public IFileParser? Create(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();

            switch (ext)
            {
                case ".trx":
                    return new TrxParser();
                case ".xml":
                    return null;///
            }

            return null;
        }
    }
}
