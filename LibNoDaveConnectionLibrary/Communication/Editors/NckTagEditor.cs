using System;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.General;

namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    public partial class NckTagEditor : Form
    {
        public static PLCNckTag ShowPLCTagEditor(PLCNckTag tag)
        {
            NckTagEditor tagedt = new NckTagEditor();
            tagedt._libnodavevalue = tag;
            tagedt.ShowDialog();
            return tagedt._libnodavevalue;
        }

        internal NckTagEditor()
        {
            //_libnodavevalue = value;
            InitializeComponent();
        }

        private NC_Var ncVar = new NC_Var();
        private PLCNckTag _libnodavevalue;
        private bool startWasNull = false;

        private void LibNoDaveValueEditor_Load(object sender, EventArgs e)
        {
            if (_libnodavevalue != null)
            {
                ncVar = new NC_Var(_libnodavevalue);
            }
            else
            {
                startWasNull = true;
                _libnodavevalue = new PLCNckTag();
            }

            pGridNCK.SelectedObject = ncVar;
        }
        
        private void OK_Click(object sender, EventArgs e)
        {
            _libnodavevalue = ncVar.GetNckTag();

            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            if (startWasNull)
                _libnodavevalue = null;
            this.Close();
        }
    }
}
