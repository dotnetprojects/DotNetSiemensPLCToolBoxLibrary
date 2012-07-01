using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using JFKCommonLibrary.ExtensionMethods;
using JFKCommonLibrary.Networking;
using JFKCommonLibrary.Settings;
using Kopplungstester.Properties;

namespace Kopplungstester
{
    public partial class MainForm : Form
    {
        private TCPFunctionsAsync myTCP_send;
        private TCPFunctionsAsync myTCP_rec;

        public MainForm()
        {
            InitializeComponent();

            LoadSettings();
        }

        private void LoadSettings()
        {
            optTwoChannel_CheckedChanged(null, null);

            dtaEmpfangstelegrammAufgeschluesselt.Rows.Clear();
            if (!string.IsNullOrEmpty(Settings.Default.RecieveTelegramm))
            {
                try
                {
                    DataRowCollection rows = DataSetConverter.ToDataSet(Settings.Default.RecieveTelegramm).Tables[0].Rows;
                    for (int n = 0; n < rows.Count - 1; n++)
                    {
                        dtaEmpfangstelegrammAufgeschluesselt.Rows.Add(new object[] { rows[n][0], rows[n][1] });
                    }
                }
                catch (Exception)
                { }
            }

            dtaSendTabelle.Rows.Clear();
            if (!string.IsNullOrEmpty(Settings.Default.SendeTelegramm))
            {
                try
                {
                    DataRowCollection rows = DataSetConverter.ToDataSet(Settings.Default.SendeTelegramm).Tables[0].Rows;
                    for (int n = 0; n < rows.Count - 1; n++)
                    {
                        if (rows[n].ItemArray.Length > 2)
                            dtaSendTabelle.Rows.Add(new object[] {rows[n][0], rows[n][1], rows[n][2]});
                        else
                            dtaSendTabelle.Rows.Add(new object[] {rows[n][0], rows[n][1]});
                    }
                }
                catch (Exception)
                { }
            }

            FillQuitt();

            lstStoredSenddata.Items.Clear();
            if (Settings.Default.SendDataList != null)
                lstStoredSenddata.Items.AddRange(Settings.Default.SendDataList.ToArray());
        }

        private void Prüfen()
        {
            bool gekuerzt = false;

            Int32 Counter = 0;
            foreach (DataGridViewRow row in dtaSendTabelle.Rows)
            {
                if (row.Cells["Laenge"].Value != null)
                {
                    Counter = Counter + Convert.ToInt32(row.Cells["Laenge"].Value.ToString());

                    if (row.Cells["Wert"].Value != null && row.Cells["Wert"].Value.ToString().Length > Convert.ToInt32(row.Cells["Laenge"].Value.ToString()))
                    {
                        row.Cells["Wert"].Value = row.Cells["Wert"].Value.ToString().Substring(0, Convert.ToInt32(row.Cells["Laenge"].Value.ToString()));
                        gekuerzt = true;
                    }
                }
            }

            if (gekuerzt)
                throw new Exception("Manche Felder waren zu lang und wurden gekürzt!");
        }

        private String SendeStringZusammenbauen()
        {
            String Ausgabe = "";

            foreach (DataGridViewRow row in dtaSendTabelle.Rows)
            {
                if (row.Cells["Laenge"].Value != null)
                {
                    string wrt = "";
                    if (row.Cells["Wert"].Value != null)
                        wrt = row.Cells["Wert"].Value.ToString();
                    if (wrt == null) wrt = "";

                    Ausgabe += wrt.FixSize(Convert.ToInt32(row.Cells["Laenge"].Value.ToString()), ' ');
                }

            }
            return Ausgabe;
        }

        private byte[] LaufnummerErzeugen()
        {
            int anz = Convert.ToInt32(Settings.Default.SequenceNumberLength);

            if (Settings.Default.Laufnummer > Convert.ToInt32("".PadLeft(anz, '9')))
                Settings.Default.Laufnummer = 1;

            return Encoding.ASCII.GetBytes(Settings.Default.Laufnummer.ToString().PadLeft(anz, '0'));
        }

        private void myTCP_ConnectionEstablished(int Number)
        {
            if (Number == 1)
                picConnection1.BackColor = Color.Green;
            else
                picConnection2.BackColor = Color.Green;
        }

        private void myTCP_TelegrammRecievedSend(byte[] telegramm, TcpClient clnt)
        {
            dtaSendQuittTable.Rows.Add(new object[] {Encoding.ASCII.GetString(telegramm)});
        }

        private byte[] oldSequenceNumber;

        private void myTCP_TelegrammRecievedRecieve(byte[] telegramm, TcpClient clnt)
        {
            if (quittConf == null || !quittConf.AutomaticQuitt)
            {
                //Automaticly send a quitt Telegramm
                byte[] quittTel = new byte[telegramm.Length];
                Array.Copy(telegramm, quittTel, telegramm.Length);

                foreach (var quittReplacmentByte in quittConf.QuittReplacmentBytes)
                {
                    Array.Copy(Encoding.ASCII.GetBytes(quittReplacmentByte.Value), 0, quittTel, quittReplacmentByte.Position, quittReplacmentByte.Value.Length);
                }
                myTCP_rec.SendData(quittTel);


                //Neues telegramm nur wenn keine WDH aktiv!
                //byte[] sequNr = new byte[quittConf.LengthSequenceNumber];
                //Array.Copy(telegramm, quittConf.SequenceNumberPosition, sequNr, 0, quittConf.LengthSequenceNumber);

                //if (oldSequenceNumber == null || !sequNr.ByteArrayCompare(oldSequenceNumber))
                grdEmpfang.Rows.Add(new object[] {Encoding.ASCII.GetString(telegramm)});
                //oldSequenceNumber = sequNr;
            }
            else
                grdEmpfang.Rows.Add(new object[] {Encoding.ASCII.GetString(telegramm)});
        }

        private void Disconnect()
        {
            picConnection1.BackColor = Color.Red;
            picConnection2.BackColor = Color.Red;

            if (myTCP_send != null)
            {
                myTCP_send.Dispose();
                myTCP_send = null;
            }

            if (myTCP_rec != null)
            {
                myTCP_rec.Dispose();
                myTCP_rec = null;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        private void grdEmpfang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var value = grdEmpfang.Rows[e.RowIndex].Cells[0].Value;
                if (value != null)
                {
                    string sValue = (string) value;
                    int pos = 0;
                    foreach (DataGridViewRow row in dtaEmpfangstelegrammAufgeschluesselt.Rows)
                    {
                        var tmp = row.Cells["colEmpfLaenge"].Value;
                        if (tmp != null)
                        {
                            int len = Convert.ToInt32(tmp);
                            if (pos + len <= sValue.Length)
                                row.Cells["colEmpfWert"].Value = sValue.Substring(pos, len);
                            pos += len;
                        }

                    }
                }
            }
        }

        private void cmdSettingsSave_Click(object sender, EventArgs e)
        {
            Settings.Default.RecieveTelegramm = DataSetConverter.ToString(DataSetConverter.DatagridviewToDataset(dtaEmpfangstelegrammAufgeschluesselt));
            Settings.Default.SendeTelegramm = DataSetConverter.ToString(DataSetConverter.DatagridviewToDataset(dtaSendTabelle));

            Settings.Default.Save();
        }

        private void cmdDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void optTwoChannel_CheckedChanged(object sender, EventArgs e)
        {
            lblKanal2.Enabled = optTwoChannel.Checked;
            chkChanel2active.Enabled = optTwoChannel.Checked;
            numPort2.Enabled = optTwoChannel.Checked;
            lblKanal2Port.Enabled = optTwoChannel.Checked;
        }

        /*
        public class MyCollectionEditor : CollectionEditor
        {
            private Type m_itemType = null;

            public MyCollectionEditor(Type type)
                : base(type)
            {
                m_itemType = type;
            }

            protected override CollectionForm CreateCollectionForm()
            {
                Button buttonLoadItem = new Button();
                buttonLoadItem.Text = "Load from DB";
                buttonLoadItem.Click += new EventHandler(ButtonLoadItem_Click);

                m_collectionForm = base.CreateCollectionForm();

                TableLayoutPanel panel1 = m_collectionForm.Controls[0] as TableLayoutPanel;
                TableLayoutPanel panel2 = panel1.Controls[1] as TableLayoutPanel;
                panel2.Controls.Add(buttonLoadItem);

                return m_collectionForm;
            }

            private void ButtonLoadItem_Click(object sender, EventArgs e)
        {
                if (m_itemType.Equals(typeof(MyCustomCollection)))
                {                               
                        MyCustomItem item = ...load from DB...

                        //definition: SetItems(object editValue, object[] value);
                        SetItems( -> what goes here?! <- , new object[] { item });
                }
        }
        }
        */

        public class CustomCollectionEditor<T> : CollectionEditor
        {
            public CustomCollectionEditor()
                : base(typeof (List<T>))
            {
            }

            protected override CollectionForm CreateCollectionForm()
            {
                Button buttonLoadItem = new Button();
                buttonLoadItem.Text = "Load from DB";
                buttonLoadItem.Click += new EventHandler(ButtonLoadItem_Click);

                var m_collectionForm = base.CreateCollectionForm();

                TableLayoutPanel panel1 = m_collectionForm.Controls[0] as TableLayoutPanel;
                TableLayoutPanel panel2 = panel1.Controls[1] as TableLayoutPanel;
                panel2.Controls.Add(buttonLoadItem);

                return m_collectionForm;
            }

            private void ButtonLoadItem_Click(object sender, EventArgs e)
            {

            }

            public void ShowDialog()
            {
                CollectionForm frm = CreateCollectionForm();
                try
                {
                    //MethodInfo methodInfo = frm.GetType().GetMethod("AddItems", BindingFlags.NonPublic | BindingFlags.Instance);
                    //methodInfo.Invoke(frm, new object[] {Settings.Default.QuittData});
                }
                catch (Exception)
                {
                }
                frm.ShowDialog();
            }
        }


        private void cmdSend_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            try
            {
                Prüfen();
            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message;
                return;
            }

            string SendeString = SendeStringZusammenbauen();

            byte[] bytes = Encoding.ASCII.GetBytes(SendeString);

            byte[] lnr = LaufnummerErzeugen();

            try
            {
                Array.Copy(lnr, 0, bytes, Convert.ToInt32(Settings.Default.SequenceNumberPosition), lnr.Length);
            }
            catch (Exception)
            {
            }


            

            try
            {
                if (myTCP_send != null)
                {
                    myTCP_send.SendData(bytes);
                    dtaSendSendTable.Rows.Add(new object[] { Encoding.ASCII.GetString(bytes) });
                }
                else
                    lblStatus.Text = "Senden nicht erfolgt, da nicht verbunden!";
            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message;
            }

            Settings.Default.Laufnummer++;

            var value = Encoding.ASCII.GetString(bytes);
            if (value != null)
            {
                string sValue = (string) value;
                int pos = 0;
                foreach (DataGridViewRow row in dtaSendTabelle.Rows)
                {
                    var tmp = row.Cells["Laenge"].Value;
                    if (tmp != null)
                    {
                        int len = Convert.ToInt32(tmp);
                        if (pos + len <= sValue.Length)
                            row.Cells["Wert"].Value = sValue.Substring(pos, len);
                        pos += len;
                    }

                }
            }
        }

        private void CountBytes()
        {
            int Counter = 0;

            foreach (DataGridViewRow row in dtaSendTabelle.Rows)
            {
                if (row.Cells["Laenge"].Value != null)
                {
                    Counter = Counter + Convert.ToInt32(row.Cells["Laenge"].Value.ToString());
                }
            }

            lblTeleLength.Text = "Telegrammlänge: " + Counter.ToString() + " Bytes";
        }

        private void dtaSendTabelle_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            CountBytes();
        }

        private void dtaSendTabelle_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CountBytes();
        }

        private void grdEmpfang_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            grdEmpfang_CellClick(sender, e);
        }

        private void cmdSelectStep7UDT_Click(object sender, EventArgs e)
        {
            S7DataBlock myDB = DotNetSiemensPLCToolBoxLibrary.Projectfiles.SelectProjectPart.SelectUDT();

            if (myDB != null)
            {
                var myLst = S7DataRow.GetChildrowsAsList(myDB.GetArrayExpandedStructure(new S7DataBlockExpandOptions() {ExpandCharArrays = false}));

                dtaSendTabelle.Rows.Clear();
                dtaEmpfangstelegrammAufgeschluesselt.Rows.Clear();

                foreach (var itm in myLst)
                {
                    if (itm.DataType != S7DataRowType.STRUCT && itm.DataType != S7DataRowType.UDT)
                    {
                        dtaSendTabelle.Rows.Add(new object[] {itm.StructuredName, itm.ByteLength});
                        dtaEmpfangstelegrammAufgeschluesselt.Rows.Add(new object[] {itm.StructuredName, itm.ByteLength});
                    }
                }
            }
        }

        private TCPFunctions.QuittConfig quittConf;

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            if (myTCP_send != null)
            {
                myTCP_send.Dispose();
            }

            if (myTCP_rec != null)
            {
                myTCP_rec.Dispose();
            }

            myTCP_send = null;
            myTCP_rec = null;


            picConnection1.BackColor = Color.Orange;
            picConnection2.BackColor = Color.Orange;

            quittConf = new TCPFunctions.QuittConfig();
            quittConf.QuittReplacmentBytes.AddRange(Settings.Default.QuittData.ToArray());
            quittConf.SequenceNumberPosition = Convert.ToInt32(Settings.Default.SequenceNumberPosition);
            quittConf.LengthSequenceNumber = Convert.ToInt32(Settings.Default.SequenceNumberLength);
            quittConf.AutomaticQuitt = Settings.Default.AutomaticQuitt;

            myTCP_send = new TCPFunctionsAsync(SynchronizationContext.Current, Settings.Default.SendConnectionActive ? IPAddress.Parse(Settings.Default.IPAddress) : null, Convert.ToInt32(Settings.Default.SendPort), Settings.Default.SendConnectionActive);
            myTCP_send.DataRecieved += myTCP_TelegrammRecievedSend;
            myTCP_send.ConnectionEstablished += (c) => myTCP_ConnectionEstablished(1);
            myTCP_send.ConnectionClosed += (c) => myTCP_ConnectionClosed(1);
            myTCP_send.Start();
                        

            if (!Settings.Default.UseOnlyOneConnection)
            {
                myTCP_rec = new TCPFunctionsAsync(SynchronizationContext.Current, Settings.Default.RecieveConnectionActive ? IPAddress.Parse(Settings.Default.IPAddress) : null, Convert.ToInt32(Settings.Default.RecievePort), Settings.Default.RecieveConnectionActive);
                myTCP_rec.DataRecieved += myTCP_TelegrammRecievedRecieve;
                myTCP_rec.ConnectionEstablished += (c) => myTCP_ConnectionEstablished(2);
                myTCP_rec.ConnectionClosed += (c) => myTCP_ConnectionClosed(2);
                myTCP_rec.Start();                               
            }
            else
            {
                myTCP_send.DataRecieved += myTCP_TelegrammRecievedRecieve;
            }
        }

        private void myTCP_ConnectionClosed(int Number)
        {
            if (Number == 1)
                picConnection1.BackColor = Color.Orange;
            else
                picConnection2.BackColor = Color.Orange;
        }

        [Serializable()]
        public class storeSendData
        {
            //This is neccessary, that the spaces stay
            [XmlAttribute("xml:space")] public String SpacePreserve = "preserve";

            public string Name { get; set; }
            public string Telegramm { get; set; }

            public override string ToString()
            {
                return Name + " [" + Telegramm + "]";
            }
        }

        private void cmdAddSendeTele_Click(object sender, EventArgs e)
        {
            string telnm = "";
            DialogResult tmp = InputBox.Show("Speichern", "Speichern des Telegrammes :", ref telnm);
            if (tmp == DialogResult.OK)
            {
                string SendeString = SendeStringZusammenbauen();

                Settings.Default.SendDataList = Settings.Default.SendDataList ?? new List<storeSendData>();
                Settings.Default.SendDataList.Add(new storeSendData() {Name = telnm, Telegramm = SendeString});

                lstStoredSenddata.Items.Clear();
                lstStoredSenddata.Items.AddRange(Settings.Default.SendDataList.ToArray());
            }
        }

        private void cmdRemoveSendeTele_Click(object sender, EventArgs e)
        {
            if (lstStoredSenddata.SelectedItem != null)
            {
                Settings.Default.SendDataList.Remove((storeSendData) lstStoredSenddata.SelectedItem);

                lstStoredSenddata.Items.Clear();
                lstStoredSenddata.Items.AddRange(Settings.Default.SendDataList.ToArray());
            }
        }

        private void lstStoredSenddata_DoubleClick(object sender, EventArgs e)
        {
            if (lstStoredSenddata.SelectedItem != null)
            {

                var value = ((storeSendData) lstStoredSenddata.SelectedItem).Telegramm;
                if (value != null)
                {
                    string sValue = (string) value;
                    int pos = 0;
                    foreach (DataGridViewRow row in dtaSendTabelle.Rows)
                    {
                        var tmp = row.Cells["Laenge"].Value;
                        if (tmp != null)
                        {
                            int len = Convert.ToInt32(tmp);
                            if (pos + len <= sValue.Length)
                                row.Cells["Wert"].Value = sValue.Substring(pos, len);
                            pos += len;
                        }

                    }
                }
            }
        }

        private void cmdSettExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sav = new SaveFileDialog();
            sav.Filter = "*.settings|*.settings";
            DialogResult ret = sav.ShowDialog();
            if (ret == DialogResult.OK)
            {
                CustomSettingsProvider.SettingsFileName = sav.FileName;
                Settings.Default.Save();
                CustomSettingsProvider.SettingsFileName = CustomSettingsProvider.OriginalSettingsFileName;
            }
        }

        private void cmdSettImport_Click(object sender, EventArgs e)
        {

            OpenFileDialog opn = new OpenFileDialog();
            opn.Filter = "*.settings|*.settings";
            DialogResult ret = opn.ShowDialog();
            if (ret == DialogResult.OK)
            {
                Disconnect();

                CustomSettingsProvider.SettingsFileName = opn.FileName;

                Settings.Default.Reload();
                
                LoadSettings();

                CustomSettingsProvider.SettingsFileName = CustomSettingsProvider.OriginalSettingsFileName;
            }
        }

        private void FillQuitt()
        {
            lstQuitt.Items.Clear();
            if (Properties.Settings.Default.QuittData != null)
                lstQuitt.Items.AddRange(Properties.Settings.Default.QuittData.ToArray());
            else
                Properties.Settings.Default.QuittData = new List<TCPFunctions.QuittConfig.QuittText>();
        }


        private void cmdEditQuittFields_Click(object sender, EventArgs e)
        {
            string quitttxt = "", quitpos = "0";
            DialogResult ret1 = InputBox.Show("Quittungstext", "Text der ins Quittungstelegramm eingefügt werden soll:", ref quitttxt);
            DialogResult ret2 = InputBox.Show("Quittungstext", "Position an der der Text eingefügt werden soll", ref quitpos);
            if (ret1 == DialogResult.OK && ret2 == DialogResult.OK)
            {

                int pos = 0;
                int.TryParse(quitpos, out pos);
                Properties.Settings.Default.QuittData.Add(new TCPFunctions.QuittConfig.QuittText(pos, quitttxt));
                FillQuitt();
            }

            /*
            CustomCollectionEditor<TCPFunctions.QuittConfig.QuittText> colEdt = new CustomCollectionEditor<TCPFunctions.QuittConfig.QuittText>();
            colEdt.EditValue(null, Settings.Default.QuittData);
            colEdt.ShowDialog();
            //Properties.Settings.Default.QuittData=new List<TCPFunctions.QuittConfig.QuittText>();
            //System.ComponentModel.Design.CollectionEditor colEdt = new CollectionEditor(Properties.Settings.Default.QuittData.GetType());
            //colEdt.EditValue(null, Properties.Settings.Default.QuittData);
            */

        }

        private void cmdDelTele_Click(object sender, EventArgs e)
        {
            if (lstQuitt.SelectedItem != null)
            {
                Properties.Settings.Default.QuittData.Remove((TCPFunctions.QuittConfig.QuittText) lstQuitt.SelectedItem);
                FillQuitt();
            }
        }       

        private void dtaSendQuittTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var value = dtaSendQuittTable.Rows[e.RowIndex].Cells[0].Value;
                lblLenEmpf.Text = value.ToString().Length.ToString();
            }
        }

        private void dtaSendSendTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            grdEmpfang.Rows.Clear();
        }

        private void cmdClearSend_Click(object sender, EventArgs e)
        {
            dtaSendSendTable.Rows.Clear();
            dtaSendQuittTable.Rows.Clear();
        }                
    }
}
