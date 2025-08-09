using TrtParserService.ParserCore;
using TrtParserService.ParserCore.Extractors;

namespace TrtParserService.Implementation.ParserCore.Utilities
{
    public static class ExtractorsUtils
    {
        /// <summary>
        /// Gets list of extractors of the exact format provided
        /// </summary>
        /// <typeparam name="T">Extractor type</typeparam>
        /// <param name="extractors">Enumerable object of the extractors registered</param>
        /// <param name="ext">Fomat to get extractors for</param>
        /// <returns></returns>
        public static List<T> GetExactExtractorsList<T>(IEnumerable<T> extractors, ParserExtension ext) where T : IXmlExtractor
        {
            return extractors.Where(e => (e.Format & ext) != 0).ToList();
        }
    }
}
