using System;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.General;

namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    public partial class PLCTagEditor : Form
    {
        public static PLCTag ShowPLCTagEditor(PLCTag tag)
        {
            PLCTagEditor tagedt=new PLCTagEditor();
            tagedt._libnodavevalue = new PLCTag(tag);
            tagedt.ShowDialog();
            return tagedt._libnodavevalue;
        }

        internal PLCTagEditor()
        {
            //_libnodavevalue = value;
            InitializeComponent();
        }
        
        private PLCTag _libnodavevalue;
        private bool startWasNull = false;

        //private PLCTag backupPLCTag = null;

        private void LibNoDaveValueEditor_Load(object sender, EventArgs e)
        {
            EnumListBoxExtensions.AddEnumToList(cmbSource, typeof(TagDataSource));
            EnumListBoxExtensions.AddEnumToList(cmbType, typeof(TagDataType));

            if (_libnodavevalue == null)
            {
                startWasNull = true;
                _libnodavevalue = new PLCTag();
            }
            //else
            //    backupPLCTag = new PLCTag(_libnodavevalue);

            _libnodavevalue.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_libnodavevalue_PropertyChanged);
            _libnodavevalue_PropertyChanged(null, null);            
        }

        void _libnodavevalue_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            txtValueInS7.Text = _libnodavevalue.S7FormatAddress;
            txtDB.Text = _libnodavevalue.DatablockNumber.ToString();
            txtByte.Text = _libnodavevalue.ByteAddress.ToString();
            txtBit.Text = _libnodavevalue.BitAddress.ToString();
            txtLen.Text = _libnodavevalue.ArraySize.ToString();
            cmbSource.SelectedIndex = cmbSource.FindString(_libnodavevalue.LibNoDaveDataSource.ToString());
            cmbType.SelectedIndex = cmbType.FindString(_libnodavevalue.LibNoDaveDataType.ToString());

        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (_libnodavevalue==null)
                _libnodavevalue = new PLCTag();

            if ((TagDataSource)((EnumListItem)cmbSource.SelectedItem).Value == TagDataSource.Datablock || (TagDataSource)((EnumListItem)cmbSource.SelectedItem).Value == TagDataSource.InstanceDatablock)
                _libnodavevalue.DatablockNumber = int.Parse(txtDB.Text);
            else
                _libnodavevalue.DatablockNumber = 0;

            _libnodavevalue.ByteAddress = int.Parse(txtByte.Text);

            if ((TagDataType)((EnumListItem)cmbType.SelectedItem).Value == TagDataType.Bool)
                _libnodavevalue.BitAddress = int.Parse(txtBit.Text);
            else
                _libnodavevalue.BitAddress = 0;


            if ((TagDataType)((EnumListItem)cmbType.SelectedItem).Value == TagDataType.String || (TagDataType)((EnumListItem)cmbType.SelectedItem).Value == TagDataType.ByteArray || (TagDataType)((EnumListItem)cmbType.SelectedItem).Value == TagDataType.CharArray)
                _libnodavevalue.ArraySize = int.Parse(txtLen.Text);
            else
                _libnodavevalue.ArraySize = 0;

            _libnodavevalue.LibNoDaveDataSource = (TagDataSource) ((EnumListItem) cmbSource.SelectedItem).Value;
            _libnodavevalue.LibNoDaveDataType = (TagDataType)((EnumListItem)cmbType.SelectedItem).Value;

            this.Close();


        }

        private void cmbSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((TagDataSource)((EnumListItem)cmbSource.SelectedItem).Value == TagDataSource.Datablock || (TagDataSource)((EnumListItem)cmbSource.SelectedItem).Value == TagDataSource.InstanceDatablock)
            {
                txtDB.Visible = true;
                //lblDB.Visible = true;
                lblPT1.Visible = true;
            }
            else
            {
                txtDB.Visible = false;
                //lblDB.Visible = false;
                lblPT1.Visible = true;
            }
            _libnodavevalue.LibNoDaveDataSource = (TagDataSource)((EnumListItem)cmbSource.SelectedItem).Value;
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((TagDataType)((EnumListItem)cmbType.SelectedItem).Value == TagDataType.Bool)
            {
                txtBit.Visible = true;
                //lblBit.Visible = true;
                lblPT2.Visible = true;
            }
            else
            {
                txtBit.Visible = false;
                //lblBit.Visible = false;
                lblPT2.Visible = false;
            }

            if ((TagDataType)((EnumListItem)cmbType.SelectedItem).Value == TagDataType.String || (TagDataType)((EnumListItem)cmbType.SelectedItem).Value == TagDataType.ByteArray || (TagDataType)((EnumListItem)cmbType.SelectedItem).Value == TagDataType.CharArray)
            {
                txtLen.Visible = true;
                lblLen.Visible = true;
            }
            else
            {
                txtLen.Visible = false;
                lblLen.Visible = false;
            }
            _libnodavevalue.LibNoDaveDataType = (TagDataType)((EnumListItem)cmbType.SelectedItem).Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (startWasNull)
                _libnodavevalue = null;
            //else
            //    _libnodavevalue = backupPLCTag;
            this.Close();
        }

        private void txtDB_TextChanged(object sender, EventArgs e)
        {
            _libnodavevalue.DatablockNumber = Int32.Parse(txtDB.Text);
        }

        private void txtByte_TextChanged(object sender, EventArgs e)
        {
            _libnodavevalue.ByteAddress = Int32.Parse(txtByte.Text);
        }

        private void txtBit_TextChanged(object sender, EventArgs e)
        {
            _libnodavevalue.BitAddress = Int32.Parse(txtBit.Text);
        }

        private void txtLen_TextChanged(object sender, EventArgs e)
        {
            _libnodavevalue.ArraySize = Int32.Parse(txtLen.Text);
        }
    }
}
