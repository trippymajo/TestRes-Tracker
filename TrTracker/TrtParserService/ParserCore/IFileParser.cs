using TrtShared.Envelope;

namespace TrtParserService.ParserCore
{
    public interface IFileParser
    {
        /// <summary>
        /// Main method for parse logic for the file
        /// </summary>
        /// <param name="streamFile">Stream to the file to parse</param>
        /// <param name="branch">Branch name</param>
        /// <param name="version">Version name (number as string)</param>
        /// <returns>Universal envelope for transfering data to DB</returns>
        public Task<UniEnvelope?> ParseAsync(Stream? streamFile, string branch, string version);
    }
}
