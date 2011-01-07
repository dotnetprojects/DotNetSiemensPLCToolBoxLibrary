using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using LibNoDaveConnectionLibrary;
using LibNoDaveConnectionLibrary.DataTypes;
using LibNoDaveConnectionLibrary.MC7;

namespace JFK_VarTab
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Configuration.ShowConfiguration("JFK-TestConnection", true);
            //Configuration.ShowConfiguration( )
            label10.Text = (new LibNoDaveConnectionConfiguration("JFK-TestConnection")).ToString();
        }

        private LibNoDaveConnection myConn = new LibNoDaveConnection("JFK-TestConnection");

        private void button2_Click(object sender, EventArgs e)
        {             
            label2.Text = "Trying to connect...";
            Application.DoEvents();
            try
            {              
                //myConn.Connect(this.Handle.ToInt32());  
                button2.Enabled = false;
                button6.Enabled = true;
                lblConn.Visible = true;
                backgroundWorker1.RunWorkerAsync(this.Handle.ToInt32());                
            }
            catch (Exception ex)
            {
                label2.Text = "Error connecting: " + ex.Message;
            }
        }

       

        private void button1_Click_1(object sender, EventArgs e)
        {
            timer2.Enabled = false;

            if (backgroundWorker1.IsBusy)
                backgroundWorker1.CancelAsync();
            myConn.Dispose();

            
            button2.Enabled = true;
            lblConn.Visible = false;
            timer1.Enabled = false;
            button7.Enabled = true;
            button6.Enabled = true;
            button4.Enabled = false;
            
            label2.Text = "Not Connected";
            button6.Enabled = false;

        }
       
        private void Form1_Load(object sender, EventArgs e)
        {            
            //cmbSource.AddEnumToList(typeof (LibNoDaveDataSource));
            EnumListBoxExtensions.AddEnumToList(cmbSource, typeof(TagDataSource));
            //cmbType.AddEnumToList(typeof (LibNoDaveDataType));
            EnumListBoxExtensions.AddEnumToList(cmbType, typeof(TagDataType));
            EnumListBoxExtensions.AddEnumToList(cmbDataType, typeof(TagDataType));
            cmbSource.SelectedIndex = 9;
            cmbType.SelectedIndex = 4;

            label10.Text = (new LibNoDaveConnectionConfiguration("JFK-TestConnection")).ToString();
        }

        private List<LibNoDaveValue> myValues = new List<LibNoDaveValue>();

        private void button7_Click(object sender, EventArgs e)
        {
            
            myValues.Add(new LibNoDaveValue
                                   {
                                       LibNoDaveDataSource = (TagDataSource)((EnumListItem)cmbSource.SelectedItem).Value,
                                       ByteAddress = Convert.ToInt32(txtByte.Text) ,
                                       BitAddress = Convert.ToInt32(txtBit.Text),
                                       DatablockNumber = Convert.ToInt32(txtDB.Text),
                                       ArraySize = Convert.ToInt32(txtLen.Text),
                                       LibNoDaveDataType = (TagDataType)((EnumListItem)cmbType.SelectedItem).Value,
                                       BackupValuesCount = int.Parse(textBox2.Text)
                                   });

            listBox1.Items.Clear();
            listBox1.Items.AddRange(myValues.ToArray());

            var tmp1 = var1.SelectedItem;
            var tmp2 = var2.SelectedItem;
            var1.Items.Clear();
            var2.Items.Clear();
            var1.Items.Add("(none)");
            var2.Items.Add("(none)");
            var1.Items.AddRange(myValues.ToArray());
            var2.Items.AddRange(myValues.ToArray());
            if (tmp1 != null && var1.Items.Contains(tmp1)) var1.SelectedItem = tmp1;
            if (tmp2 != null && var2.Items.Contains(tmp2)) var2.SelectedItem = tmp2;

            lblItems.Text = listBox1.Items.Count.ToString() + " Item(s)";
        }

        private void button6_Click(object sender, EventArgs e)
        {            
            //myValues = myConn.ReadValues(myValues);
            //listBox1.Items.Clear();
            //listBox1.Items.AddRange(myValues.ToArray());
                button7.Enabled = false;
                button6.Enabled = false;
            button4.Enabled = true;
                timer1.Enabled = true;
            
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                myValues.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.Clear();
                listBox1.Items.AddRange(myValues.ToArray());
            }
            lblItems.Text = listBox1.Items.Count.ToString() + " Item(s)";

            var tmp1 = var1.SelectedItem;
            var tmp2 = var2.SelectedItem;
            var1.Items.Clear();
            var2.Items.Clear();
            var1.Items.Add("(none)");
            var2.Items.Add("(none)");
            var1.Items.AddRange(myValues.ToArray());
            var2.Items.AddRange(myValues.ToArray());
            if (tmp1 != null && var1.Items.Contains(tmp1)) var1.SelectedItem = tmp1;
            if (tmp2 != null && var2.Items.Contains(tmp2)) var2.SelectedItem = tmp2;
        }

        private int cnt = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {                
                myConn.ReadValues(myValues);
                listBox1.Items.Clear();
                listBox1.Items.AddRange(myValues.ToArray());
                cnt++;
                label1.Text = cnt.ToString();


                if (var1.SelectedItem != null && var1.SelectedItem.ToString() != "(none)")
                {
                    chart1.Series["Series1"].Points.Clear();
                    var tmp = (LibNoDaveValue) var1.SelectedItem;
                    foreach (var oldValue in tmp.OldValues)
                    {
                        double wrt = (float) Convert.ToDouble(oldValue);
                        chart1.Series["Series1"].Points.AddY(wrt);
                    }
                }

                if (var2.SelectedItem != null && var2.SelectedItem.ToString() != "(none)")
                {
                    chart1.Series["Series2"].Points.Clear();
                    var tmp = (LibNoDaveValue)var2.SelectedItem;
                    foreach (var oldValue in tmp.OldValues)
                    {
                        double wrt = (float)Convert.ToDouble(oldValue);
                        chart1.Series["Series2"].Points.AddY(wrt);
                    }
                }


            }
            catch (Exception ex)
            {
                label2.Text = ex.Message;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button7.Enabled = true;
            button4.Enabled = false;
            button6.Enabled = true;
            timer1.Enabled = false;
        }

       
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {

            if (listBox1.SelectedIndex >= 0 && timer1.Enabled == false)
            {
                LibNoDaveValue myAkValue = myValues[listBox1.SelectedIndex];
                String newVal = myAkValue.Value.ToString();
                System.Windows.Forms.DialogResult ret = Form1.InputBox("SPS Wert ändern", "Neuer Wert:", ref newVal);
                if (ret == DialogResult.OK)
                {
                    myAkValue.Controlvalue = newVal;
                    listBox1.Items.Clear();
                    listBox1.Items.AddRange(myValues.ToArray());
                    button9.Visible = true;
                    //myConn.WriteValue(myAkValue);
                }
                //timer1_Tick(sender, e);
            }

        }


        // Inputbox from: http://www.csharp-examples.net/inputbox/

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] {label, textBox, buttonOk, buttonCancel});
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            
            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void txtLen_TextChanged(object sender, EventArgs e)
        {

        }


        public delegate void stateConnectedDelegate();
        public stateConnectedDelegate myDelegate;

        private void stateConnected()
        {
            lblConn.Visible = false;
            label2.Text = "Connected";
            //timer2.Enabled = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                myConn.Connect((int) e.Argument);
                myDelegate += new stateConnectedDelegate(stateConnected);
                this.Invoke(myDelegate);
            }
            catch (Exception)
            {
                backgroundWorker1.ReportProgress(0);
            }
           
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            myConn.PLCStop();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            myConn.PLCStart();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //button1_Click_1(sender, e);
            label2.Text = "Error connecting...";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            myConn.WriteValues(myValues);
            button9.Visible = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                var tmp = (LibNoDaveValue) listBox1.SelectedItem;
                txtByteAddress.Text = tmp.GetS7FormatAddress();
                EnumListBoxExtensions.SelectEnumListItem(cmbDataType, (int) tmp.LibNoDaveDataType);
            }
        }        

        private void button11_Click(object sender, EventArgs e)
        {
            MessageBox.Show(myConn.PLCGetState().ToString());
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
           
        }

        private void button12_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            listBox2.MultiColumn = false;
            listBox2.Items.AddRange(myConn.PLCGetDiagnosticBuffer().ToArray());
        }

        private void button13_Click(object sender, EventArgs e)
        {
            timer1_Tick(sender, e);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            
        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show(myConn.PLCReadTime().ToString());
        }

        private void button15_Click(object sender, EventArgs e)
        {
            myConn.PLCSetTime(DateTime.Now);
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                timer1.Interval = int.Parse(textBox1.Text);
            }
            catch(Exception ex)
            {
                timer1.Interval = 1000;
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            foreach (var tmp in myValues)
                tmp.BackupValuesCount = int.Parse(textBox2.Text);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem!=null)
            {
                var itm = (LibNoDaveValue) listBox1.SelectedItem;
                string ret = itm.GetS7FormatAddress() + "\n";
                foreach (var oldValue in itm.OldValues)
                {
                    ret += oldValue.ToString() + "\n";
                }

                Clipboard.SetText(ret);
            }
        }

        private void var1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button16_Click(object sender, EventArgs e)
        {                                                        
            MessageBox.Show(String.Join(",",myConn.PLCListBlocks(PLCBlockType.AllEditableBlocks).ToArray()));
            string newVal = "";
            System.Windows.Forms.DialogResult ret = Form1.InputBox("Block löschen", "Welchen Block?", ref newVal);
            if (ret == DialogResult.OK)
            {
                myConn.PLCDeleteBlock(newVal);
                MessageBox.Show(String.Join(",", myConn.PLCListBlocks(PLCBlockType.AllEditableBlocks).ToArray()));
            }
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (myConn.PLCGetState() == PLCState.Running)
                label11.Text = "Running";
            else if (myConn.PLCGetState() == PLCState.Stopped)
                label11.Text = "Stopped";
            else if (myConn.PLCGetState() == PLCState.Starting)
                label11.Text = "Starting";
        }

        private void txtByteAddress_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtByteAddress_Leave(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                var tmp = (LibNoDaveValue)listBox1.SelectedItem;
                tmp.ChangeAddressFromString(txtByteAddress.Text);
                //txtByteAddress.Text = tmp.GetS7FormatAddress();

                int aa = listBox1.SelectedIndex;
                listBox1.Items.Clear();
                listBox1.Items.AddRange(myValues.ToArray());
                listBox1.SelectedIndex = aa;
            }
        }

        private void txtByteAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                txtByteAddress_Leave(null, null);
        }

        private void cmbDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                var tmp = (LibNoDaveValue)listBox1.SelectedItem;
                var bb = (EnumListItem) cmbDataType.SelectedItem;

                tmp.LibNoDaveDataType = (TagDataType) bb.Value;

                int aa = listBox1.SelectedIndex;
                listBox1.Items.Clear();
                listBox1.Items.AddRange(myValues.ToArray());
                listBox1.SelectedIndex = aa;
            }
            
        }

        private void button17_Click(object sender, EventArgs e)
        {
            string pwd = "";
            DialogResult ret = InputBox("Passwort", "Passwort", ref pwd);
            if (ret == DialogResult.OK)
                myConn.PLCSendPassword(pwd);
        }

       
        
       

    }
}

