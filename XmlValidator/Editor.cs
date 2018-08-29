using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XmlValidator
{
    public class Editor
    {
        public NodeStatus CurrentNodeStatus { get; private set; } = NodeStatus.Closed;
        public XSDTreeNode CurrentNode;

        public const string ATTR_SEPARATOR = " : ";
        public const string ATTR_TAG = "attr";

        public string XmlName = "Untitled";

        private string _xmlPath;
        private XSDData _xsd;
        private bool _fileChanged;

        /// <summary>
        /// opens XML file and returns file content
        /// </summary>
        /// <param name="path">what XML to open</param>
        /// <returns>file content if succesfull, otherwise null</returns>
        public string OpenXML(string path)
        {
            XmlName = Path.GetFileName(path);
            _xmlPath = path;
            return File.ReadAllText(path);
        }

        /// <summary>
        /// opens XSD file and returns file name
        /// </summary>
        /// <param name="path">what XSD to open</param>
        /// <returns>filename if succesfull, otherwise null</returns>
        public string OpenXSD(string path)
        {
            _xsd = new XSDData(path);
            CurrentNode = _xsd.RootNode;

            return _xsd.Name;
        }

        public XSDData GetXSD()
        {
            return _xsd;
        }

        public void SetXSD(XSDData xsd)
        {
            _xsd = xsd;
        }

        public bool SaveXML(string xml)
        {
            try
            {
                File.WriteAllText(_xmlPath ?? XmlName + ".xml", xml, Encoding.UTF8);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void ResetCurrentNode()
        {
            CurrentNodeStatus = NodeStatus.Closed;
            CurrentNode = _xsd.RootNode;
        }

        public HashSet<string> CurrentNodeElements()
        {
            if (null == CurrentNode)
            {
                return null;
            }

            var elements = new HashSet<string>();

            foreach (var cne in CurrentNode.Elements.Where(e => e.Value.Element == true))
            {
                elements.Add(cne.Key);
            }

            if (elements.Count == 0)
            {
                var complexElems = _xsd.RootNode.Elements.Where(e => e.Value.ComplexType == true);

                var simpleElems = CurrentNode.Elements.Where(e => e.Value.Element == true || e.Value.ComplexType == false);
                foreach (var el in simpleElems)
                {
                    elements.Add(el.Key);
                }

                var type = CurrentNode.Type?.Substring(CurrentNode.Type.LastIndexOf(":") + 1);
                var complexTypes = complexElems.Where(ce => ce.Key == type);

                if (complexTypes.Count() > 0)
                {
                    foreach (var cel in complexTypes.First().Value.Elements)
                    {
                        elements.Add(cel.Key);
                    }
                }
            }

            return elements;
        }

        public HashSet<string> CurrentNodeAttributes()
        {
            var attributes = new HashSet<string>();

            foreach (var at in CurrentNode.Attributes)
            {
                attributes.Add(at.Key + ATTR_SEPARATOR + at.Value);
            }

            foreach (var cat in CurrentNode.Attributes)
            {
                attributes.Add(cat.Key + ATTR_SEPARATOR + cat.Value);
            }

            if (attributes.Count == 0)
            {
                var complexElems = _xsd.RootNode.Elements.Where(e => e.Value.ComplexType == true);

                var type = CurrentNode.Type?.Substring(CurrentNode.Type.LastIndexOf(":") + 1);
                var complexTypes = complexElems.Where(ce => ce.Key == type);

                if (complexTypes.Count() > 0)
                {
                    foreach (var at in complexTypes.First().Value.Attributes)
                    {
                        attributes.Add(at.Key + ATTR_SEPARATOR + at.Value);
                    }
                }
            }

            return attributes;
        }

        public void CurrentNodeOpening()
        {
            CurrentNodeStatus = NodeStatus.Opening;
        }

        public void CurrentNodeOpen(string nodeName)
        {
            CurrentNodeStatus = NodeStatus.Open;

            XSDTreeNode node = null;
            CurrentNode?.Elements.TryGetValue(nodeName, out node);
            CurrentNode = node ?? CurrentNode;
        }

        public void CurrentNodeClosing()
        {
            CurrentNodeStatus = NodeStatus.Closing;
        }

        public void CurrentNodeClose()
        {
            CurrentNodeStatus = NodeStatus.Closed;
            CurrentNode = CurrentNode != _xsd.RootNode ? CurrentNode.Parent ?? CurrentNode : CurrentNode;
        }

        public void CurrentNodeEnd(string nodeName)
        {
            CurrentNodeStatus = NodeStatus.Inside;

            XSDTreeNode node = null;
            CurrentNode?.Elements.TryGetValue(nodeName, out node);
            CurrentNode = node ?? GetFromComplex(CurrentNode, nodeName) ?? CurrentNode;
        }

        private XSDTreeNode GetFromComplex(XSDTreeNode startNode, string nodeName)
        {
            XSDTreeNode node = null;

            var complexElems = _xsd.RootNode.Elements.Where(e => e.Value.ComplexType == true);

            var type = CurrentNode.Type?.Substring(CurrentNode.Type.LastIndexOf(":") + 1);
            var complexTypes = complexElems.Where(ce => ce.Key == type);

            if (complexTypes.Count() > 0)
            {
                complexTypes?.First().Value?.Elements?.TryGetValue(nodeName, out node);
            }

            node?.UpdateParent(startNode);

            return node;
        }

        public bool SaveXMLAs(string xml)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.AddExtension = true;
                sfd.CheckPathExists = true;
                sfd.DefaultExt = "xlm";
                sfd.OverwritePrompt = true;
                sfd.Title = "Save XML";

                DialogResult result = sfd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    return true;
                }

                return false;
            }
        }

        public ValidationData CheckXML(string xml, bool omitNamespace)
        {
            var validator = new ValidateWithXSD(ref xml, _xsd);

            var result = validator.Validate(omitNamespace);
            return result;
        }

        public void FileChanged()
        {
            _fileChanged = true;
        }

        public bool IsFileChanged()
        {
            return _fileChanged;
        }
    }
}
