using System.Xml.Linq;
using System.Globalization;

using TrtParserService.Implementation.ParserCore.Utilities;
using TrtParserService.ParserCore;
using TrtParserService.ParserCore.Extractors;
using TrtParserService.Implementation.ParserCore.Utilities.ValueParsingExtensions;

using TrtShared.Envelope;

namespace TrtParserService.Implementation.ParserCore.TRX.Extractors
{
    /// <summary>
    /// UnitTestResult and UnitTest roots metadata extractor for Test and Results
    /// P.S. It was merged due specific of the TRX
    /// </summary>
    public class TrxTestsResultsExtractor : IXmlExtractor
    {
        public ParserExtension Format => ParserExtension.Trx;

        private long? ReadDuration(string? duration)
        {
            if (duration == null)
                return null;

            var ts = TimeSpan.ParseExact(duration, "c", CultureInfo.InvariantCulture);

            return (ts.Ticks + TimeSpan.TicksPerMillisecond / 2) / TimeSpan.TicksPerMillisecond;
        }

        public void Extract(UniEnvelope envelope, XDocument xDoc, XNamespace? xNS)
        {
            // <UnitTest name="Regression_19787_close_stressTest_OGL" storage="c:\autotests\nano161\nanocad\mgd\unittestsrunner\unittestsrunner.dll" id="1da8c44a-e561-e457-9f90-23652fa09e20">
            //  <TestCategory>
            //    <TestCategoryItem TestCategory="OpenGL" />

            var unitTests = xDoc.Descendants(XmlUtils.ElementNsName("UnitTest", xNS));
            var dicTestCategory = unitTests.ToDictionary
                (
                    ut => ut.Attribute("id")?.Value!,
                    ut => ut.Element(XmlUtils.ElementNsName("TestCategory", xNS))?
                        .Element(XmlUtils.ElementNsName("TestCategoryItem", xNS))?
                        .Attribute("TestCategory")?.Value
                );


            // <UnitTestResult executionId="6b180009-24b6-463c-b718-b046746bda93"
            // testId="1da8c44a-e561-e457-9f90-23652fa09e20" testName="Regression_19787_close_stressTest_OGL"
            // computerName="AT-W10EN-STAND" duration="00:06:30.1028512"
            // startTime="2025-04-16T20:38:19.2176371+03:00" endTime="2025-04-16T20:44:49.3810816+03:00"
            // testType="13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b" outcome="Passed"
            // testListId="8c84fa94-04c1-424b-9868-57a2d4851a1d" relativeResultsDirectory="6b180009-24b6-463c-b718-b046746bda93">

            var unitTestResults = xDoc.Descendants(XmlUtils.ElementNsName("UnitTestResult", xNS));
            var resultList = new List<Dictionary<string, object?>>();

            foreach (var result in unitTestResults)
            {
                // Test Info
                var testName = result.AttrStr("testName");

                // Result Info
                var outcome = result.AttrStr("outcome")?.NormalizeOutcome();
                var startTime = result.AttrDateTime("startTime");
                var endTime = result.AttrDateTime("endTime");
                var duration = ReadDuration(result.AttrStr("duration"));


                // UnitTestResult
                //  Output
                //    DebugTrace
                //    ErrorInfo
                //      Message
                //      StackTrace

                var outputNode = result.Element(XmlUtils.ElementNsName("Output", xNS));
                var errInfoNode = outputNode?.Element(XmlUtils.ElementNsName("ErrorInfo", xNS));

                var debugTrace = outputNode.ElemValStr(xNS, "DebugTrace");
                var errMsg = errInfoNode.ElemValStr(xNS, "Message");
                var errStack = errInfoNode.ElemValStr(xNS, "StackTrace");


                var testResult = new Dictionary<string, object?>
                {
                    [UniEnvelopeSchema.TestInfo.Name] = testName,
                };

                var resultInfo = new Dictionary<string, object?>
                {
                    [UniEnvelopeSchema.ResultInfo.Outcome] = outcome,
                    [UniEnvelopeSchema.ResultInfo.StartedAt] = startTime,
                    [UniEnvelopeSchema.ResultInfo.FinishedAt] = endTime,
                    [UniEnvelopeSchema.ResultInfo.DurationMs] = duration,
                    [UniEnvelopeSchema.ResultInfo.ErrMsg] = errMsg,
                    [UniEnvelopeSchema.ResultInfo.ErrStack] = errStack,
                    [UniEnvelopeSchema.ResultInfo.StdOut] = debugTrace
                };


                envelope.Data[UniEnvelopeSchema.Tests] = testResult;
                envelope.Data[UniEnvelopeSchema.Results] = resultList;
            }
        }
    }
}
