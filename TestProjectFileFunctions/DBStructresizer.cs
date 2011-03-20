using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;

namespace TestProjectFileFunctions
{
    public partial class DBStructresizer : Form
    {
        private byte[] readBytes = null;

        public DBStructresizer()
        {
            InitializeComponent();
        }

        private void DBStructresizer_Load(object sender, EventArgs e)
        {
            lstConnections.Items.Clear();
            lstConnections.Items.AddRange(PLCConnectionConfiguration.GetConfigurationNames());

            if (lstConnections.Items.Count > 0)
                lstConnections.SelectedItem = lstConnections.Items[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                PLCConnection myConn = new PLCConnection(lstConnections.SelectedItem.ToString());
                myConn.Connect();
                PLCTag plcTag=new PLCTag(){LibNoDaveDataType = TagDataType.ByteArray, LibNoDaveDataSource = TagDataSource.Datablock, DatablockNumber = Convert.ToInt32(txtDB.Text), ByteAddress = Convert.ToInt32(txtStartByte.Text), ArraySize = Convert.ToInt32(txtBytes.Text)};
                myConn.ReadValue(plcTag);
                readBytes = (byte[]) plcTag.Value;
                myConn.Disconnect();
                lblState.Text = readBytes.Length.ToString() + " Bytes gelesen";
                MessageBox.Show("So nun den neuen DB übertragen....");
            }
            catch (Exception ex)
            {
                lblState.Text = ex.Message;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                PLCConnection myConn = new PLCConnection(lstConnections.SelectedItem.ToString());
                myConn.Connect();

                int anz = readBytes.Length/Convert.ToInt32(txtSize.Text);
                int olen = Convert.ToInt32(txtSize.Text);
                int len = Convert.ToInt32(txtNewSize.Text);

                for (int n = 0; n < anz; n++)
                {
                    PLCTag plcTag = new PLCTag() {LibNoDaveDataType = TagDataType.ByteArray, LibNoDaveDataSource = TagDataSource.Datablock, DatablockNumber = Convert.ToInt32(txtDB.Text), ByteAddress = Convert.ToInt32(txtStartByte.Text) + n*Convert.ToInt32(txtNewSize.Text), ArraySize = Convert.ToInt32(txtSize.Text)};

                    byte[] ctrlV = new byte[len];
                    Array.Copy(readBytes, olen*n, ctrlV, 0, olen);
                    plcTag.Controlvalue = ctrlV;

                    myConn.WriteValue(plcTag);
                }
                lblState.Text = anz.ToString() + " Strukturen a " + len.ToString() + " Bytes geschrieben!";
            }
            catch (Exception ex)
            {
                lblState.Text = ex.Message;
            }
        }
    }
}
