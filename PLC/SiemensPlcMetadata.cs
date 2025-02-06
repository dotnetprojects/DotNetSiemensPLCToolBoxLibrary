using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.PLC
{
    public class SiemensPlcMetadata
    {
        public SiemensPlcMetadata() { }

        public SiemensPlcMetadata(string name, string description, string path, string metadata)
        {
            this.Name = name;
            this.Description = description;
            this.Path = path;
            this.Metadata = metadata;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string Metadata { get; set; }
    }
}
