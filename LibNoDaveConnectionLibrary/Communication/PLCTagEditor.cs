using System;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.General;

namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    public partial class PLCTagEditor : Form
    {
        public PLCTagEditor(PLCTag value)
        {
            _libnodavevalue = value;
            InitializeComponent();
        }
        
        private PLCTag _libnodavevalue;

        private void LibNoDaveValueEditor_Load(object sender, EventArgs e)
        {
            EnumListBoxExtensions.AddEnumToList(cmbSource, typeof(TagDataSource));
            EnumListBoxExtensions.AddEnumToList(cmbType, typeof(TagDataType));
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
                lblDB.Visible = true;
                lblPT1.Visible = true;
            }
            else
            {
                txtDB.Visible = false;
                lblDB.Visible = false;
                lblPT1.Visible = true;
            }
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((TagDataType)((EnumListItem)cmbType.SelectedItem).Value == TagDataType.Bool)
            {
                txtBit.Visible = true;
                lblBit.Visible = true;
                lblPT2.Visible = true;
            }
            else
            {
                txtBit.Visible = false;
                lblBit.Visible = false;
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
