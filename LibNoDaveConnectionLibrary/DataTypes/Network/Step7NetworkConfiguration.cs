using System;
using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Network
{
    /// <summary>NetPro topology, saved layout and configured communication connections.</summary>
    public class Step7NetworkConfiguration
    {
        public List<Network> Networks { get; } = new List<Network>();
        public List<NetworkNode> Nodes { get; } = new List<NetworkNode>();
        public List<NetworkInterface> Interfaces { get; } = new List<NetworkInterface>();
        public List<NetworkConnection> Connections { get; } = new List<NetworkConnection>();
        public List<string> Diagnostics { get; } = new List<string>();
    }

    /// <summary>STEP 7 IDs are scoped by object type and owning unit, not globally unique.</summary>
    public sealed class Step7ObjectReference : IEquatable<Step7ObjectReference>
    {
        public int UnitType { get; }
        public int UnitId { get; }
        public int ObjectType { get; }
        public int ObjectId { get; }

        public Step7ObjectReference(int unitType, int unitId, int objectType, int objectId)
        {
            UnitType = unitType; UnitId = unitId; ObjectType = objectType; ObjectId = objectId;
        }

        public bool Equals(Step7ObjectReference other) => other != null && UnitType == other.UnitType &&
            UnitId == other.UnitId && ObjectType == other.ObjectType && ObjectId == other.ObjectId;
        public override bool Equals(object obj) => Equals(obj as Step7ObjectReference);
        public override int GetHashCode() => unchecked(((UnitType * 397 ^ UnitId) * 397 ^ ObjectType) * 397 ^ ObjectId);
        public override string ToString() => $"{UnitType:X}:{UnitId:X}/{ObjectType:X}:{ObjectId:X}";
    }

    public class NetworkPosition
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class NetworkLayout
    {
        public Step7ObjectReference Reference { get; set; }
        /// <summary>Saved manual position in native NetPro units; null when unset (-1,-1).</summary>
        public NetworkPosition Position { get; set; }
        public NetworkPosition AutomaticPosition { get; set; }
    }

    public class NetworkNode : ProjectItem
    {
        public Step7ObjectReference Reference { get; set; }
        public NetworkLayout Layout { get; set; }
        public StationConfigurationFolder Station { get; set; }
        public List<CPUFolder> Plcs { get; } = new List<CPUFolder>();
        public List<NetworkInterface> Interfaces { get; } = new List<NetworkInterface>();
    }

    public enum NetworkConnectionType { Unknown, S7, Tcp, IsoOnTcp, Iso, Udp, Mpi }

    public class NetworkConnection : ProjectItem
    {
        public Step7ObjectReference Reference { get; set; }
        public int LocalId { get; set; }
        public int RawType { get; set; }
        public NetworkConnectionType ConnectionType { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsUnspecified { get; set; }
        public bool? ActiveEstablishment { get; set; }
        public CPUFolder LocalPlc { get; set; }
        public CPUFolder RemotePlc { get; set; }
        public Step7ObjectReference LocalHardware { get; set; }
        public Step7ObjectReference RemoteHardware { get; set; }
        public Step7ObjectReference PartnerConnection { get; set; }
        public List<NetworkInterface> Interfaces { get; } = new List<NetworkInterface>();
        /// <summary>Original DBF columns, including binary TSAP/address fields decoded losslessly as Latin-1.</summary>
        public Dictionary<string, object> Attributes { get; } = new Dictionary<string, object>();
        public string SourceFile { get; set; }
        public NetworkInterface LocalInterface { get; set; }
        public NetworkInterface RemoteInterface { get; set; }
        public NetworkType? Transport { get; set; }
        public System.Net.IPAddress RemoteAddress { get; set; }
        public byte[] LocalTsap { get; set; }
        public byte[] RemoteTsap { get; set; }
        public int? LocalPort { get; set; }
        public int? RemotePort { get; set; }
    }
}
