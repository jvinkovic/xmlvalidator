using System.Collections;
using System.Collections.Generic;

namespace xxxTest
{
    internal class XSDTreeNode : IEnumerable<XSDTreeNode>
    {
        public readonly Dictionary<string, XSDTreeNode> Elements = new Dictionary<string, XSDTreeNode>();
        public readonly Dictionary<string, string> Attributes = new Dictionary<string, string>();

        public string Name;
        public XSDTreeNode Parent { get; private set; }

        public XSDTreeNode(string id)
        {
            this.Name = id;
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
