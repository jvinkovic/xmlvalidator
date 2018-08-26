using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlValidator
{
    public enum NodeStatus
    {
        Opening,
        Open,
        Inside,
        Closing,
        Closed
    }
}
