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
                myConn.Connect();
                timer.Enabled = true;
            }
            catch(Exception ex)
            { }
        }
    }
}
