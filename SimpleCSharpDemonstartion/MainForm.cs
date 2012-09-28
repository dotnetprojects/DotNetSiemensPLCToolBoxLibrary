using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.Communication.Library;
using DotNetSiemensPLCToolBoxLibrary.Communication.Library.Interfaces;
using DotNetSiemensPLCToolBoxLibrary.Communication.Library.Pdus;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace SimpleCSharpDemonstration
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private PLCConnection myConn = null;

        private PLCTag myValue = new PLCTag()
                                             {
                                                 ByteAddress = 0,
                                                 BitAddress = 0,
                                                 LibNoDaveDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.String,
                                                 ArraySize = 10
                                             };

        private void button1_Click(object sender, EventArgs e)
        {
            Configuration.ShowConfiguration("SimpleCSharpDemonstrationConnection", true);

            
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            myConn.ReadValue(myValue);
            lblString.Text = myValue.GetValueAsString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                myConn = new PLCConnection("SimpleCSharpDemonstrationConnection");
                myConn.Connect();
                timer.Enabled = true;
            }
            catch(Exception ex)
            { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5.SymbolTable symTab;
            symTab = DotNetSiemensPLCToolBoxLibrary.Projectfiles.SelectProjectPart.SelectSymbolTable();            
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct TestStruct
        {
            public Int16 aa;
            public Int16 bb;
            public Int16 cc;
            public Int32 ee;
            public UInt16 ff;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)] 
            public string test;
        }


        private void cmdReadStruct_Click(object sender, EventArgs e)
        {
            myConn = new PLCConnection("SimpleCSharpDemonstrationConnection");
            myConn.Connect();
            //PLCTagGeneric
            PLCTag<TestStruct> tst = new PLCTag<TestStruct>() {DatablockNumber = 97, ByteAddress = 0};
            myConn.ReadValue(tst);
            TestStruct read = tst.GenericValue;

            TestStruct wrt = new TestStruct();
            wrt.aa = 11;
            wrt.bb = 12;
            wrt.cc = 13;
            wrt.ee = 14;
            wrt.ff = 15;
            wrt.test = "Bin da!";
            tst.Controlvalue = wrt;
            myConn.WriteValue(tst);

        }

        private Connection aa, bb;
        private void button4_Click(object sender, EventArgs e)
        {
            /*
            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();

            sw.Start();
            Interface tmp = new S7OnlineInterface("S7ONLINE");
            //aa = tmp.ConnectPlc(new ConnectionConfig(2, 0, 2));
            aa = tmp.ConnectPlc(new ConnectionConfig(new IPAddress(new byte[] {192, 168, 1, 185}), 0, 2));
            Pdu_ReadRequest rd = new Pdu_ReadRequest();
            rd.addVarToReadRequest(0x83, 1, 0, 1);
            var rs = aa.ExecReadRequest(rd);
            var erg = rs.useResult(0);           
            aa.Dispose();
            sw.Stop();
            */
            myConn = new PLCConnection("SimpleCSharpDemonstrationConnection");
            myConn.Connect();
            //PLCTag tag = new PLCTag("MD0") {LibNoDaveDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.ByteArray, ArraySize=16};
            PLCTag tag = new PLCTag("P#DB1.DBX0.0 BYTE 8");
            myConn.ReadValue(tag);
            myConn.Disconnect();

            lblString.Text = tag.ValueAsString;  
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var cfg = new PLCConnectionConfiguration() {ConfigurationType = LibNodaveConnectionConfigurationType.ObjectSavedConfiguration, ConnectionName = "MyPrivateConnection"};
            Configuration.ShowConfiguration(cfg);
            myConn = new PLCConnection(cfg);
        }
    }
}
