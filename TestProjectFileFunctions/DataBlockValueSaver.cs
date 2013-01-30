using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;

namespace ToolboxForSiemensPLCs
{
    public partial class DataBlockValueSaver : Form
    {
        private string ConnectionName = "";
        private PLCConnection myConn;

        public DataBlockValueSaver(string ConnectionName)
        {
            this.ConnectionName = ConnectionName;
            InitializeComponent();

            myConn = new PLCConnection(ConnectionName);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var lst = myConn.PLCListBlocks(PLCBlockType.DB);
            listBox1.Items.AddRange(lst.ToArray());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog saveDataDir=new FolderBrowserDialog();

            if (saveDataDir.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string db in listBox1.SelectedItems)
                {
                    var size = myConn.PLCGetDataBlockSize(db);
                    var Tag = new PLCTag(db + ".DBX0.0");
                    Tag.TagDataType = TagDataType.ByteArray;
                    Tag.ArraySize = size;
                    myConn.ReadValue(Tag);

                    BinaryWriter wrt = new BinaryWriter(File.Open(Path.Combine(saveDataDir.SelectedPath, db + ".data"), FileMode.Create));
                    wrt.Write((byte[])Tag.Value);
                    wrt.Close();
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg=new OpenFileDialog();
            dlg.Filter = "*.data|*.data";
            dlg.Multiselect = true;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var fileName in dlg.FileNames)
                {
                    var rd = new BinaryReader(File.Open(fileName, FileMode.Open));
                    var bytes = rd.ReadBytes(Convert.ToInt32(rd.BaseStream.Length));

                    var fn = Path.GetFileName(fileName);
                    fn = fn.Substring(0, fn.Length - 5);
                    var Tag = new PLCTag(fn + ".DBX0.0");
                    Tag.TagDataType = TagDataType.ByteArray;
                    Tag.ArraySize = bytes.Length;
                    Tag.Controlvalue = bytes;
                    myConn.WriteValue(Tag);
                }
               
            }
        }
    }
}
