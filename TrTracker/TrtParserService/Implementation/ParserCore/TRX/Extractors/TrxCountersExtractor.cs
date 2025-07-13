using System.Xml.Linq;
using TrtParserService.Implementation.ParserCore.Utilities;
using TrtParserService.ParserCore;
using TrtShared.Envelope;

namespace TrtParserService.Implementation.ParserCore.TRX.Extractors
{
    /// <summary>
    /// Counters root metadata extractor
    /// </summary>
    public class TrxCountersExtractor : IXmlExtractor
    {
        public void Extract(UniEnvelope envelope, XDocument xDoc, XNamespace? xNS)
        {
            // <Counters total="578" executed="578"
            // passed="432" error="0" failed="146"
            // timeout="0" aborted="0" inconclusive="0"
            // passedButRunAborted="0" notRunnable="0"
            // notExecuted="0" disconnected="0" warning="0" completed="0"
            // inProgress="0" pending="0" />

            // Hmmm consider using Attributes and foreach
            var element = xDoc.Root?
                .Element(XmlUtils.ElementNsName("Counters", xNS));

            if (element == null)
                return;

            var total = element.Attribute("total")?.Value;
            var executed = element.Attribute("executed")?.Value;
            var passed = element.Attribute("passed")?.Value;
            var failed = element.Attribute("failed")?.Value;

            envelope.Data["total"] = total;
            envelope.Data["executed"] = executed;
            envelope.Data["passed"] = passed;
            envelope.Data["failed"] = failed;
        }
    }
}
