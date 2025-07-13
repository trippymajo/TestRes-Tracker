using System.Xml.Linq;
using TrtParserService.ParserCore;
using TrtShared.Envelope;

namespace TrtParserService.Implementation.ParserCore.TRX.Extractors
{
    /// <summary>
    /// TestRun root metadata extractor
    /// </summary>
    public class TrxTestRunExtractor : IXmlExtractor
    {
        public void Extract(UniEnvelope envelope, XDocument xDoc, XNamespace? xNS = null)
        {
            //<TestRun xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            //xmlns:xsd="http://www.w3.org/2001/XMLSchema"
            //id="8945d9a3-ed99-458e-8db8-11cbeabbf745"
            //name="tester@AT-W10EN-STAND 2025-04-16 16:16:21"
            //runUser="AT-W10EN-STAND\tester"
            //xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">

            var name = xDoc.Root?
                .Attribute("name")?.Value;

            var runUser = xDoc.Root?
                .Attribute("runUser");

            envelope.Data["name"] = name;
            envelope.Data["runUser"] = runUser;
        }
    }
}
