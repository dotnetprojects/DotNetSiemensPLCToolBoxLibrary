using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;

namespace ReadSpeedtest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void cmdConfig_Click(object sender, EventArgs e)
        {
            DotNetSiemensPLCToolBoxLibrary.Communication.Configuration.ShowConfiguration("ReadSpeedTest", true);
        }

        private DotNetSiemensPLCToolBoxLibrary.Communication.PLCConnection myConn = null;
        private void cmdConnect_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "";
                myConn = new PLCConnection("ReadSpeedTest");
                myConn.Connect();
                lblStatus.Text = "Connected!";
            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message;                
            }
            
        }

        private void cmdReadWithReadValue_Click(object sender, EventArgs e)
        {
            try
            {
                txtWert.Text = "";
                txtZeit.Text = "";
                lblStatus.Text = "";

                if (myConn == null)
                    cmdConnect_Click(sender, e);

                PLCTag myTag = new PLCTag(txtTag.Text);


                Stopwatch sw = new Stopwatch();
                sw.Start();
                myConn.ReadValue(myTag);
                sw.Stop();

                txtZeit.Text = sw.ElapsedMilliseconds.ToString() + " ms";
                txtWert.Text = myTag.ValueAsString;
            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message;
            }
        }

        private void cmdReadWithReadValues_Click(object sender, EventArgs e)
        {
            try
            {
                txtWert.Text = "";
                txtZeit.Text = "";
                lblStatus.Text = "";

                if (myConn == null)
                    cmdConnect_Click(sender, e);

                PLCTag myTag = new PLCTag(txtTag.Text);

                List<PLCTag> tagList = new List<PLCTag>() {myTag};

                Stopwatch sw = new Stopwatch();
                sw.Start();
                myConn.ReadValues(tagList);
                sw.Stop();

                txtZeit.Text = sw.ElapsedMilliseconds.ToString() + " ms";
                txtWert.Text = myTag.ValueAsString;
            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message;
            }
        }

        private void cmdReadWithLibnodave_Click(object sender, EventArgs e)
        {
            try
            {
                txtWert.Text = "";
                txtZeit.Text = "";
                lblStatus.Text = "";

                if (myConn == null)
                    cmdConnect_Click(sender, e);

                PLCTag myTag = new PLCTag(txtTag.Text);

                byte[] tmp = new byte[myTag.ArraySize];

                Stopwatch sw = new Stopwatch();
                sw.Start();
                int res = myConn._dc.readManyBytes(DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave.libnodave.daveDB, myTag.DatablockNumber, myTag.ByteAddress, myTag.ArraySize, ref tmp);
                sw.Stop();
                if (res != 0)
                {
                    MessageBox.Show("Error using libnodave, readmaybytes! Code:" + res.ToString());
                    return;
                }

                txtZeit.Text = sw.ElapsedMilliseconds.ToString() + " ms";
                string tmp2 = "{";
                foreach (byte b in tmp)
                {
                    tmp2 += b.ToString() + ",";
                }
                txtWert.Text = tmp2 + "}";
            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message;
            }
        }

        private void cmdReadValuesWithvarTab_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Achtung: Es können hier nur rund 200 Bytes gelesen werden, aber die länge wird noch nicht geprüft! Und es wird nur die Zeit zum lesen, nicht die zum starten der Anfrage gemessen!");
            try
            {
                txtWert.Text = "";
                txtZeit.Text = "";
                lblStatus.Text = "";

                if (myConn == null)
                    cmdConnect_Click(sender, e);

                PLCTag myTag = new PLCTag(txtTag.Text);

                List<PLCTag> tagList = new List<PLCTag>() { myTag };

                var read = myConn.ReadValuesWithVarTabFunctions(tagList, PLCTriggerVarTab.BeginOfCycle);
                Stopwatch sw = new Stopwatch();
                sw.Start();                
                read.RequestData();
                sw.Stop();

                txtZeit.Text = sw.ElapsedMilliseconds.ToString() + " ms";
                txtWert.Text = myTag.ValueAsString;
            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message;
            }

        }
    }
}

