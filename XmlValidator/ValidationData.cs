using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlValidator
{
    public class ValidationData
    {
        public bool Valid { get; set; } = true;
        public List<string> Errors { get; set; } = new List<string>();
    }
}
