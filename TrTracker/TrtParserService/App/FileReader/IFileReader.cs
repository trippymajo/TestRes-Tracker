namespace TrtParserService.App.FileReader
{
    public interface IFileReader
    {
        /// <summary>
        ///  Asyncroniously creates Stream to the file to be parsed
        /// </summary>
        /// <param name="keyPath">Path/Key to file to be opened</param>
        /// <returns>Stream to file should be parsed</returns>
        public Task<Stream?> OpenFileStreamAsync(string keyPath);
    }
}
