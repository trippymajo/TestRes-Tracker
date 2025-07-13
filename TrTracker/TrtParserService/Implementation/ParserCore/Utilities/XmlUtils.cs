using System.Xml.Linq;

namespace TrtParserService.Implementation.ParserCore.Utilities
{
    public static class XmlUtils
    {
        public static XName ElementNsName(string elemName, XNamespace? ns)
        {
            return ns == null || ns == XNamespace.None
                ? (XName)elemName
                : ns + elemName;
        }
    }
}
