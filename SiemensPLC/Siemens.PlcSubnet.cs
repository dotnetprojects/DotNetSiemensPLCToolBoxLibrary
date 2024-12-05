using System;
using System.Collections.Generic;
using NLog;

namespace Siemens.PLC
{
    public class SiemensPlcSubnet
    {
        public SiemensPlcSubnet() { }

        public SiemensPlcSubnet(string CommInterface, List<SiemensPlcNode> plcNodes)
        {
            this.Interface = CommInterface;
            this.PlcNodes = plcNodes;
        }

        public string Interface { get; set; }
        public List<SiemensPlcNode> PlcNodes { get; set; }
    }

    public class SiemensPlcNode
    {
        public SiemensPlcNode() { }

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SiemensPlcNode(string id, string name, string subnet, string type, string address)
        {
            this.Id = id;
            this.Name = name;
            this.Subnet = subnet;
            this.Type = type;
            this.Address = address;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Subnet { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }

        public static void PrintNodeData(SiemensPlcNode node)
        {
            logger.Info(
                "Node: "
                    + node.Id
                    + ":"
                    + node.Name
                    + " - "
                    + "Subnet:"
                    + node.Subnet
                    + " - "
                    + node.Type
                    + ":"
                    + node.Address
            );
        }
    }
}
