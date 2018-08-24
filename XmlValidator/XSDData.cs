using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XmlValidator
{
    public class XSDData
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public Dictionary<string, string> Structure = new Dictionary<string, string>();

        public XSDData(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);
            var content = File.ReadAllText(path, Encoding.UTF8);
            Content = content;

            Structure = ParseStructure(ref content);
        }

        private Dictionary<string, string> ParseStructure(ref string content)
        {
            // TODO parse structure of XSD
            var doc = XDocument.Parse(content);
            var elements = doc.Elements().Where(el => el.Attribute("name").Value == "entities");

            return null;
        }
    }
}
