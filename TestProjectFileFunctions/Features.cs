using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace TestProjectFileFunctions
{
    public partial class Features : Form
    {
        public Features()
        {
            InitializeComponent();
        }

        private void Features_Load(object sender, EventArgs e)
        {
            lblVer.Text = String.Format("{0}",  Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }
    }
}
