using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XmlValidator
{
    public class XSDData
    {
        public readonly XSDTreeNode RootNode = new XSDTreeNode("root", "type", false);

        public string Path { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        public XSDData(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);
            var content = File.ReadAllText(path, Encoding.UTF8);
            Content = content;

            var doc = XDocument.Parse(content);
            try
            {
                StreamReader tr = new StreamReader(Path);
                var schema = XmlSchema.Read(tr, new ValidationEventHandler(SchemaValidationHandler));
                tr.Close();

                // compile schema
                schema.Compile(new ValidationEventHandler(SchemaValidationHandler));

                // Create Root Node
                RootNode = new XSDTreeNode("root", "type", false);

                DecodeSchema(schema, RootNode);
            }
            catch (Exception err)
            {
                // TODO handle exception?
            }
        }

        private void SchemaValidationHandler(object sender, ValidationEventArgs e)
        {
            // TODO invalid schema handler
        }

        private void DecodeSchema(XmlSchema schema, XSDTreeNode node)
        {
            try
            {
                DecodeSchema2(schema, node);
            }
            catch (Exception err)
            {
            }
        }

        // recursive decoder
        private XSDTreeNode DecodeSchema2(XmlSchemaObject obj, XSDTreeNode node)
        {
            XSDTreeNode newNode = node;

            // convert the object to all types and then check what type actually exists
            XmlSchemaAnnotation annot = obj as XmlSchemaAnnotation;
            XmlSchemaAttribute attrib = obj as XmlSchemaAttribute;
            XmlSchemaFacet facet = obj as XmlSchemaFacet;
            XmlSchemaDocumentation doc = obj as XmlSchemaDocumentation;
            XmlSchemaAppInfo appInfo = obj as XmlSchemaAppInfo;
            XmlSchemaElement element = obj as XmlSchemaElement;
            XmlSchemaSimpleType simpleType = obj as XmlSchemaSimpleType;
            XmlSchemaComplexType complexType = obj as XmlSchemaComplexType;

            // if annotation, add a tree node and recurse for documentation and app info
            if (annot != null)
            {
                newNode = new XSDTreeNode("--annotation--", "--annotation--", false);
                node.Add(newNode);
                foreach (XmlSchemaObject schemaObject in annot.Items)
                {
                    DecodeSchema2(schemaObject, newNode);
                }
            }
            // if attribute, add an attribute at tree node
            else if (attrib != null)
            {
                node.AddAttribute(attrib.QualifiedName.Name, attrib.SchemaTypeName.Name);
            }
            // if facet, add a tree node
            else if (facet != null)
            {
                newNode = new XSDTreeNode(facet.ToString(), facet.ToString(), false);
                node.Add(newNode);
            }
            // if documentation, add a tree node
            else if (doc != null)
            {
                newNode = new XSDTreeNode("--documentation--", "--documentation--", false);
                node.Add(newNode);
            }
            // if app info, add a tree node
            else if (appInfo != null)
            {
                newNode = new XSDTreeNode("--app info--", "--app info--", false);
                node.Add(newNode);
            }
            // if an element, determine whether the element is a simple type or a complex type
            else if (element != null)
            {
                XmlSchemaSimpleType st = element.SchemaType as XmlSchemaSimpleType;
                XmlSchemaComplexType ct = element.SchemaType as XmlSchemaComplexType;

                if (st != null)
                {
                    // this is a simple type element.  Recurse.
                    XSDTreeNode node2 = DecodeSchema2(st, newNode);
                    node2.Name = element.Name;
                    node2.Element = true;
                    node2.ComplexType = false;
                }
                else if (ct != null)
                {
                    // this is a complex type element.  Recurse.
                    XSDTreeNode node2 = DecodeSchema2(ct, newNode);
                    newNode.Remove(node2);
                    node2.Name = element.Name;
                    node2.Element = true;
                    node2.ComplexType = true;
                    newNode.Add(node2);
                }
                else
                {
                    // This is a plain ol' fashioned element.
                    newNode = new XSDTreeNode(element.QualifiedName.Name, element.SchemaTypeName.Name, false);
                    node.Add(newNode);
                }
            }
            // if a simple type, then add a tree node and recurse facets
            else if (simpleType != null)
            {
                newNode = new XSDTreeNode(simpleType.QualifiedName.Name, simpleType.BaseXmlSchemaType.Name, false);
                node.Add(newNode);
                XmlSchemaSimpleTypeRestriction rest = simpleType.Content as XmlSchemaSimpleTypeRestriction;
                if (rest != null)
                {
                    foreach (XmlSchemaFacet schemaObject in rest.Facets)
                    {
                        DecodeSchema2(schemaObject, newNode);
                    }
                }
            }
            // if a complex type, add a tree node and recurse its sequence
            else if (complexType != null)
            {
                if (null == complexType.Name)
                {
                    newNode = new XSDTreeNode("--tmp-name--", complexType.BaseXmlSchemaType.Name, true);
                }
                else
                {
                    newNode = new XSDTreeNode(complexType.Name, complexType.BaseXmlSchemaType.Name, true);
                }

                node.Add(newNode);

                XmlSchemaSequence seq = complexType.Particle as XmlSchemaSequence;
                if (seq != null)
                {
                    foreach (XmlSchemaObject schemaObject in seq.Items)
                    {
                        DecodeSchema2(schemaObject, newNode);
                    }
                }
            }

            // now recurse any object collection of the type.
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                if (property.PropertyType.FullName == "System.Xml.Schema.XmlSchemaObjectCollection")
                {
                    XmlSchemaObjectCollection childObjectCollection = (XmlSchemaObjectCollection)property.GetValue(obj, null);
                    foreach (XmlSchemaObject schemaObject in childObjectCollection)
                    {
                        DecodeSchema2(schemaObject, newNode);
                    }
                }
            }
            return newNode;
        }
    }
}
