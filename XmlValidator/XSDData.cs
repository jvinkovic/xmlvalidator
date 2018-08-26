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
        public readonly XSDTreeNode RootNode = new XSDTreeNode("root");

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
                RootNode = new XSDTreeNode("root");

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

            // If the current node is the root node of the tree, then we are
            // possibly adding a global attribute, element, simple type, or complex type.
            if (node == RootNode)
            {
                if (attrib != null)
                {
                    if (attrib.Name != null)
                    {
                        // add to global list
                        //cbGlobalTypes.Items.Add(new GlobalElementType(attrib.Name, attrib));
                    }
                }
                else if (element != null)
                {
                    if (element.Name != null)
                    {
                        // add to global list
                        //cbGlobalTypes.Items.Add(new GlobalElementType(element.Name, element));
                    }
                }
                else if (simpleType != null)
                {
                    if (simpleType.Name != null)
                    {
                        // add to global list
                        //cbGlobalTypes.Items.Add(new GlobalElementType(simpleType.Name, simpleType));
                    }
                }
                else if (complexType != null)
                {
                    if (complexType.Name != null)
                    {
                        // add to global list
                        // cbGlobalTypes.Items.Add(new GlobalElementType(complexType.Name, complexType));
                    }
                }
            }

            // if annotation, add a tree node and recurse for documentation and app info
            if (annot != null)
            {
                newNode = new XSDTreeNode("--annotation--");
                node.Add(newNode);
                foreach (XmlSchemaObject schemaObject in annot.Items)
                {
                    DecodeSchema2(schemaObject, newNode);
                }
            }
            else
                // if attribute, add an attribute at tree node
                if (attrib != null)
            {
                node.AddAttribute(attrib.QualifiedName.Name, attrib.SchemaTypeName.Name);
            }
            else
                    // if facet, add a tree node
                    if (facet != null)
            {
                newNode = new XSDTreeNode(facet.ToString());
                node.Add(newNode);
            }
            else
                        // if documentation, add a tree node
                        if (doc != null)
            {
                newNode = new XSDTreeNode("--documentation--");
                node.Add(newNode);
            }
            else
                            // if app info, add a tree node
                            if (appInfo != null)
            {
                newNode = new XSDTreeNode("--app info--");
                node.Add(newNode);
            }
            else
                                // if an element, determine whether the element is a simple type or a complex type
                                if (element != null)
            {
                XmlSchemaSimpleType st = element.SchemaType as XmlSchemaSimpleType;
                XmlSchemaComplexType ct = element.SchemaType as XmlSchemaComplexType;

                if (st != null)
                {
                    // this is a simple type element.  Recurse.
                    XSDTreeNode node2 = DecodeSchema2(st, newNode);
                    node2.Name = element.Name;
                }
                else if (ct != null)
                {
                    // this is a complex type element.  Recurse.
                    XSDTreeNode node2 = DecodeSchema2(ct, newNode);
                    node2.Name = element.Name;
                }
                else
                {
                    // This is a plain ol' fashioned element.
                    newNode = new XSDTreeNode(element.QualifiedName.Name);
                    node.Add(newNode);
                }
            }
            else
                                    // if a simple type, then add a tree node and recurse facets
                                    if (simpleType != null)
            {
                newNode = new XSDTreeNode(simpleType.QualifiedName.Name);
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
            else
                                        // if a complex type, add a tree node and recurse its sequence
                                        if (complexType != null)
            {
                newNode = new XSDTreeNode(complexType.Name);
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
