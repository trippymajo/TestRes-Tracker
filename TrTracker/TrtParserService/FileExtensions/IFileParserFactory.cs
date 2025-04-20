using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrtParserService.FileExtensions
{
    public interface IFileParserFactory
    {
        /// <summary>
        /// Factory method for parsing service
        /// </summary>
        /// <param name="filePath">Full file path to file</param>
        /// <returns>
        /// Exact parser for the specified extension 
        /// or null if cannot parse such extension
        /// </returns>
        public IFileParser? Create(string filePath);
    }
}
