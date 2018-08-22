using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public async Task<ValidationData> CheckXML(string xml)
        {
            var validator = new ValidateWithXSD(ref xml, _xsd);

            var result = await validator.Validate();
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
