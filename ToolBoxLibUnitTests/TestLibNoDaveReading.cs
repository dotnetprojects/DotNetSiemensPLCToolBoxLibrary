using DotNetSiemensPLCToolBoxLibrary.Communication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolBoxLibUnitTests
{
    [TestClass]
    public class TestLibNoDaveReading
    {
        [TestMethod]
        public void TestReading()
        {
            var wrapper = new ConnectionWrapper(480);

            var conn = new PLCConnection(new PLCConnectionConfiguration(), wrapper);
            

            var listTag = new List<PLCTag>();
            listTag.Add(new PLCTag("P#DB60.DBX0 BYTE 300"));
            listTag.Add(new PLCTag("P#DB10.DBX10 BYTE 300"));

            conn.ReadValues(listTag, true);

            var pdus = wrapper.PDUs;

            foreach (var pdu in pdus)
            {
                Console.WriteLine(pdu);
            }





            var wrapper2 = new ConnectionWrapper(480);

            var conn2 = new PLCConnection(new PLCConnectionConfiguration(), wrapper2);

            var listTag2 = new List<PLCTag>();
            listTag2.Add(new PLCTag("P#DB60.DBX0 BYTE 300"));
            listTag2.Add(new PLCTag("P#DB10.DBX10 BYTE 300"));

            conn2._TestNewReadValues(listTag2, true);

            var pdus2 = wrapper2.PDUs;

            foreach (var pdu in pdus2)
            {
                Console.WriteLine(pdu);
            }
        }
    }
}
