using System.IO;
using System.Text;

namespace XmlValidator
{
    public class XSDData
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        public XSDData(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);
            Content = File.ReadAllText(path, Encoding.UTF8);
        }
    }
}
