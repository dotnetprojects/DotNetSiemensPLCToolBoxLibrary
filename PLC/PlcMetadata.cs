using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLC
{
    public class PlcMetadata
    {
        public PlcMetadata() { }

        public PlcMetadata(string name, string description, string path, string metadata)
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
