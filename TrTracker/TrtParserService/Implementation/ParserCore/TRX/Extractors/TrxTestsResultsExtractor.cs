using System.Xml.Linq;
using TrtParserService.Implementation.ParserCore.Utilities;
using TrtParserService.ParserCore;
using TrtShared.Envelope;

namespace TrtParserService.Implementation.ParserCore.TRX.Extractors
{
    /// <summary>
    /// UnitTestResult and UnitTest roots metadata extractor
    /// P.S. It was merged due specific of the TRX
    /// </summary>
    public class TrxTestsResultsExtractor : IXmlExtractor
    {

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

            var listUnitTestResults = xDoc.Descendants(XmlUtils.ElementNsName("UnitTestResult", xNS))
                    .ToList();

            var resultList = new List<Dictionary<string, object?>>();

            for (int i = 0; i < listUnitTestResults.Count; i++)
            {
                var result = listUnitTestResults[i];

                var testId = result.Attribute("testId")?.Value;
                var testName = result.Attribute("testName")?.Value;
                var duration = result.Attribute("duration")?.Value;
                var outcome = result.Attribute("outcome")?.Value;
                var errMessage = result
                    .Element(XmlUtils.ElementNsName("ErrorInfo", xNS))?
                    .Element(XmlUtils.ElementNsName("Message", xNS))?.Value;

                var category = dicTestCategory?.GetValueOrDefault(testId!);

                var testResult = new Dictionary<string, object?>
                {
                    ["testName"] = testName,
                    ["duration"] = duration,
                    ["outcome"] = outcome,
                    ["errorInfo"] = errMessage,
                    ["category"] = category
                };

                resultList.Add(testResult);
            }

            envelope.Data["Results"] = resultList;
        }
    }
}
