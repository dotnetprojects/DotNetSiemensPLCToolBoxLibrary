using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary.Communication;

namespace MultipleSPSExample
{
    public partial class Form1 : Form
    {
        private string[] conn = new string[] {"MultipleSPSExample_Connection_1", 
            "MultipleSPSExample_Connection_2", "MultipleSPSExample_Connection_3", 
            "MultipleSPSExample_Connection_5", "MultipleSPSExample_Connection_6"};

        public Form1()
        {
            InitializeComponent();
        }

        private void connection1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.ShowConfiguration(conn[0], true);
        }

        private void connection2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.ShowConfiguration(conn[1], true);
        }

        private void connection3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.ShowConfiguration(conn[2], true);
        }

        private void connection4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.ShowConfiguration(conn[3], true);
        }

        private void connection5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.ShowConfiguration(conn[4], true);
        }

        private void connection6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.ShowConfiguration(conn[5], true);
        }

        private List<PLCConnection> plcConnections;


        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            plcConnections = new List<PLCConnection>();
            try
            {
                for (int n = 0; n < 6; n++)
                {
                    PLCConnection akConn = new PLCConnection(conn[n]);
                    plcConnections.Add(akConn);
                    akConn.Connect();
                }
            }
            catch (Exception ex)
            {
                closeConnections();
                MessageBox.Show("Fehler beim Verbindungsaufbau: " + ex.Message);
            }

            read_timer_plc1.Enabled = true;
            read_timer_plc2.Enabled = true;
        }

        private void closeConnections()
        {
            read_timer_plc1.Enabled = false;
            read_timer_plc2.Enabled = false;

            if (plcConnections!=null)
                foreach (PLCConnection plcConnection in plcConnections)
                {
                    plcConnection.Dispose();
                }
            plcConnections = null;            
        }

        private void read_timer_plc1_Tick(object sender, EventArgs e)
        {
            List<PLCTag> tags = new List<PLCTag>();
            tags.Add(new PLCTag("DB2.DBW4"));
            tags.Add(new PLCTag("DB5.DBW4"));
            plcConnections[0].ReadValues(tags);

            //Auslesen der gelesenen Werte
            Console.WriteLine(tags[0].Value);
            Console.WriteLine(tags[1].Value);

        }

        private void read_timer_plc2_Tick(object sender, EventArgs e)
        {
            List<PLCTag> tags = new List<PLCTag>();
            tags.Add(new PLCTag("DB5.DBW4"));
            tags.Add(new PLCTag("DB6.DBW4"));
            tags.Add(new PLCTag("P#DB1.DBX0.0 BYTE 8"));
            plcConnections[1].ReadValues(tags);

            //Auslesen der gelesenen Werte
            Console.WriteLine(tags[0].Value);
            Console.WriteLine(tags[1].Value);
            Console.WriteLine(tags[2].Value);
        }
    }
}
