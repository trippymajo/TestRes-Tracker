﻿using TrtShared.DTO;

namespace TrtParserService.ParserCore
{
    public interface IFileParser
    {
        /// <summary>
        /// Get Date info for TestRun
        /// </summary>
        /// <returns>DateTime when testRung has finished</returns>
        public DateTime? ParseDate();

        /// <summary>
        /// Get UnitTestResult info with TestName, oucome and error msg
        /// </summary>
        /// <returns>
        /// Dictionary with testName as KEY
        /// and Outcome and optional error msg as TUPLE
        /// </returns>
        public IDictionary<string, (string outcome, string? error)>? ParseResults();

        /// <summary>
        /// Main method for parse logic for the file
        /// </summary>
        /// <param name="path">Path to the file to parse</param>
        /// <param name="branch">Branch name</param>
        /// <param name="version">Version name (number as string)</param>
        /// <returns>TestRunDTO for transfering data to DB</returns>
        public Task<TestRunDTO?> ParseAsync(Stream? streamFile, string branch, string version);
    }
}
