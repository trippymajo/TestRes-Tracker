using System.Xml.Linq;

using TrtParserService.Implementation.ParserCore.Utilities;
using TrtParserService.Implementation.ParserCore.Utilities.ValueParsingExtensions;
using TrtParserService.ParserCore;
using TrtParserService.ParserCore.Extractors;

using TrtShared.Envelope;

namespace TrtParserService.Implementation.ParserCore.TRX.Extractors
{
    /// <summary>
    /// Counters root metadata extractor for TestRun info
    /// </summary>
    public class TrxCountersExtractor : IXmlExtractor
    {
        public ParserExtension Format => ParserExtension.Trx;

        public void Extract(UniEnvelope envelope, XDocument xDoc, XNamespace? xNS)
        {
            // <Counters total="578" executed="578"
            // passed="432" error="0" failed="146"
            // timeout="0" aborted="0" inconclusive="0"
            // passedButRunAborted="0" notRunnable="0"
            // notExecuted="0" disconnected="0" warning="0" completed="0"
            // inProgress="0" pending="0" />

            var element = xDoc.Root?
                .Element(XmlUtils.ElementNsName("Counters", xNS));

            if (element == null)
                return;

            var total = element.AttrInt("total");
            var passed = element.AttrInt("passed");
            var failed = element.AttrInt("failed");
            // Error: Error, Aborted, Timeout, Disconnected, PassedButRunAborted
            var error = element.AttrInt("error");
            var notRunable = element.AttrInt("notRunnable");
            var notExecuted = element.AttrInt("notExecuted");
            var inconclus = element.AttrInt("inconclusive");
            var passButAbort = element.AttrInt("passedButRunAborted");
            var aborted = element.AttrInt("aborted");
            var skipped = notRunable + notExecuted
                + inconclus + passButAbort + aborted;


            envelope.Data[UniEnvelopeSchema.Agregates] =
                new Dictionary<string, object?>()
                {
                    [UniEnvelopeSchema.TestrunAgregates.Total] = total,
                    [UniEnvelopeSchema.TestrunAgregates.Passed] = passed,
                    [UniEnvelopeSchema.TestrunAgregates.Failed] = failed,
                    [UniEnvelopeSchema.TestrunAgregates.Skipped] = skipped,
                    [UniEnvelopeSchema.TestrunAgregates.Errors] = error
                };
        }
    }
}
