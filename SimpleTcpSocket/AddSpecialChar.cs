using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleTcpSocket
{
    public partial class AddSpecialChar : Form
    {
        private TextBox textBox;

        public AddSpecialChar(TextBox textBox)
        {
            InitializeComponent();

            this.textBox = textBox;
        }

        private void AddSpecialChar_Load(object sender, EventArgs e)
        {
            cmbSpecialChar.Items.Add("NUL (0x00)");
            cmbSpecialChar.Items.Add("SOH (0x01)");
            cmbSpecialChar.Items.Add("STX (0x02)");
            cmbSpecialChar.Items.Add("ETX (0x03)");
            cmbSpecialChar.Items.Add("EOT (0x04)");
            cmbSpecialChar.Items.Add("ENQ (0x05)");
            cmbSpecialChar.Items.Add("ACK (0x06)");
            cmbSpecialChar.Items.Add("BEL (0x07)");
            cmbSpecialChar.Items.Add("BS  (0x08)");
            cmbSpecialChar.Items.Add("LF  (0x0A)");
            cmbSpecialChar.Items.Add("VT  (0x0B)");
            cmbSpecialChar.Items.Add("FF  (0x0C)");
            cmbSpecialChar.Items.Add("CR  (0x0D)");
            cmbSpecialChar.Items.Add("SO  (0x0E)");
            cmbSpecialChar.Items.Add("SI  (0x0F)");
            cmbSpecialChar.Items.Add("DLE (0x10)");
            cmbSpecialChar.Items.Add("DC1 (0x11)");
            cmbSpecialChar.Items.Add("DC2 (0x12)");
            cmbSpecialChar.Items.Add("DC3 (0x13)");
            cmbSpecialChar.Items.Add("DC4 (0x14)");
            cmbSpecialChar.Items.Add("NAK (0x15)");
            cmbSpecialChar.Items.Add("SYN (0x16)");
            cmbSpecialChar.Items.Add("ETB (0x17)");
            cmbSpecialChar.Items.Add("CAN (0x18)");
            cmbSpecialChar.Items.Add("EM  (0x19)");
            cmbSpecialChar.Items.Add("SUB (0x1A)");
            cmbSpecialChar.Items.Add("ESC (0x1B)");
            cmbSpecialChar.Items.Add("FS  (0x1C)");
            cmbSpecialChar.Items.Add("GS  (0x1D)");
            cmbSpecialChar.Items.Add("RS  (0x1E)");
            cmbSpecialChar.Items.Add("US  (0x1F)");
            cmbSpecialChar.Items.Add("SP  (0x20)");

            cmbSpecialChar.SelectedIndex = 0;
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            textBox.Text = textBox.Text + ((char)cmbSpecialChar.SelectedIndex).ToString();
        }
    }
}
