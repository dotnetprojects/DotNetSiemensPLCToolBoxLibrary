using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;

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
                                                 TagDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.String,
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
            ISymbolTable symTab;
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
            PLCTag<TestStruct> tst = new PLCTag<TestStruct>() {DataBlockNumber = 97, ByteAddress = 0};
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

        private byte[] getBytes(int value)
        {
            var retVal = new List<byte>();

            var wr = value;
            if (value < 0)
                wr *= -1;

            var anzBytes = 1;
            
            var anzBits = (int)Math.Ceiling(Math.Log(wr) / Math.Log(2));
            if (anzBits > 6)
            {
                anzBytes++;
                anzBits -= 6;
            }
            while (anzBits > 7)
            {
                anzBytes++;
                anzBits -= 7;
            }
            

            for (int i = 1; i <= anzBytes; i++)
            {
                byte wert = 0;

                var fakt = (anzBytes - i)*7;
                var divisor = (int) Math.Pow(2, fakt);
                wert = (byte) (wr/divisor);
                wr = wr%divisor;

                if (value < 0)
                {
                    if (i == anzBytes || wr == 0)
                        wert -= 1;

                    wert = (byte) ~(wert);
                    if (i == anzBytes)
                        wert &= 0x7F;

                    if (i == 1)
                        wert |= 0x40;
                }

                if (i != anzBytes)
                    wert |= 0x80;


                retVal.Add(wert);
            }

            return retVal.ToArray();
        }

        private int getInt(byte[] array, int offset)
        {
            int retVal = 0;
            var bt = array[offset];

            retVal += (bt & 0x3f);
            
            while ((bt & 0x80) > 0)
            {
                offset++;
                bt = array[offset];

                //if (retVal == 0)
                    retVal += 1;

                if (bt == 0)
                    retVal += 1;

                retVal = retVal << 6;
                
                //retVal = (retVal+1)*(2 ^ 7);
                retVal += (bt & 0x7f);                
            }

            return retVal;
        }

        private void button4_Click(object sender, EventArgs e)
        {

            var a = getBytes(-1);
            var wra = getInt(a, 0);
            var b = getBytes(-2);
            var wrb = getInt(b, 0);
            var z = getBytes(127);
            var wrz = getInt(z, 0);
            var c = getBytes(123456789);
            var wrc = getInt(c, 0);
            var d = getBytes(254);
            var wrd = getInt(d, 0);
            var ee = getBytes(255);
            var wree = getInt(ee, 0);
            var f = getBytes(256);
            var wrff = getInt(f, 0);
            var g = getBytes(-127);
            var wrg = getInt(g, 0);
            var h = getBytes(-128);
            var wrh = getInt(h, 0);
            var i = getBytes(-129);
            var wri = getInt(i, 0);
            //myConn = new PLCConnection("SimpleCSharpDemonstrationConnection");
            //myConn.Connect();

            //List<PLCTag> listTag = new List<PLCTag>();

            //for (int ii = 0; ii < 300; ii++)
            //    listTag.Add(new PLCTag() { DataBlockNumber = 100, ByteAddress = ii * 2, TagDataType = TagDataType.Word });
            //myConn.ReadValues(listTag);

            //var _tags = new List<PLCTag>();
            //var j = 0;
            //for (var i = 0; i < 96; i++)
            //{
            //    Console.WriteLine("DB1.DBD" + j.ToString(CultureInfo.InvariantCulture));
            //    _tags.Add(new PLCTag("DB1.DBD" + j.ToString(CultureInfo.InvariantCulture))
            //        {
            //            TagDataType = TagDataType.Float
            //        });
            //    j += 4;
            //}
            //myConn.ReadValues(_tags, false);

            //var tag = new PLCTag();
            //tag.TagDataType = TagDataType.Word;
            //tag.SymbolicAccessKey = "8a0e000124134d054000000a";
            //myConn.ReadValue(tag);
            /*tag.Controlvalue = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 88, 1, 2, 3, 4, 5, 6, 7, 8, 9, 77 };
            myConn.WriteValue(tag);
            var db = myConn.PLCGetBlockInMC7("DB99");
            MessageBox.Show("DB:" + Encoding.ASCII.GetString(db));
            myConn.PLCPutBlockFromMC7toPLC("DB98", db);*/


            //var prj = new Step7ProjectV11("C:\\Users\\jkuehner\\Documents\\Automatisierung\\Projekt2\\Projekt2.ap13");
            //var fld = prj.ProjectStructure as TIAProjectFolder;
            //var allFolders = fld.SubItems.Flatten(x => x.SubItems).Cast<TIAProjectFolder>();
            //var blockFolders = allFolders.Where(x => x is TIABlocksFolder).Cast<TIABlocksFolder>();
            //foreach (var ffld in blockFolders)
            //{
            //    var blocks = ffld.readPlcBlocksList();
            //}
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var cfg = new PLCConnectionConfiguration() {ConfigurationType = LibNodaveConnectionConfigurationType.ObjectSavedConfiguration, ConnectionName = "MyPrivateConnection"};
            Configuration.ShowConfiguration(cfg);
            myConn = new PLCConnection(cfg);
        }
    }
}
