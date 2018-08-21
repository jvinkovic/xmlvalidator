using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlValidator
{
    public class XSDData
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        public XSDData(string path)
        {
            // TODO open and read XSD
        }
    }
}
