using System.Xml;
using System.Xml.Linq;

using TrtShared.DTO;
using TrtParserService.ParserCore;

namespace TrtParserService.Implementation.ParserCore
{
    class TrxParser : IFileParser
    {
        private readonly ILogger<TrxParser> _logger;
        private XDocument? _xDoc;
        private XNamespace? _xNamespace;

        public TrxParser(ILogger<TrxParser> logger)
        {
            _logger = logger;
            _xDoc = new XDocument();
            _xNamespace = null;
        }

        /// <summary>
        /// Makes a full element name with current default namespace
        /// </summary>
        /// <param name="elemName">Element name</param>
        /// <returns>Full XName for the element</returns>
        private XName ElementNSName(string elemName)
        {
            return (_xNamespace == null || _xNamespace == XNamespace.None) ? (XName)elemName : _xNamespace + elemName;
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
                .Element(ElementNSName("Times"))?
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
            var unitTestResults = _xDoc.Descendants(ElementNSName("UnitTestResult"))
                .Where(utr => utr.Attribute("testName") != null && utr.Attribute("outcome") != null)
                .ToDictionary
                (
                    utr => utr.Attribute("testName")?.Value!,
                    utr =>
                    (
                        outcome: utr.Attribute("outcome")?.Value!,
                        error: utr.Element(ElementNSName("Output"))?
                            .Element(ElementNSName("ErrorInfo"))?
                            .Element(ElementNSName("Message"))?.Value
                    )
                );

            // For debug and testing, then clear this trash....
            //var xmlNS = _xDoc.Root?.GetDefaultNamespace();
            //var allResults = _xDoc.Descendants(ElementNSName("UnitTestResult")).ToList();
            //var listFiltered = allResults.Where(utr => utr.Attribute("testName") != null && utr.Attribute("outcome") != null).ToList();
            //var dictOut = listFiltered.ToDictionary(
            //    utr => utr.Attribute("testName")?.Value!,
            //    utr => (
            //        outcome: utr.Attribute("outcome")?.Value!,
            //        error: utr.Element(ElementNSName("Output"))?
            //            .Element(ElementNSName("ErrorInfo"))?
            //            .Element(ElementNSName("Message"))?.Value
            //    )
            //);

            return unitTestResults;
        }

        public async Task<TestRunDTO?> ParseAsync(Stream? streamFile, string branch, string version)
        {
            if (streamFile == null)
            {
                _logger.LogWarning("Parse failed. File stream is null");
                return null;
            }

            // Parse products:
            DateTime date = DateTime.UtcNow;
            IDictionary<string, (string outcome, string? error)>? results = null;
            //string? branch = null;
            //string? version = null;

            // Avoid XML injections
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                Async = true
            };

            try
            {
                using var reader = XmlReader.Create(streamFile, settings);
                _xDoc = await XDocument.LoadAsync(reader, LoadOptions.None, CancellationToken.None);
                _xNamespace = _xDoc.Root?.GetDefaultNamespace();

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
                date = await Task.Run(() => ParseDate()) ?? DateTime.UtcNow;
                results = await Task.Run(() => ParseResults());
                // branch = do something to get branch name
                // version = do something to get version
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Parse failed. Cannot read stream of document to parse");
                return null;
            }

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
