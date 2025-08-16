using System.Xml.Linq;

using TrtParserService.Implementation.ParserCore.Utilities;
using TrtParserService.Implementation.ParserCore.Utilities.ValueParsingExtensions;
using TrtParserService.ParserCore;
using TrtParserService.ParserCore.Extractors;

using TrtShared.Envelope;

namespace TrtParserService.Implementation.ParserCore.TRX.Extractors
{
    /// <summary>
    /// Times root metadata extractor for TestRun info
    /// </summary>
    public class TrxTimesExtractor : IXmlExtractor
    {
        public ParserExtension Format => ParserExtension.Trx;

        private long? GetDuration(DateTimeOffset? start, DateTimeOffset? finish)
        {
            if (start.HasValue && finish.HasValue)
                return null;

            return Math.Max(0L, (finish.Value - start.Value).Ticks / TimeSpan.TicksPerMillisecond);
        }

        public void Extract(UniEnvelope envelope, XDocument xDoc, XNamespace? xNS)
        {
            //   <Times creation="2025-04-16T16:16:21.6226347+03:00"
            //   queuing="2025-04-16T16:16:32.9976281+03:00"
            //   start="2025-04-16T16:16:33.2320172+03:00"
            //   finish="2025-04-16T23:54:05.470844+03:00" />

            var start = xDoc.Root?
                .Element(XmlUtils.ElementNsName("Times", xNS))?
                .AttrDateTime("start");

            var finish = xDoc.Root?
                .Element(XmlUtils.ElementNsName("Times", xNS))?
                .AttrDateTime("finish");

            var duration = GetDuration(start, finish);

            envelope.Data[UniEnvelopeSchema.StartedAt] = start.ToUtcIso();
            envelope.Data[UniEnvelopeSchema.FinishedAt] = finish.ToUtcIso();
            envelope.Data[UniEnvelopeSchema.DurationMs] = duration;
        }
    }
}
