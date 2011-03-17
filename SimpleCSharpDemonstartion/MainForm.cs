using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary;
using DotNetSiemensPLCToolBoxLibrary.Communication;
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
            
            if (symTab!=null)
                foreach (var symbolTableEntry in symTab.SymbolTableEntrys)
                {

                }             
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

        private void button4_Click(object sender, EventArgs e)
        {
            myConn = new PLCConnection("SimpleCSharpDemonstrationConnection");
            myConn.Connect();
            PLCTag tag = new PLCTag("MD200") {LibNoDaveDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.TimeOfDay};
            myConn.ReadValue(tag);
            lblString.Text = tag.ValueAsString;

            tag.ParseControlValueFromString("TOD#11:22:33.999");
            myConn.WriteValue(tag);
        }
    }
}
