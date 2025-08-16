using System.Globalization;
using System.Xml.Linq;

namespace TrtParserService.Implementation.ParserCore.Utilities.ValueParsingExtensions
{
    public static class XmlValueExtensions
    {
        /// <summary>
        /// Reads attribute as string
        /// </summary>
        /// <returns>Parsed string or null if string not recognized</returns>
        public static string? AttrStr(this XElement? e, XName name)
        {
            var s = e?.Attribute(name)?.Value;
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }

        /// <summary>
        /// Reads attribute as int
        /// </summary>
        /// <returns>Parsed int or null if string not recognized</returns>
        public static int? AttrInt(this XElement? e, XName name)
        {
            var s = e?.Attribute(name)?.Value;
            if (string.IsNullOrWhiteSpace(s)) 
                return null;

            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)
                ? v
                : null;
        }

        /// <summary>
        /// Reads attribute as DateTimeOffset in ISO-8601 ("2025-04-16T16:16:21.6226347+03:00")
        /// Don't forget to do normalization for the output in order to get 
        /// </summary>
        /// <returns>DateTimeOffset or null if string not recognized</returns>
        public static DateTimeOffset? AttrDateTime(this XElement? e, XName name)
        {
            var s = e?.Attribute(name)?.Value;
            if (string.IsNullOrWhiteSpace(s)) 
                return null;

            if (DateTimeOffset.TryParseExact(s, "o", CultureInfo.InvariantCulture,
                                             DateTimeStyles.RoundtripKind, out var dtoExact))
                return dtoExact;

            if (DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture,
                                        DateTimeStyles.RoundtripKind, out var dto))
                return dto;

            return null;
        }

        /// <summary>
        /// Reads element's value as string
        /// </summary>
        /// <param name="xNS">Current namespace</param>
        /// <returns>Parsed string or null if string not recognized</returns>
        public static string? ElemValStr(this XElement? e, XNamespace? xNS, string elemName)
        {
            var s = e?.Element(XmlUtils.ElementNsName(elemName, xNS))?.Value;
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }
    }
}
