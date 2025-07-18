namespace TrtParserService.Implementation.ParserCore.Utilities
{
    public static class ExtractorsUtils
    {
        /// <summary>
        /// Gets list of extractors of the exact format provided
        /// </summary>
        /// <typeparam name="T">Extractor type</typeparam>
        /// <param name="extractors">Enumerable object of the extractors registered</param>
        /// <param name="format">Fomat to get extractors for</param>
        /// <returns></returns>
        public static List<T> GetExactExtractorsList<T>(IEnumerable<T> extractors, string format)
        {
            var formatL = format.ToLowerInvariant();
            return formatL switch
            {
                "trx" => extractors
                            .Where(e => e!.GetType().Namespace?.Contains(".Trx") == true)
                            .ToList(),

                _ => new List<T>()
            };
        }
    }
}
