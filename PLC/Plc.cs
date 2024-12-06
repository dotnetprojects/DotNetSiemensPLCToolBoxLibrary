using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLC
{
    public class Plc
    {
        public Plc() { }

        public Plc(
            string id,
            List<PlcSubnet> plcNetwork,
            string address,
            string type,
            string rack,
            string slot,
            string partNumber,
            string firmwareVersion,
            bool status
        )
        {
            this.Id = id;
            this.Type = type;
            this.Address = address;
            this.PlcNetwork = plcNetwork;
            this.Rack = rack;
            this.Slot = slot;
            this.PartNumber = partNumber;
            this.FirmwareVersion = firmwareVersion;
            this.Status = status;
        }

        public string Id { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string PartNumber { get; set; }
        public string FirmwareVersion { get; set; }
        public string Rack { get; set; }
        public string Slot { get; set; }
        public bool Status { get; set; }
        public List<PlcSubnet> PlcNetwork { get; set; }
        public List<PlcSignal> Signals { get; set; }
        public List<PlcMetadata> Metadatas { get; set; }
        public List<PlcEvent> Events { get; set; }
    }
}
