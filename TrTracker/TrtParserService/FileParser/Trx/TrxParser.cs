using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TrtShared.DTO;

namespace TrtParserService.FileExtensions
{
    class TrxParser : IFileParser
    {
        private readonly ILogger<TrxParser> _logger;
        private XDocument? _xDoc;

        public TrxParser(ILogger<TrxParser> logger)
        {
            _logger = logger;
            _xDoc = new XDocument();
        }

        public DateTime? ParseDate()
        {
            if (_xDoc == null)
            {
                _logger.LogWarning("Trying parsing date failed. Document is null");
                return null; 
            }

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
            {
                _logger.LogWarning("Trying parsing tests info failed. Document is null");
                return null;
            }

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

        public async Task<TestRunDTO?> Parse(string path, string branch, string version)
        {
            if (string.IsNullOrEmpty(path))
            {
                _logger.LogWarning("Parse failed. Document is null");
                return null;
            }

            // Avoid XML injections
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            };

            using var fstream = File.OpenRead(path);
            using var reader = XmlReader.Create(fstream, settings);
            _xDoc = await XDocument.LoadAsync(reader, LoadOptions.None, CancellationToken.None);

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
            {
                _logger.LogWarning("Parse failed. No results have been parsed from file");
                return null;
            }

            // Create DTO's
            var resultDtos = results.Select(r => new ResultDTO
            {
                TestName = r.Key,
                Outcome = r.Value.outcome,
                ErrMsg = r.Value.error
            }).ToList();

            var testRunDto = new TestRunDTO
            {
                Branch = branch,
                Version = version,
                Date = date,
                Results = resultDtos
            };


            return testRunDto;
        }
    }
}
