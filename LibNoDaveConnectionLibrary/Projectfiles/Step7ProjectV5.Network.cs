using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Network;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using Network = DotNetSiemensPLCToolBoxLibrary.DataTypes.Network.Network;
using NetworkInterface = DotNetSiemensPLCToolBoxLibrary.DataTypes.Network.NetworkInterface;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public partial class Step7ProjectV5
    {
        private Step7NetworkConfiguration _networkConfiguration;
        private readonly Dictionary<Step7ObjectReference, CPUFolder> _networkCpus = new Dictionary<Step7ObjectReference, CPUFolder>();
        public Step7NetworkConfiguration NetworkConfiguration
        {
            get
            {
                if (!_projectLoaded) LoadProject();
                return _networkConfiguration;
            }
        }

        private sealed class NetObject
        {
            public int Start, End, LinkOffset;
            public Step7ObjectReference Reference;
        }

        private sealed class NetRelation
        {
            public Step7ObjectReference Target;
            public int Id;
        }

        private string NetworkPath(string relative) => ProjectFolder + relative.Replace('/', _DirSeperator);

        private byte[] ReadNetworkFile(string relative)
        {
            using (var stream = _ziphelper.GetReadStream(NetworkPath(relative)))
            {
                if (stream == null) return new byte[0];
                using (var buffer = new MemoryStream())
                {
                    stream.CopyTo(buffer);
                    return buffer.ToArray();
                }
            }
        }

        private void ReadNetworkPart(string source, Action read)
        {
            try { read(); }
            catch (Exception ex) when (ex is IOException || ex is InvalidDataException || ex is ArgumentException || ex is FormatException || ex is OverflowException)
            {
                _networkConfiguration.Diagnostics.Add(source + ": " + ex.Message);
            }
        }

        private static int NetInt(byte[] bytes, int offset) => BitConverter.ToInt32(bytes, offset);

        private static Step7ObjectReference NetReference(byte[] bytes, int offset) => new Step7ObjectReference(
            NetInt(bytes, offset), NetInt(bytes, offset + 4), NetInt(bytes, offset + 8), NetInt(bytes, offset + 12));

        private static List<NetRelation> NetRelations(byte[] bytes, int offset)
        {
            var result = new List<NetRelation>();
            if (offset < 512 || offset > bytes.Length - 16) return result;
            int count = NetInt(bytes, offset + 8);
            if (count < 0 || count > (bytes.Length - offset - 16) / 24)
                throw new InvalidDataException("Truncated relation list.");
            for (int i = 0; i < count; i++)
            {
                int at = offset + 16 + i * 24;
                result.Add(new NetRelation { Target = NetReference(bytes, at), Id = NetInt(bytes, at + 16) });
            }
            return result;
        }

        // Only accept NetPro object headers, not an incidental byte pattern inside an attribute value.
        private static List<NetObject> NetObjects(byte[] bytes)
        {
            var result = new List<NetObject>();
            for (int i = 0; i <= bytes.Length - 80; i++)
            {
                if (bytes[i] < 1 || bytes[i] > 3 || (bytes[i + 1] != 0x51 && bytes[i + 1] != 0x52) ||
                    bytes[i + 2] != 0x14 || bytes[i + 3] != 0 || NetInt(bytes, i + 8) != 0 ||
                    NetInt(bytes, i + 12) != 0 || NetInt(bytes, i + 16) != 0x112001) continue;
                if (result.Count > 0) result[result.Count - 1].End = i;
                result.Add(new NetObject { Start = i, End = bytes.Length, LinkOffset = NetInt(bytes, i + 28),
                    Reference = new Step7ObjectReference(0, 0, NetInt(bytes, i), NetInt(bytes, i + 4)) });
                i += 79;
            }
            return result;
        }

        // Attribute IDs occur twice, followed by a Pascal label and ten metadata bytes.
        // Searches are bounded to this object, so absent attributes cannot leak in from the next member.
        private static int NetAttribute(byte[] bytes, NetObject obj, int id)
        {
            for (int i = obj.Start + 78; i <= obj.End - 20; i++)
            {
                if (NetInt(bytes, i) != id || NetInt(bytes, i + 4) != id) continue;
                int value = i + 19 + bytes[i + 8];
                if (value < obj.End) return value;
            }
            return -1;
        }

        private string NetString(byte[] bytes, NetObject obj, int id)
        {
            int at = NetAttribute(bytes, obj, id);
            if (at < 0 || bytes[at] > obj.End - at - 1) return null;
            return ProjectEncoding.GetString(bytes, at + 1, bytes[at]);
        }

        private static bool NetBool(byte[] bytes, NetObject obj, int id)
        {
            int at = NetAttribute(bytes, obj, id);
            return at >= 0 && bytes[at] != 0;
        }

        private IPAddress NetIp(byte[] bytes, NetObject obj, int id)
        {
            string value = NetString(bytes, obj, id);
            if (value == null || value.Length != 8) return null;
            var address = new byte[4];
            for (int i = 0; i < 4; i++)
                if (!byte.TryParse(value.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out address[i])) return null;
            return new IPAddress(address);
        }

        private void LoadNetworkConfiguration(List<CPFolder> dpFolders, List<StationConfigurationFolder> stations)
        {
            _networkConfiguration = new Step7NetworkConfiguration();
            _networkCpus.Clear();
            foreach (string family in new[] { "s7hk31ax", "s7hk41ax", "s7hkcomx" })
                ReadNetworkPart("Hardware " + family, () =>
                {
                    foreach (var row in NetworkRows("hOmSave7/" + family + "/HOBJECT1.DBF"))
                    {
                        var cpu = CPUFolders.FirstOrDefault(c => c.ID == NetNumber(row, "ID") &&
                            (family == "s7hk41ax" ? c.CpuType == DataTypes.PLCType.Simatic400 || c.CpuType == DataTypes.PLCType.Simatic400H :
                             family == "s7hkcomx" ? c.CpuType == DataTypes.PLCType.SimaticET200 :
                             c.CpuType == DataTypes.PLCType.Simatic300 || c.CpuType == DataTypes.PLCType.SimaticRTX));
                        if (cpu != null) _networkCpus[new Step7ObjectReference(NetNumber(row, "UNITTYP"), NetNumber(row, "UNITID"),
                            NetNumber(row, "OBJTYP"), NetNumber(row, "ID"))] = cpu;
                    }
                });
            var layouts = new Dictionary<Step7ObjectReference, NetworkLayout>();
            ReadNetworkPart("NetPro layout", () =>
            {
                byte[] bytes = ReadNetworkFile("Global/s7netpro.npa");
                if (bytes.Length == 0) bytes = ReadNetworkFile("s7netpro.npa");
                if (bytes.Length == 0) return;
                if (bytes.Length < 8 || Encoding.ASCII.GetString(bytes, 0, 4) != "SPAK" || (bytes.Length - 8) % 40 != 0)
                    throw new InvalidDataException("Unsupported NetPro layout header or record size.");
                for (int i = 8; i <= bytes.Length - 40; i += 40)
                {
                    var reference = new Step7ObjectReference(NetInt(bytes, i + 8), NetInt(bytes, i + 12), NetInt(bytes, i), NetInt(bytes, i + 4));
                    layouts[reference] = new NetworkLayout { Reference = reference,
                        Position = NetPosition(bytes, i + 24), AutomaticPosition = NetPosition(bytes, i + 32) };
                }
            });

            foreach (var station in stations)
            {
                var reference = new Step7ObjectReference(0x112001, station.UnitID, station.ObjTyp, station.ID);
                var node = new NetworkNode { Reference = reference, Station = station, Name = station.Name };
                layouts.TryGetValue(reference, out var layout);
                node.Layout = layout;
                node.Plcs.AddRange(CPUFolders.Where(c => ReferenceEquals(c.Parent, station)));
                _networkConfiguration.Nodes.Add(node);
            }
            // Preserve other/PC stations present in the layout even when no PLC folder exists.
            foreach (var layout in layouts.Values.Where(l => l.Reference.UnitType != 0 &&
                !_networkConfiguration.Nodes.Any(n => n.Reference.Equals(l.Reference))))
                _networkConfiguration.Nodes.Add(new NetworkNode { Reference = layout.Reference, Layout = layout });

            foreach (string family in new[] { "s7hstatx", "s7hslntx", "s7hdevnx" })
                ReadNetworkPart("Network elements " + family, () =>
                {
                    foreach (var row in NetworkRows("hOmSave7/" + family + "/HOBJECT1.DBF"))
                    {
                        var reference = new Step7ObjectReference(NetNumber(row, "UNITTYP"), NetNumber(row, "UNITID"),
                            NetNumber(row, "OBJTYP"), NetNumber(row, "ID"));
                        var node = _networkConfiguration.Nodes.FirstOrDefault(n => n.Reference.Equals(reference));
                        if (node != null) node.Name = NetText(row, "NAME");
                    }
                });

            byte[] tab = ReadNetworkFile("S7Netze/S7NONFGX.tab");
            byte[] links = ReadNetworkFile("S7Netze/S7NONFGX.lnk");
            var objects = NetObjects(tab);
            foreach (var obj in objects.Where(o => (o.Reference.ObjectType & 0xff00) == 0x5100))
            {
                var network = new Network { Reference = obj.Reference, Name = NetString(tab, obj, 0x1001),
                    NetworkType = NetType(obj.Reference.ObjectType), SubnetId = NetString(tab, obj, 0xa008) };
                layouts.TryGetValue(obj.Reference, out var layout);
                network.Layout = layout;
                _networkConfiguration.Networks.Add(network);
            }
            foreach (var obj in objects.Where(o => (o.Reference.ObjectType & 0xff00) == 0x5200))
                ReadNetworkPart("NetPro member " + obj.Reference, () =>
                {
                    NetworkInterface member;
                    if (NetType(obj.Reference.ObjectType) == NetworkType.Ethernet)
                    {
                        var ethernet = new EthernetNetworkInterface { IpAddress = NetIp(tab, obj, 0xfe0),
                            SubnetMask = NetIp(tab, obj, 0xfe5), Router = NetIp(tab, obj, 0xfe3),
                            UseIp = NetBool(tab, obj, 0xfea), UseIso = NetBool(tab, obj, 0xfed), UseRouter = NetBool(tab, obj, 0xfe8) };
                        string mac = NetString(tab, obj, 0xfa2);
                        if (mac != null && mac.Length == 12 && mac.All(Uri.IsHexDigit)) ethernet.Mac = PhysicalAddress.Parse(mac);
                        member = ethernet;
                    }
                    else
                    {
                        int at = NetAttribute(tab, obj, NetType(obj.Reference.ObjectType) == NetworkType.Mpi ? 0x89a : 0x836);
                        member = new MpiProfiBusNetworkInterface { NetworkInterfaceType = NetType(obj.Reference.ObjectType),
                            Address = at >= 0 ? tab[at] : -1 };
                    }
                    member.Reference = obj.Reference;
                    member.Name = NetString(tab, obj, 0x1001);
                    var relations = NetRelations(links, obj.LinkOffset);
                    var subnet = relations.FirstOrDefault(r => r.Id == 9);
                    if (subnet != null)
                        member.Network = _networkConfiguration.Networks.FirstOrDefault(n => n.Reference.Equals(subnet.Target));
                    member.Network?.Members.Add(member);
                    var station = relations.FirstOrDefault(r => r.Id == 0xb);
                    member.StationReference = station?.Target;
                    if (station != null) member.Node = _networkConfiguration.Nodes.FirstOrDefault(n => n.Reference.Equals(station.Target));
                    member.HardwareReference = relations.FirstOrDefault(r => r.Id == 0x41)?.Target;
                    var hardware = member.HardwareReference;
                    var element = hardware == null ? null : _networkConfiguration.Nodes.FirstOrDefault(n =>
                        n.Reference.Equals(hardware) || (n.Reference.ObjectType == hardware.UnitType && n.Reference.ObjectId == hardware.UnitId));
                    if (element != null)
                    {
                        element.Station = member.Node?.Station;
                        member.Node = element;
                    }
                    member.Plc = ResolveNetworkCpu(member.HardwareReference);
                    if (member is EthernetNetworkInterface)
                        member.CommunicationProcessor = CPFolders.FirstOrDefault(c => c.TobjId != null && c.TobjId.Contains(obj.Reference.ObjectId));
                    // Older hardware uses the DP master relation chain instead of a direct module reference.
                    if (member.Plc == null && member.CommunicationProcessor == null && member.HardwareReference == null)
                    {
                        var dp = dpFolders.FirstOrDefault(d => d.TobjId != null && d.TobjId.Contains(obj.Reference.ObjectId));
                        if (dp != null) member.Plc = CPUFolders.FirstOrDefault(c => c.UnitID == dp.UnitID &&
                            (member.Node == null || ReferenceEquals(c.Parent, member.Node.Station)));
                    }
                    if (member.Plc == null && member.CommunicationProcessor == null && member.HardwareReference == null && member is EthernetNetworkInterface)
                        member.Plc = CPUFolders.FirstOrDefault(c => c.TobjId == obj.Reference.ObjectId);
                    member.Node?.Interfaces.Add(member);
                    if (member.CommunicationProcessor != null)
                    {
                        var cp = member.CommunicationProcessor;
                        if (cp.NetworkInterfaces == null) cp.NetworkInterfaces = new List<NetworkInterface>();
                        cp.NetworkInterfaces.Add(member);
                        // A CP in a multi-CPU station cannot safely be attributed to an arbitrary CPU.
                        if (member.Plc == null && member.Node?.Plcs.Count == 1) member.Plc = member.Node.Plcs[0];
                    }
                    else if (member.Plc != null)
                    {
                        if (member.Plc.NetworkInterfaces == null) member.Plc.NetworkInterfaces = new List<NetworkInterface>();
                        member.Plc.NetworkInterfaces.Add(member);
                    }
                    _networkConfiguration.Interfaces.Add(member);
                });

            LoadNetworkConnections();
        }

        private static NetworkType NetType(int objectType) => (objectType & 0xff) == 1 ? NetworkType.Mpi :
            (objectType & 0xff) == 2 ? NetworkType.Profibus : NetworkType.Ethernet;

        private static NetworkPosition NetPosition(byte[] bytes, int offset) => NetInt(bytes, offset) == -1 && NetInt(bytes, offset + 4) == -1 ?
            null : new NetworkPosition { X = NetInt(bytes, offset), Y = NetInt(bytes, offset + 4) };

        private CPUFolder ResolveNetworkCpu(Step7ObjectReference reference)
        {
            if (reference == null) return null;
            if (_networkCpus.TryGetValue(reference, out var cpu)) return cpu;
            // An integrated interface belongs to the CPU object used as its owning unit.
            var matches = _networkCpus.Where(c => c.Key.ObjectType == reference.UnitType && c.Key.ObjectId == reference.UnitId)
                .Select(c => c.Value).Distinct().ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        private IEnumerable<DataRow> NetworkRows(string relative)
        {
            var table = DBF.ParseDBF.ReadDBF(NetworkPath(relative), _ziphelper, _DirSeperator, Encoding.GetEncoding("ISO-8859-1"));
            return table.Rows.Cast<DataRow>().Where(r => _showDeleted || !(bool)r["DELETED_FLAG"]);
        }

        private static int NetNumber(DataRow row, string column) => row.Table.Columns.Contains(column) && row[column] != DBNull.Value ? Convert.ToInt32(row[column]) : 0;
        private string NetText(DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column)) return null;
            var text = Convert.ToString(row[column]).Split('\0')[0].TrimEnd();
            return ProjectEncoding.GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(text));
        }

        private void LoadNetworkConnections()
        {
            byte[] containerLinks = ReadNetworkFile("CONN/LINKVL.dbf");
            ReadNetworkPart("Connection tables", () =>
            {
                foreach (var container in NetworkRows("CONN/VERBTAB.DBF"))
                {
                    int id = NetNumber(container, "ID");
                    var owner = NetRelations(containerLinks, NetNumber(container, "LINKID")).FirstOrDefault(r => r.Id == 0x35)?.Target;
                    ReadConnectionTable("CONN/CO_" + id + "/HOMOVERB.DBF", "CONN/CO_" + id + "/HOMOLINK.dbf", id, owner);
                }
            });
            ReadConnectionTable("CONN/S7wntcps/IPVERB.DBF", "CONN/S7wntcps/iplink.dbf", 0, null);
            foreach (var connection in _networkConfiguration.Connections)
            {
                var partner = _networkConfiguration.Connections.FirstOrDefault(c => c.Reference.Equals(connection.PartnerConnection));
                if (connection.RemotePlc == null) connection.RemotePlc = partner?.LocalPlc;
                connection.LocalInterface = connection.Interfaces.FirstOrDefault(i => i.Plc != null && ReferenceEquals(i.Plc, connection.LocalPlc));
                connection.RemoteInterface = connection.Interfaces.FirstOrDefault(i => !ReferenceEquals(i, connection.LocalInterface) &&
                    i.Plc != null && ReferenceEquals(i.Plc, connection.RemotePlc));
                if (connection.LocalInterface != null) connection.Transport = connection.LocalInterface.NetworkInterfaceType;
            }
        }

        private void ReadConnectionTable(string table, string linkFile, int containerId, Step7ObjectReference owner)
        {
            ReadNetworkPart(table, () =>
            {
                byte[] links = ReadNetworkFile(linkFile);
                foreach (var row in NetworkRows(table))
                {
                    var connection = new NetworkConnection { SourceFile = table, Name = NetText(row, "NAME"),
                        Reference = new Step7ObjectReference(0x145301, containerId == 0 ? NetNumber(row, "UNITID") : containerId,
                            NetNumber(row, "TYPE"), NetNumber(row, "ID")),
                        RawType = NetNumber(row, "TYPE"), LocalId = NetNumber(row, "LOCALID"),
                        IsDeleted = (bool)row["DELETED_FLAG"], IsUnspecified = NetNumber(row, "UNSPEZ") != 0,
                        ActiveEstablishment = row.Table.Columns.Contains("AUFBAU") ? (bool?)(NetNumber(row, "AUFBAU") != 0) : null,
                        LocalHardware = owner, LocalPlc = ResolveNetworkCpu(owner), Comment = NetText(row, "COMMENT") };
                    connection.ConnectionType = connection.RawType == 0x145401 ? NetworkConnectionType.S7 :
                        connection.RawType == 0x145442 ? NetworkConnectionType.Tcp : NetworkConnectionType.Unknown;
                    foreach (DataColumn column in row.Table.Columns) connection.Attributes[column.ColumnName] = row[column];
                    connection.LocalTsap = NetBinary(row, "LOCTSAP", "LOCTSAP_L");
                    connection.RemoteTsap = NetBinary(row, "REMTSAP", "REMTSAP_L");
                    var address = NetBinary(row, "NETADDR", "NETADDR_L");
                    if (address.Length == 4) connection.RemoteAddress = new IPAddress(address);
                    if (connection.ConnectionType == NetworkConnectionType.Tcp)
                    {
                        if (connection.LocalTsap.Length == 2) connection.LocalPort = connection.LocalTsap[0] * 256 + connection.LocalTsap[1];
                        if (connection.RemoteTsap.Length == 2) connection.RemotePort = connection.RemoteTsap[0] * 256 + connection.RemoteTsap[1];
                    }
                    foreach (var relation in NetRelations(links, NetNumber(row, "LINKID")))
                    {
                        if (relation.Id == 0x3a) { connection.RemoteHardware = relation.Target; connection.RemotePlc = ResolveNetworkCpu(relation.Target); }
                        if (relation.Id == 0x3e) connection.PartnerConnection = relation.Target;
                        if (relation.Id == 0x6c)
                        {
                            var member = _networkConfiguration.Interfaces.FirstOrDefault(m => m.Reference.Equals(relation.Target));
                            if (member != null && !connection.Interfaces.Contains(member)) connection.Interfaces.Add(member);
                        }
                    }
                    // For TCP the local module is recorded in the linked interface, not VERBTAB.
                    if (owner == null && connection.ConnectionType == NetworkConnectionType.Tcp && connection.Interfaces.Count == 1)
                    {
                        connection.LocalPlc = connection.Interfaces[0].Plc;
                        connection.LocalHardware = connection.Interfaces[0].HardwareReference;
                    }
                    _networkConfiguration.Connections.Add(connection);
                }
            });
        }

        private static byte[] NetBinary(DataRow row, string column, string lengthColumn)
        {
            if (!row.Table.Columns.Contains(column)) return new byte[0];
            var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(Convert.ToString(row[column]));
            int length = NetNumber(row, lengthColumn);
            return bytes.Take(Math.Max(0, Math.Min(length, bytes.Length))).ToArray();
        }
    }
}
