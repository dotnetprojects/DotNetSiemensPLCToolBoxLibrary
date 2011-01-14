using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleCSharpDemonstration
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private DotNetSiemensPLCToolBoxLibrary.LibNoDaveConnection myConn = null;
        //private DotNetSiemensPLCToolBoxLibrary.LibNoDaveValue myValue=new LibNoDaveValue();

        private void button1_Click(object sender, EventArgs e)
        {
            DotNetSiemensPLCToolBoxLibrary.Configuration.ShowConfiguration("SimpleCSharpDemonstrationConnection", true);
        }

        private void timer_Tick(object sender, EventArgs e)
        {

           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }
    }
}
