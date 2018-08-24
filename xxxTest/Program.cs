using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace xxxTest
{
    internal class Program
    {
        private const string xsdPath = "test.xsd";

        private static void Main(string[] args)
        {
            var content = File.ReadAllText(xsdPath);

            var doc = XDocument.Parse(content);

            var xs = XNamespace.Get("http://www.w3.org/2001/XMLSchema");

            foreach (XElement element in doc.Descendants())
            {
                Console.WriteLine(element.NodeType.ToString()
                    + " - " + element.Name.LocalName
                    + " - "
                    + string.Join(", ", element.Attributes().Select(a => a.ToString())));
            }
        }
    }
}
