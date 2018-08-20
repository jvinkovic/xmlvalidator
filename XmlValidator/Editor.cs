using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlValidator
{
    public class Editor
    {
        public string XmlName = "Untitled";

        private string _xmlPath;
        private string _xsdPath;
        private bool _fileChanged;

        /// <summary>
        /// opens XML file and returns file name
        /// </summary>
        /// <param name="path">what XML to open</param>
        /// <returns>filename if succesfull, otherwise null</returns>
        public string OpenXML(string path)
        {
            // TOOD
            return null;
        }

        /// <summary>
        /// opens XSD file and returns file name
        /// </summary>
        /// <param name="path">what XSD to open</param>
        /// <returns>filename if succesfull, otherwise null</returns>
        public string OpenXSD(string path)
        {
            // TOOD
            return null;
        }

        public bool SaveXML(string xml)
        {
            throw new NotImplementedException();
        }

        public bool SaveXMLAs(string xml)
        {
            throw new NotImplementedException();
        }

        public bool CheckXML(string xml)
        {
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
