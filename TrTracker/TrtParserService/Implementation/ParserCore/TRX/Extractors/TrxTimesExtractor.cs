using System.Xml.Linq;
using TrtParserService.Implementation.ParserCore.Utilities;
using TrtParserService.ParserCore.Extractors;
using TrtShared.Envelope;

namespace TrtParserService.Implementation.ParserCore.TRX.Extractors
{
    /// <summary>
    /// Times root metadata extractor
    /// </summary>
    public class TrxTimesExtractor : IXmlExtractor
    {
        public void Extract(UniEnvelope envelope, XDocument xDoc, XNamespace? xNS)
        {
            //   <Times creation="2025-04-16T16:16:21.6226347+03:00"
            //   queuing="2025-04-16T16:16:32.9976281+03:00"
            //   start="2025-04-16T16:16:33.2320172+03:00"
            //   finish="2025-04-16T23:54:05.470844+03:00" />

            var finish = xDoc.Root?
                .Element(XmlUtils.ElementNsName("Times", xNS))?
                .Attribute("finish")?.Value;

            envelope.Data["finish"] = finish;
        }
    }
}
