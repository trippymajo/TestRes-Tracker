using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

using TrtParserService.Implementation.ParserCore.Utilities;
using TrtParserService.Implementation.ParserCore.Utilities.ValueParsingExtensions;
using TrtParserService.ParserCore;
using TrtParserService.ParserCore.Extractors;

using TrtShared.Envelope;

namespace TrtParserService.Implementation.ParserCore.TRX.Extractors
{
    /// <summary>
    /// TestRun root metadata extractor for TestRun info
    /// </summary>
    public class TrxTestRunExtractor : IXmlExtractor
    {
        public ParserExtension Format => ParserExtension.Trx;

        private static readonly JsonSerializerOptions envJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

        public void Extract(UniEnvelope envelope, XDocument xDoc, XNamespace? xNS = null)
        {
            //<TestRun xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            //xmlns:xsd="http://www.w3.org/2001/XMLSchema"
            //id="8945d9a3-ed99-458e-8db8-11cbeabbf745"
            //name="tester@AT-W10EN-STAND 2025-04-16 16:16:21"
            //runUser="AT-W10EN-STAND\tester"
            //xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">

            var runUser = xDoc.Root?.AttrStr("runUser")?.TrimEnd();


            // <UnitTestResult executionId="6b180009-24b6-463c-b718-b046746bda93"
            // testId="1da8c44a-e561-e457-9f90-23652fa09e20" testName="Regression_19787_close_stressTest_OGL"
            // computerName="AT-W10EN-STAND" duration="00:06:30.1028512"
            // startTime="2025-04-16T20:38:19.2176371+03:00" endTime="2025-04-16T20:44:49.3810816+03:00"
            // testType="13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b" outcome="Passed"
            // testListId="8c84fa94-04c1-424b-9868-57a2d4851a1d" relativeResultsDirectory="6b180009-24b6-463c-b718-b046746bda93">

            var anyResult = xDoc.Descendants(XmlUtils.ElementNsName("UnitTestResult", xNS)).FirstOrDefault();
            var computerName = anyResult?.AttrStr("computerName");

            var dicEnv = new Dictionary<string, object?>()
            {
                ["runner"] = "MsTest",
                ["computerName"] = computerName,
                ["runUser"] = runUser
            };

            envelope.Data[UniEnvelopeSchema.EnvironmentJson] = JsonSerializer.Serialize(dicEnv, envJsonOptions);
        }
    }
}
