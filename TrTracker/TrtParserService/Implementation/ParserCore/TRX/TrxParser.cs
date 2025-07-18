using System.Xml;
using System.Xml.Linq;

using TrtParserService.ParserCore;
using TrtParserService.ParserCore.Extractors;

using TrtParserService.Implementation.ParserCore.Utilities;

using TrtShared.Envelope;

namespace TrtParserService.Implementation.ParserCore.TRX
{
    class TrxParser : IFileParser
    {
        private readonly ILogger<TrxParser> _logger;
        private readonly List<IXmlExtractor> _extractors;

        public TrxParser(IEnumerable<IXmlExtractor> extractors, ILogger<TrxParser> logger)
        {
            _logger = logger;
            _extractors = ExtractorsUtils.GetExactExtractorsList(extractors, "trx");
        }

        public async Task<UniEnvelope?> ParseAsync(Stream? streamFile, string branch, string version)
        {
            if (streamFile == null)
            {
                _logger.LogWarning("Parse failed. File stream is null");
                return null;
            }

            XDocument? xDoc = new XDocument();
            XNamespace? xNamespace = null;

            // Avoid XML injections
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                Async = true
            };

            try
            {
                using var reader = XmlReader.Create(streamFile, settings);
                xDoc = await XDocument.LoadAsync(reader, LoadOptions.None, CancellationToken.None);
                xNamespace = xDoc.Root?.GetDefaultNamespace();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Parse failed. Cannot read stream of document to parse");
                return null;
            }

            var envelope = new UniEnvelope();
            envelope.SchemaId = "default";

            try
            {
                for (int i = 0; i < _extractors.Count; i++)
                    _extractors[i].Extract(envelope, xDoc, xNamespace);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Parse failed. An error occured while extracting data from document");
                return null;
            }

            if (envelope.Data.Count == 0)
            {
                _logger.LogWarning("Parse failed. No results have been parsed from file");
                return null;
            }

            return envelope;
        }
    }
}
