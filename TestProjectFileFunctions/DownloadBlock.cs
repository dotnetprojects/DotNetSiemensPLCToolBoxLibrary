using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;

namespace TestProjectFileFunctions
{
    public partial class DownloadBlock : Form
    {
        private string ConnectionName = "";
        private PLCConnection myConn;
        public DownloadBlock(string ConnectionName)
        {
            this.ConnectionName = ConnectionName;
            InitializeComponent();
        }

        private void DownloadBlock_Load(object sender, EventArgs e)
        {            
            try
            {
                myConn = new PLCConnection(ConnectionName);
                label1.Text = ConnectionName + "\r\n" +
                              (new PLCConnectionConfiguration(ConnectionName)).ToString();
                myConn.Connect();
                listBox1.Items.AddRange(myConn.PLCListBlocks(PLCBlockType.AllEditableBlocks).ToArray());
                //myConn.PLCSendPassword("admin");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem!=null)
            {
                SaveFileDialog sav=new SaveFileDialog();
                sav.Filter = "*.hex|*.hex|*.*|*'.*";
                DialogResult res = sav.ShowDialog();
                if (res==DialogResult.OK)
                {
                    try
                    {
                        
                        byte[] blk = myConn.PLCGetBlockInMC7(listBox1.SelectedItem.ToString());

                        System.IO.FileStream _FileStream = new System.IO.FileStream(sav.FileName,
                                                                                    System.IO.FileMode.Create,
                                                                                    System.IO.FileAccess.Write);
                        _FileStream.Write(blk, 0, blk.Length);
                        _FileStream.Close();

                        MessageBox.Show("Block " + listBox1.SelectedItem.ToString() + " saved to: " + sav.FileName);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void DownloadBlock_FormClosing(object sender, FormClosingEventArgs e)
        {
            myConn.Dispose();
        }
    }
}
