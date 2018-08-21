using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlValidator
{
    public class Editor
    {
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
            // TODO
            throw new NotImplementedException();
        }

        public bool SaveXMLAs(string xml)
        {
            // TODO
            throw new NotImplementedException();
        }

        public bool CheckXML(string xml)
        {
            // TODO
            throw new NotImplementedException();
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
