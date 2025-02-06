using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.PLC
{
    public class SiemensPlcSignal
    {
        public SiemensPlcSignal() { }

        public string Address { get; set; }
        public string AbsoluteAddress { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }

        // TODO: List from descriptions for s7 projects.
        //public dynamic DescriptionList { get; set; }
        public string Datatype { get; set; }

        public string DefaultValue { get; set; }
        public string ProjectName { get; set; }
        public string Station { get; set; }
        public string Offset { get; set; }
        public string DBNumber { get; set; }

        private bool Readable = true;

        public bool isReadable
        {
            get { return Readable; }
            set { Readable = value; }
        }
    }
}
