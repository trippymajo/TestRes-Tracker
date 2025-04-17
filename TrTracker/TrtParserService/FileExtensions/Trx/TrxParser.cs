using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TrtParserService.FileExtensions
{
    class TrxParser : IFileParser
    {
        private  XDocument? _xDoc;

        public TrxParser()
        {
            _xDoc = new XDocument();
        }

        public async Task<string?> Parse(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            await using var fstream = File.OpenRead(path);
            _xDoc = await XDocument.LoadAsync(fstream, LoadOptions.None, CancellationToken.None);

            // ### First step ###
            // Get UnitTest info
            //  <UnitTest name="Test123" storage="path" id="123">
            var UnitTests = _xDoc.Descendants("UnitTest")
                .Where(ut => ut.Attribute("id") != null && ut.Attribute("name") != null)
                .ToDictionary
                (
                    ut => ut.Attribute("id")?.Value!,
                    ut => ut.Attribute("name")?.Value!
                );

            
        }
    }
}
