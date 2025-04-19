using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TrtShared.DTO;

namespace TrtParserService.FileExtensions
{
    class TrxParser : IFileParser
    {
        private  XDocument? _xDoc;

        public TrxParser()
        {
            _xDoc = new XDocument();
        }

        public DateTime? ParseDate()
        {
            if (_xDoc == null)
                return null;

            //   <Times creation="2025-04-16T16:16:21.6226347+03:00"
            //   queuing="2025-04-16T16:16:32.9976281+03:00"
            //   start="2025-04-16T16:16:33.2320172+03:00"
            //   finish="2025-04-16T23:54:05.470844+03:00" />
            var timeFinish = _xDoc.Root?
                .Element("Times")?
                .Attribute("finish")?.Value;

            DateTime? retVal = null;
            if (timeFinish != null)
                retVal = DateTime.Parse(timeFinish);

            return retVal;
        }

        public IDictionary<string, (string outcome, string? error)>? ParseResults()
        {
            if (_xDoc == null)
                return null;

            // <UnitTestResult executionId="222" testId="123" testName="Test123" computerName="StandName"
            // duration="00:00:31.8927852" startTime="2025-04-16T16:58:54.3104569+03:00"
            // endTime="2025-04-16T16:59:26.2166433+03:00" testType="123"
            // outcome="Passed" testListId="123" relativeResultsDirectory="111">
            var unitTestResult = _xDoc.Descendants("UnitTestResult")
                .Where(utr => utr.Attribute("testName") != null && utr.Attribute("outcome") != null)
                .ToDictionary
                (
                    utr => utr.Attribute("testName")?.Value!,
                    utr =>
                    (
                        outcome: utr.Attribute("outcome")?.Value!,
                        error: utr.Element("Output")?
                            .Element("ErrorInfo")?
                            .Element("Message")?.Value
                    )
                );

            return unitTestResult;
        }

        public async Task<TestRunDTO?> Parse(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            await using var fstream = File.OpenRead(path);
            _xDoc = await XDocument.LoadAsync(fstream, LoadOptions.None, CancellationToken.None);

            //// Get UnitTest info
            ////  <UnitTest name="Test123" storage="path" id="123">
            //var unitTests = _xDoc.Descendants("UnitTest")
            //    .Where(ut => ut.Attribute("id") != null && ut.Attribute("name") != null)
            //    .ToDictionary
            //    (
            //        ut => ut.Attribute("id")?.Value!,
            //        ut => ut.Attribute("name")?.Value!
            //    );

            // Parse Trx file.
            var date = ParseDate() ?? DateTime.UtcNow;
            var results = ParseResults();
            // var branch = do something to get branch name
            // var version = do something to get version

            if (results == null) 
                return null;

            // Create DTO's
            var resultDtos = results.Select(r => new ResultDTO
            {
                TestName = r.Key,
                Outcome = r.Value.outcome,
                ErrMsg = r.Value.error
            }).ToList();

            var testRunDto = new TestRunDTO
            {
                // Branch = branch,
                // Version = version,
                Date = date,
                Results = resultDtos
            };


            return testRunDto;
        }
    }
}
