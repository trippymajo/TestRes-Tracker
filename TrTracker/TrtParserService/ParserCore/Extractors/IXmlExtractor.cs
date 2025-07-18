using System.Xml.Linq;
using TrtShared.Envelope;

namespace TrtParserService.ParserCore.Extractors
{
    public interface IXmlExtractor
    {
        /// <summary>
        /// Extract specific xml data from test result format
        /// </summary>
        /// <param name="envelope">Universal envelope for transfering data</param>
        /// <param name="xDoc">Xml document to read</param>
        /// <param name="xNS">Namespace used in the document</param>
        public void Extract(UniEnvelope envelope, XDocument xDoc, XNamespace? xNS);
    }
}
