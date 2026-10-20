using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Network;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;
using NUnit.Framework;

namespace ToolBoxLibUnitTests
{
    [TestFixture]
    public class TestStep7Networks
    {
        private string directory;

        [SetUp]
        public void SetUp()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            directory = Path.Combine(Path.GetTempPath(), "step7-network-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            Write("test.s7p", new byte[] { 0, 0, 0, 0, 4, (byte)'T', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0 });
        }

        [TearDown]
        public void TearDown() => Directory.Delete(directory, true);

        [TestCase(false)]
        [TestCase(true)]
        public void ReadsTopologyLayoutAddressesAndConnections(bool zipped)
        {
            CreateTopology();
            string path = Path.Combine(directory, "test.s7p");
            if (zipped)
            {
                path = Path.Combine(directory, "test.zip");
                using (var archive = ZipFile.Open(path, ZipArchiveMode.Create))
                    foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories).Where(f => f != path))
                        archive.CreateEntryFromFile(file, "project/" + Path.GetRelativePath(directory, file).Replace('\\', '/'));
            }
            using (var project = new Step7ProjectV5(path, false))
            {
                var net = project.NetworkConfiguration;
                Assert.That(net.Diagnostics, Is.Empty);
                Assert.That(net.Networks.Count, Is.EqualTo(2));
                Assert.That(net.Networks[0].Name, Is.EqualTo("Ethernet"));
                Assert.That(net.Networks[0].Layout.Position.X, Is.EqualTo(20));
                Assert.That(net.Networks[0].Layout.Position.Y, Is.EqualTo(40));
                Assert.That(net.Networks[1].Layout.Position, Is.Null);
                Assert.That(net.Networks[1].Layout.AutomaticPosition.Y, Is.EqualTo(88));
                Assert.That(net.Interfaces.Count, Is.EqualTo(4));
                var ethernet = (EthernetNetworkInterface)net.Interfaces[0];
                Assert.That(ethernet.Name, Is.EqualTo("Port A"));
                Assert.That(ethernet.IpAddress.ToString(), Is.EqualTo("192.0.2.10"));
                Assert.That(ethernet.SubnetMask.ToString(), Is.EqualTo("255.255.255.0"));
                Assert.That(ethernet.Network, Is.SameAs(net.Networks[0]));
                Assert.That(ethernet.Plc, Is.SameAs(project.CPUFolders.First()));
                Assert.That(ethernet.Plc.NetworkInterfaces, Does.Contain(ethernet));
                Assert.That(ethernet.Node.Layout.Position.X, Is.EqualTo(200));
                var mpi = (MpiProfiBusNetworkInterface)net.Interfaces[1];
                Assert.That(mpi.Address, Is.EqualTo(7));
                Assert.That(mpi.Network.NetworkType, Is.EqualTo(NetworkType.Mpi));
                Assert.That(mpi.Plc, Is.SameAs(ethernet.Plc));
                // The third object has no IP attribute; it must not inherit the preceding member's value.
                Assert.That(((EthernetNetworkInterface)net.Interfaces[2]).IpAddress, Is.Null);
                var tcp = net.Connections.Single(c => c.ConnectionType == NetworkConnectionType.Tcp);
                Assert.That(tcp.ConnectionType, Is.EqualTo(NetworkConnectionType.Tcp));
                Assert.That(tcp.LocalPlc, Is.SameAs(ethernet.Plc));
                Assert.That(tcp.LocalInterface, Is.SameAs(ethernet));
                Assert.That(tcp.RemotePlc, Is.Null);
                Assert.That(tcp.RemoteAddress.ToString(), Is.EqualTo("192.0.2.20"));
                Assert.That(tcp.LocalPort, Is.EqualTo(2000));
                Assert.That(tcp.RemotePort, Is.EqualTo(3000));
                var s7 = net.Connections.First(c => c.ConnectionType == NetworkConnectionType.S7);
                Assert.That(s7.LocalPlc, Is.SameAs(project.CPUFolders[0]));
                Assert.That(s7.RemotePlc, Is.SameAs(project.CPUFolders[1]));
                Assert.That(s7.Transport, Is.EqualTo(NetworkType.Mpi));
                Assert.That(s7.LocalInterface, Is.SameAs(mpi));
                Assert.That(s7.RemoteInterface, Is.SameAs(net.Interfaces[3]));
                Assert.That(net.Connections[1].LocalPlc, Is.SameAs(s7.RemotePlc));
                Assert.That(net.Connections[1].RemotePlc, Is.SameAs(s7.LocalPlc));
            }
        }

        [Test]
        public void MissingNetworkFilesGiveEmptyConfiguration()
        {
            using (var project = new Step7ProjectV5(Path.Combine(directory, "test.s7p"), false))
            {
                Assert.That(project.NetworkConfiguration.Interfaces, Is.Empty);
                Assert.That(project.NetworkConfiguration.Connections, Is.Empty);
                Assert.That(project.NetworkConfiguration.Diagnostics, Is.Empty);
            }
        }

        [Test]
        public void InvalidLayoutIsReportedWithoutLosingTopology()
        {
            CreateTopology();
            Write("Global/s7netpro.npa", new byte[] { 1, 2, 3 });
            using (var project = new Step7ProjectV5(Path.Combine(directory, "test.s7p"), false))
            {
                Assert.That(project.NetworkConfiguration.Diagnostics.Count, Is.EqualTo(1));
                Assert.That(project.NetworkConfiguration.Interfaces.Count, Is.EqualTo(4));
            }
        }

        [Test]
        public void BadMemberRelationsDoNotPreventOtherMembersFromLoading()
        {
            CreateTopology();
            string path = Path.Combine(directory, "S7Netze/S7NONFGX.lnk");
            byte[] bytes = File.ReadAllBytes(path);
            BitConverter.GetBytes(int.MaxValue).CopyTo(bytes, 520);
            File.WriteAllBytes(path, bytes);
            using (var project = new Step7ProjectV5(Path.Combine(directory, "test.s7p"), false))
            {
                Assert.That(project.NetworkConfiguration.Diagnostics.Count, Is.EqualTo(1));
                Assert.That(project.NetworkConfiguration.Interfaces.Count, Is.EqualTo(3));
            }
        }

        [TestCase(false, 2)]
        [TestCase(true, 3)]
        public void DeletedConnectionsRespectProjectOption(bool showDeleted, int expected)
        {
            CreateTopology();
            string path = Path.Combine(directory, "CONN/S7wntcps/IPVERB.DBF");
            byte[] bytes = File.ReadAllBytes(path);
            bytes[BitConverter.ToUInt16(bytes, 8)] = (byte)'*';
            File.WriteAllBytes(path, bytes);
            using (var project = new Step7ProjectV5(Path.Combine(directory, "test.s7p"), showDeleted))
                Assert.That(project.NetworkConfiguration.Connections.Count, Is.EqualTo(expected));
        }

        [Test]
        public void ObjectReferencesIncludeUnitAndType()
        {
            var a = new Step7ObjectReference(1, 2, 3, 4);
            Assert.That(a, Is.EqualTo(new Step7ObjectReference(1, 2, 3, 4)));
            Assert.That(a, Is.Not.EqualTo(new Step7ObjectReference(1, 9, 3, 4)));
            Assert.That(a, Is.Not.EqualTo(new Step7ObjectReference(1, 2, 9, 4)));
        }

        private void CreateTopology()
        {
            // Entirely synthetic DBF, NetPro and link records; no customer project content.
            WriteDbf("hOmSave7/s7hstatx/HOBJECT1.DBF", new[] { "ID", "OBJTYP", "UNITID", "UNITTYP", "NAME:C" },
                new object[] { 1, 0x141099, 10, 0x112001, "Station" },
                new object[] { 2, 0x14109a, 10, 0x112001, "Other station" });
            WriteDbf("hOmSave7/s7hk31ax/HRELATI1.DBF", new[] { "SOBJID", "TUNITID", "TOBJTYP", "RELID" },
                new object[] { 1, 1, 1314972, 0 });
            WriteDbf("hOmSave7/s7hk31ax/HOBJECT1.DBF", new[] { "ID", "OBJTYP", "UNITID", "UNITTYP", "NAME:C", "SUBSTATN", "MODULN" },
                new object[] { 1, 0x241582, 1, 0x141031, "PLC", 0, 2 });
            WriteDbf("hOmSave7/s7hk41ax/HRELATI1.DBF", new[] { "SOBJID", "TUNITID", "TOBJTYP", "RELID" },
                new object[] { 1, 2, 1314972, 0 });
            WriteDbf("hOmSave7/s7hk41ax/HOBJECT1.DBF", new[] { "ID", "OBJTYP", "UNITID", "UNITTYP", "NAME:C", "SUBSTATN", "MODULN" },
                new object[] { 1, 0x24155e, 1, 0x141031, "Other PLC", 0, 3 });
            var station = new[] { 0x112001, 10, 0x141099, 1 };
            var hardware = new[] { 0x241582, 1, 0x241584, 1 };
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(new byte[28]);
                Object(writer, 0x145103, 1, 0, "Ethernet");
                Object(writer, 0x145101, 1, 0, "MPI");
                Object(writer, 0x145203, 1, 512, "Port A", () =>
                {
                    Attribute(writer, 0xfe0, "C000020A");
                    Attribute(writer, 0xfe5, "FFFFFF00");
                });
                Object(writer, 0x145201, 1, 1024, "MPI port", () =>
                {
                    writer.Write(0x89a); writer.Write(0x89a); writer.Write(new byte[11]); writer.Write((byte)7);
                });
                Object(writer, 0x145203, 2, 0, "Unconnected");
                Object(writer, 0x145201, 2, 1536, "Other MPI port");
                Write("S7Netze/S7NONFGX.tab", stream.ToArray());
            }
            var links = new byte[2048];
            Relations(links, 512, (new[] { 0, 0, 0x145103, 1 }, 9), (station, 11), (hardware, 0x41));
            Relations(links, 1024, (new[] { 0, 0, 0x145101, 1 }, 9), (station, 11), (hardware, 0x41));
            Relations(links, 1536, (new[] { 0, 0, 0x145101, 1 }, 9), (new[] { 0x112001, 10, 0x14109a, 2 }, 11),
                (new[] { 0x24155e, 1, 0x24155f, 1 }, 0x41));
            Write("S7Netze/S7NONFGX.lnk", links);
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(Encoding.ASCII.GetBytes("SPAK")); writer.Write(16);
                Layout(writer, station, 200, 160, -1, -1);
                Layout(writer, new[] { 0, 0, 0x145103, 1 }, 20, 40, -1, -1);
                Layout(writer, new[] { 0, 0, 0x145101, 1 }, -1, -1, 20, 88);
                Write("Global/s7netpro.npa", stream.ToArray());
            }
            WriteDbf("CONN/S7wntcps/IPVERB.DBF", new[] { "ID", "TYPE", "UNITID", "NAME:C", "LOCALID", "UNSPEZ", "LINKID", "LOCTSAP_L", "LOCTSAP:C", "REMTSAP_L", "REMTSAP:C", "NETADDR_L", "NETADDR:C" },
                new object[] { 1, 0x145442, 1, "TCP", 1, 1, 512, 2, "\u0007\u00d0", 2, "\u000b\u00b8", 4, "\u00c0\0\u0002\u0014" });
            var tcpLinks = new byte[1024];
            Relations(tcpLinks, 512, (new[] { 0, 0, 0x145203, 1 }, 0x6c));
            Write("CONN/S7wntcps/iplink.dbf", tcpLinks);
            WriteDbf("CONN/VERBTAB.DBF", new[] { "ID", "TYPE", "LINKID" },
                new object[] { 1, 0x145301, 512 }, new object[] { 2, 0x145301, 1024 });
            var localCpu = new[] { 0x141031, 1, 0x241582, 1 };
            var remoteCpu = new[] { 0x141031, 1, 0x24155e, 1 };
            var containerLinks = new byte[1536];
            Relations(containerLinks, 512, (localCpu, 0x35));
            Relations(containerLinks, 1024, (remoteCpu, 0x35));
            Write("CONN/LINKVL.dbf", containerLinks);
            for (int id = 1; id <= 2; id++)
            {
                WriteDbf("CONN/CO_" + id + "/HOMOVERB.DBF", new[] { "ID", "TYPE", "NAME:C", "LOCALID", "LINKID" },
                    new object[] { 1, 0x145401, "S7", 1, 512 });
                var connectionLinks = new byte[1024];
                Relations(connectionLinks, 512, (id == 1 ? remoteCpu : localCpu, 0x3a),
                    (new[] { 0x145301, 3 - id, 0x145401, 1 }, 0x3e),
                    (new[] { 0, 0, 0x145201, 1 }, 0x6c), (new[] { 0, 0, 0x145201, 2 }, 0x6c));
                Write("CONN/CO_" + id + "/HOMOLINK.dbf", connectionLinks);
            }
        }

        private static void Object(BinaryWriter writer, int type, int id, int links, string name, Action attributes = null)
        {
            writer.Write(type); writer.Write(id); writer.Write(0L); writer.Write(0x112001); writer.Write(10);
            writer.Write(0); writer.Write(links); writer.Write(new byte[48]);
            Attribute(writer, 0x1001, name);
            attributes?.Invoke();
        }

        private static void Attribute(BinaryWriter writer, int id, string text)
        {
            writer.Write(id); writer.Write(id); writer.Write(new byte[11]);
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            writer.Write((byte)bytes.Length); writer.Write(bytes);
        }

        private static void Relations(byte[] bytes, int offset, params (int[] Reference, int Id)[] relations)
        {
            using (var stream = new MemoryStream(bytes))
            using (var writer = new BinaryWriter(stream))
            {
                stream.Position = offset + 8; writer.Write(relations.Length); writer.Write(0);
                foreach (var relation in relations)
                {
                    foreach (int value in relation.Reference) writer.Write(value);
                    writer.Write(relation.Id); writer.Write(4);
                }
            }
        }

        private static void Layout(BinaryWriter writer, int[] reference, params int[] coordinates)
        {
            writer.Write(reference[2]); writer.Write(reference[3]); writer.Write(reference[0]); writer.Write(reference[1]);
            writer.Write(0x112001); writer.Write(10);
            foreach (int value in coordinates) writer.Write(value);
        }

        private void Write(string relative, byte[] bytes)
        {
            string path = Path.Combine(directory, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, bytes);
        }

        private void WriteDbf(string relative, string[] columns, params object[][] rows)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(new byte[] { 3, 0, 1, 1 }); writer.Write(rows.Length);
                writer.Write((ushort)(33 + columns.Length * 32));
                writer.Write((ushort)(1 + columns.Sum(c => c.EndsWith(":C") ? 40 : 11))); writer.Write(new byte[20]);
                foreach (string column in columns)
                {
                    var descriptor = new byte[32];
                    Encoding.ASCII.GetBytes(column.Split(':')[0]).CopyTo(descriptor, 0);
                    descriptor[11] = (byte)(column.EndsWith(":C") ? 'C' : 'N');
                    descriptor[16] = (byte)(column.EndsWith(":C") ? 40 : 11);
                    writer.Write(descriptor);
                }
                writer.Write((byte)13);
                foreach (var row in rows)
                {
                    writer.Write((byte)' ');
                    for (int i = 0; i < columns.Length; i++)
                    {
                        bool text = columns[i].EndsWith(":C");
                        string value = Convert.ToString(row[i], CultureInfo.InvariantCulture);
                        writer.Write(Encoding.GetEncoding("ISO-8859-1").GetBytes(text ? value.PadRight(40) : value.PadLeft(11)));
                    }
                }
                writer.Write((byte)26); Write(relative, stream.ToArray());
            }
        }
    }
}
