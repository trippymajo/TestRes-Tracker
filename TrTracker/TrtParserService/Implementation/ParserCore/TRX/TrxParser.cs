using System.Xml;
using System.Xml.Linq;

using TrtParserService.ParserCore;
using TrtParserService.Implementation.ParserCore.TRX.Extractors;

using TrtShared.Envelope;

namespace TrtParserService.Implementation.ParserCore.TRX
{
    class TrxParser : IFileParser
    {
        private readonly ILogger<TrxParser> _logger;
        private readonly List<IXmlExtractor> _extractors;

        private XDocument? _xDoc;
        private XNamespace? _xNamespace;


        public TrxParser(ILogger<TrxParser> logger)
        {
            _logger = logger;
            _xDoc = new XDocument();
            _xNamespace = null;

            _extractors = new List<IXmlExtractor>
            {
                new TrxTestRunExtractor(),
                new TrxTimesExtractor(),
                new TrxCountersExtractor(),
                new TrxTestsResultsExtractor()
            };

        }

        public async Task<UniEnvelope?> ParseAsync(Stream? streamFile, string branch, string version)
        {
            if (streamFile == null)
            {
                _logger.LogWarning("Parse failed. File stream is null");
                return null;
            }

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
                _xDoc = await XDocument.LoadAsync(reader, LoadOptions.None, CancellationToken.None);
                _xNamespace = _xDoc.Root?.GetDefaultNamespace();
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
                    _extractors[i].Extract(envelope, _xDoc, _xNamespace);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Parse failed. An error occured while extracting data from document");
                return null;
            }

            if (envelope.Data.Count > 0)
            {
                _logger.LogWarning("Parse failed. No results have been parsed from file");
                return null;
            }

            return envelope;
        }
    }
}
