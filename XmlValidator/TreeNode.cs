using System.Collections;
using System.Collections.Generic;

namespace XmlValidator
{
    public class XSDTreeNode : IEnumerable<XSDTreeNode>
    {
        public readonly Dictionary<string, XSDTreeNode> Elements = new Dictionary<string, XSDTreeNode>();
        public readonly Dictionary<string, string> Attributes = new Dictionary<string, string>();

        public string Name { get; set; }
        public bool Element { get; set; } = false;
        public string Type { get; set; }
        public bool ComplexType { get; set; }
        public XSDTreeNode Parent { get; private set; }

        public XSDTreeNode(string name, string type, bool complexType)
        {
            this.Name = name;
            this.Type = type;
            this.ComplexType = complexType;
        }

        public void UpdateParent(XSDTreeNode node)
        {
            this.Parent = node;
        }

        public XSDTreeNode GetElement(string id)
        {
            return this.Elements[id];
        }

        public void Add(XSDTreeNode item)
        {
            if (item.Parent != null)
            {
                item.Parent.Elements.Remove(item.Name);
            }

            item.Parent = this;
            this.Elements.Add(item.Name, item);
        }

        public void Remove(XSDTreeNode item)
        {
            item.Parent = null;
            this.Elements.Remove(item.Name);
        }

        public string GetAttributeType(string name)
        {
            return this.Attributes[name];
        }

        public void AddAttribute(string name, string type)
        {
            this.Attributes.Add(name, type);
        }

        public IEnumerator<XSDTreeNode> GetEnumerator()
        {
            return this.Elements.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int ElementsCount
        {
            get { return this.Elements.Count; }
        }

        public int AttributesCount
        {
            get { return this.Attributes.Count; }
        }
    }
}
