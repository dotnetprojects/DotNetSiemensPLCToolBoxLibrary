using DotNetSiemensPLCToolBoxLibrary.Communication;
using System;
using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace ToolBoxLibUnitTests
{
    [TestFixture]
    public class TestLibNoDaveReading
    {
        [Test]
        public void TestReading()
        {
            var wrapper = new ConnectionWrapper(480);
            var conn = new PLCConnection(new PLCConnectionConfiguration(), wrapper);
            var listTag = new List<PLCTag>();
            listTag.Add(new PLCTag("P#DB60.DBX0 BYTE 300"));
            listTag.Add(new PLCTag("P#DB10.DBX10 BYTE 300"));
            conn.ReadValues(listTag, true);
            var pdus = wrapper.PDUs;
            string req = string.Join(Environment.NewLine, pdus);
            string t =
@"READ  Area:132, DBnum:10, Start:10, Bytes:300
READ  Area:132, DBnum:60, Start:0, Bytes:300";
            ClassicAssert.AreEqual(req.Replace("\r\n", "\n"), t.Replace("\r\n", "\n"));

            //var wrapper2 = new ConnectionWrapper(480);
            //var conn2 = new PLCConnection(new PLCConnectionConfiguration(), wrapper2);
            //var listTag2 = new List<PLCTag>();
            //listTag2.Add(new PLCTag("P#DB60.DBX0 BYTE 300"));
            //listTag2.Add(new PLCTag("P#DB10.DBX10 BYTE 300"));
            //conn2._TestNewReadValues(listTag2, true);
            //var pdus2 = wrapper2.PDUs;
            //string req2 = string.Join(Environment.NewLine, pdus2);
            //string t2 = "READ  Area:132, DBnum:10, Start:10, Bytes:300\r\nREAD  Area:132, DBnum:60, Start:0, Bytes:300";
            //ClassicAssert.AreEqual(req2, t2);

            var wrapper3 = new ConnectionWrapper(480);
            var conn3 = new PLCConnection(new PLCConnectionConfiguration(), wrapper3);
            var listTag3 = new List<PLCTag>();
            listTag3.Add(new PLCTag("P#DB60.DBX0 BYTE 300") { DontSplitValue = false });
            listTag3.Add(new PLCTag("P#DB10.DBX10 BYTE 300") { DontSplitValue = false });
            conn3.ReadValues(listTag3, true);
            var pdus3 = wrapper3.PDUs;
            string req3 = string.Join(Environment.NewLine, pdus3);
            string t3 = "READ  Area:132, DBnum:10, Start:10, Bytes:300\r\nREAD  Area:132, DBnum:60, Start:0, Bytes:140\r\nREAD  Area:132, DBnum:60, Start:140, Bytes:160";
            ClassicAssert.AreEqual(req3.Replace("\r\n", "\n"), t3.Replace("\r\n", "\n"));

            var wrapper4 = new ConnectionWrapper(480);
            var conn4 = new PLCConnection(new PLCConnectionConfiguration(), wrapper4);
            var listTag4 = new List<PLCTag>();
            listTag4.Add(new PLCTag("P#DB1.DBX0 BYTE 4") { DontSplitValue = false });
            listTag4.Add(new PLCTag("P#DB1.DBX8 BYTE 4") { DontSplitValue = false });
            listTag4.Add(new PLCTag("P#DB1.DBX10 BYTE 4") { DontSplitValue = false });
            listTag4.Add(new PLCTag("P#DB1.DBX30 BYTE 4") { DontSplitValue = false });
            listTag4.Add(new PLCTag("P#DB1.DBX50 BYTE 4") { DontSplitValue = false });
            listTag4.Add(new PLCTag("P#DB1.DBX80 BYTE 4") { DontSplitValue = false });
            conn4.ReadValues(listTag4, true);
            var pdus4 = wrapper4.PDUs;
            string req4 = string.Join(Environment.NewLine, pdus4);
            string t4 = "READ  Area:132, DBnum:1, Start:0, Bytes:14\r\nREAD  Area:132, DBnum:1, Start:30, Bytes:4\r\nREAD  Area:132, DBnum:1, Start:50, Bytes:4\r\nREAD  Area:132, DBnum:1, Start:80, Bytes:4";
            ClassicAssert.AreEqual(req4.Replace("\r\n", "\n"), t4.Replace("\r\n", "\n"));

            var wrapper5 = new ConnectionWrapper(240);
            var conn5 = new PLCConnection(new PLCConnectionConfiguration(), wrapper5);
            var listTag5 = new List<PLCTag>();
            listTag5.Add(new PLCTag("P#DB1.DBX0 BYTE 4") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX8 BYTE 4") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX10 BYTE 4") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX30 BYTE 4") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX50 BYTE 4") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX80 BYTE 4") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX90 BYTE 30") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX100 BYTE 4") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX140 BYTE 4") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX160 BYTE 4") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX180 BYTE 44") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX230 BYTE 20") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX240 BYTE 4") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX250 BYTE 4") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX260 BYTE 4") { DontSplitValue = false });
            listTag5.Add(new PLCTag("P#DB1.DBX270 BYTE 4") { DontSplitValue = false });
            conn5.ReadValues(listTag5, true);
            var pdus5 = wrapper5.PDUs;
            string req5 = string.Join(Environment.NewLine, pdus5);
            string t5 = "READ  Area:132, DBnum:1, Start:0, Bytes:14\r\nREAD  Area:132, DBnum:1, Start:30, Bytes:4\r\nREAD  Area:132, DBnum:1, Start:50, Bytes:4\r\nREAD  Area:132, DBnum:1, Start:80, Bytes:4\r\nREAD  Area:132, DBnum:1, Start:90, Bytes:30\r\nREAD  Area:132, DBnum:1, Start:140, Bytes:4\r\nREAD  Area:132, DBnum:1, Start:160, Bytes:4\r\nREAD  Area:132, DBnum:1, Start:180, Bytes:44\r\nREAD  Area:132, DBnum:1, Start:230, Bytes:24\r\nREAD  Area:132, DBnum:1, Start:260, Bytes:4\r\nREAD  Area:132, DBnum:1, Start:270, Bytes:4";
            ClassicAssert.AreEqual(req5.Replace("\r\n", "\n"), t5.Replace("\r\n", "\n"));

            //var tag=new PLCNckTag() { TagDataType = TagDataType.Float, NckArea = 0xa, NckUnit = 0x8,NckColumn = 0x23, NckLine = 0x1,NckModule = 0x1a,NckLinecount = 0x1};
        }

        [Test]
        public void TestReading2()
        {
            var wrapper = new ConnectionWrapper(480);
            var conn = new PLCConnection(new PLCConnectionConfiguration(), wrapper);
            var listTag = new List<PLCTag>();
            listTag.Add(new PLCTag("DB2.DBB0"));
            listTag.Add(new PLCTag("DB2.DBB1"));
            listTag.Add(new PLCTag("DB2.DBB2"));
            conn.ReadValues(listTag, true);
            var pdus = wrapper.PDUs;
            string req = string.Join(Environment.NewLine, pdus);
            string t = "READ  Area:132, DBnum:2, Start:0, Bytes:3";
            ClassicAssert.AreEqual(req, t);
        }

        [Test]
        public void TestReading3()
        {
            var wrapper = new ConnectionWrapper(480);
            var conn = new PLCConnection(new PLCConnectionConfiguration(), wrapper);
            var listTag = new List<PLCTag>();

            listTag.Add(new PLCTag("DB781.DBX5.0"));
            listTag.Add(new PLCTag("DB781.DBX7.0"));
            listTag.Add(new PLCTag("DB781.DBX9.0"));
            listTag.Add(new PLCTag("DB781.DBX11.0"));
            listTag.Add(new PLCTag("DB781.DBX13.0"));
            listTag.Add(new PLCTag("DB781.DBX4.2"));
            listTag.Add(new PLCTag("DB781.DBX6.2"));
            listTag.Add(new PLCTag("DB781.DBX8.2"));
            listTag.Add(new PLCTag("DB781.DBX10.2"));
            listTag.Add(new PLCTag("DB781.DBX12.2"));
            listTag.Add(new PLCTag("DB781.DBX3.1"));
            listTag.Add(new PLCTag("DB781.DBX5.1"));
            listTag.Add(new PLCTag("DB781.DBX7.1"));
            listTag.Add(new PLCTag("DB781.DBX9.1"));
            listTag.Add(new PLCTag("DB781.DBX11.1"));
            listTag.Add(new PLCTag("DB781.DBX13.1"));

            conn.ReadValues(listTag, true);
            var pdus = wrapper.PDUs;
            string req = string.Join(Environment.NewLine, pdus);
            string t = "READ  Area:132, DBnum:781, Start:3, Bytes:11";
            ClassicAssert.AreEqual(req, t);
        }

        [Test]
        public void TestReading4()
        {
            var wrapper = new ConnectionWrapper(480);
            var conn = new PLCConnection(new PLCConnectionConfiguration(), wrapper);
            var listTag = new List<PLCTag>();

            listTag.Add(new PLCTag("DB781.DBX5.0"));
            listTag.Add(new PLCTag("DB781.DBX10.0"));

            conn.ReadValues(listTag, true);
            var pdus = wrapper.PDUs;
            string req = string.Join(Environment.NewLine, pdus);
            string t = "READ  Area:132, DBnum:781, Start:5, Bytes:6";
            ClassicAssert.AreEqual(req, t);
        }

        [Test]
        public void TestReading5()
        {
            var wrapper = new ConnectionWrapper(480);
            var conn = new PLCConnection(new PLCConnectionConfiguration(), wrapper);
            var listTag = new List<PLCTag>();

            listTag.Add(new PLCTag("DB781.DBX5.0"));
            listTag.Add(new PLCTag("DB781.DBX10.0"));
            listTag.Add(new PLCTag("DB781.DBX15.0"));

            conn.ReadValues(listTag, true);
            var pdus = wrapper.PDUs;
            string req = string.Join(Environment.NewLine, pdus);
            string t = "READ  Area:132, DBnum:781, Start:5, Bytes:11";
            ClassicAssert.AreEqual(req, t);
        }

        [Test]
        public void TestReading6()
        {
            var wrapper = new ConnectionWrapper(480);
            var conn = new PLCConnection(new PLCConnectionConfiguration(), wrapper);
            var listTag = new List<PLCTag>();

            listTag.Add(new PLCTag("DB781.DBX5.0"));
            listTag.Add(new PLCTag("DB781.DBX9.0"));
            listTag.Add(new PLCTag("DB781.DBX15.0"));

            conn.ReadValues(listTag, true);
            var pdus = wrapper.PDUs;
            string req = string.Join(Environment.NewLine, pdus);
            string t = "READ  Area:132, DBnum:781, Start:5, Bytes:11";
            ClassicAssert.AreEqual(req, t);
        }

        class ConnectionWrapperResult : ConnectionWrapper
        {
            private readonly Action<int, byte[]> _fillBuffer;

            public ConnectionWrapperResult(Action<int, byte[]> fillBuffer) : base(480)
            {
                _fillBuffer = fillBuffer;
            }

            public override int useResultBuffer(IresultSet rs, int number, byte[] buffer)
            {
                _fillBuffer(number, buffer);
                return base.useResultBuffer(rs, number, buffer);
            }
        }


        [Test]
        public void TestReading7()
        {
           
            var wrapper = new ConnectionWrapperResult((nr, r) =>
            {
                r[0] = 0;
                r[1] = 5;
                r[2] = 0;
                r[3] = 1;
                r[4] = 0;
                r[5] = 1;
            });
            var conn = new PLCConnection(new PLCConnectionConfiguration(), wrapper);
            var listTag = new List<PLCTag>();

            listTag.Add(new PLCTag("DB781.DBX0.2"));
            listTag.Add(new PLCTag("DB781.DBX1.0"));
            listTag.Add(new PLCTag("DB781.DBX1.0"));
            listTag.Add(new PLCTag("DB781.DBX1.0"));
            listTag.Add(new PLCTag("DB781.DBX1.2"));
            listTag.Add(new PLCTag("DB781.DBX1.2"));
            listTag.Add(new PLCTag("DB781.DBX1.2"));
            listTag.Add(new PLCTag("DB781.DBX2.2"));
            listTag.Add(new PLCTag("DB781.DBX3.0"));
            listTag.Add(new PLCTag("DB781.DBX3.2"));
            listTag.Add(new PLCTag("DB781.DBX4.2"));
            listTag.Add(new PLCTag("DB781.DBX5.0"));
            listTag.Add(new PLCTag("DB781.DBX5.0"));
            listTag.Add(new PLCTag("DB781.DBX5.0"));
            listTag.Add(new PLCTag("DB781.DBX5.2"));

            conn.ReadValues(listTag, true);
            var pdus = wrapper.PDUs;
            string req = string.Join(Environment.NewLine, pdus);
            string t = "READ  Area:132, DBnum:781, Start:0, Bytes:6";
            ClassicAssert.AreEqual(req, t);

        }
    }
}
