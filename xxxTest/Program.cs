using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace xxxTest
{
    internal class Program
    {
        private static XSDTreeNode rootNode = new XSDTreeNode("root");
        private const string xsdPath = "test.xsd";

        private static void Main(string[] args)
        {
            var content = File.ReadAllText(xsdPath);

            try
            {
                StreamReader tr = new StreamReader(xsdPath);
                var schema = XmlSchema.Read(tr, new ValidationEventHandler(SchemaValidationHandler));
                tr.Close();
                CompileSchema(schema);
                CreateRootNode();
                DecodeSchema(schema, rootNode);
            }
            catch (Exception err)
            {
            }
        }

        private static void CreateRootNode()
        {
            rootNode = new XSDTreeNode("root");
        }

        private static void CompileSchema(XmlSchema schema)
        {
            schema.Compile(new ValidationEventHandler(SchemaValidationHandler));
        }

        private static void SchemaValidationHandler(object sender, ValidationEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
