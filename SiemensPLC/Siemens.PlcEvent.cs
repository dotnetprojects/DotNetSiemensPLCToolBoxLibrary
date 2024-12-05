using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.PLC
{
    public class SiemensPlcEvent
    {
        public SiemensPlcEvent() { }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public string Severity { get; set; }
    }
}
