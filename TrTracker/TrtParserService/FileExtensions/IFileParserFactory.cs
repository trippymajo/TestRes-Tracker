using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrtParserService.FileExtensions
{
    interface IFileParserFactory
    {
        public IFileParser? Create(string filePath);
    }
}
