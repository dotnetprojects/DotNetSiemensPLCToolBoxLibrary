using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using DotNetSiemensPLCToolBoxLibrary;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace JFK_VarTab
{
    public partial class Vartab : Form
    {
        private PLCConnection myConn;
        public Vartab()
        {
            InitializeComponent();
        }

        //List of the Rows in the VarTab
        //List<WPFToolboxForSiemensPLCs.LibNoDaveValue> myValues = new List<LibNoDaveValue>();
        private List<S7VATRow> myValues = new List<S7VATRow>();


        private void Vartab_Load(object sender, EventArgs e)
        {
            lstConnections.Items.Clear();
            lstConnections.Items.AddRange(PLCConnectionConfiguration.GetConfigurationNames());

            if (lstConnections.Items.Count > 0)
                lstConnections.SelectedItem = lstConnections.Items[0];      
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int nr = e.RowIndex;
                if (myValues.Count <= nr)
                    myValues.Add(new S7VATRow());
                    //myValues.Add(new LibNoDaveValue());

                if (myValues[myValues.Count - 1].LibNoDaveValue == null)
                    myValues[myValues.Count - 1].LibNoDaveValue = new PLCTag();

                if (dataGridViewVarTab.Rows[nr].Cells[e.ColumnIndex].Value != null)
                {
                    if (e.ColumnIndex == 0)
                        myValues[nr].LibNoDaveValue.ChangeAddressFromString(dataGridViewVarTab.Rows[nr].Cells[e.ColumnIndex].Value.ToString());
                        //myValues[nr].ChangeAddressFromString(dataGridViewVarTab.Rows[nr].Cells[e.ColumnIndex].Value.ToString());
                    else if (e.ColumnIndex == 1)
                        myValues[nr].LibNoDaveValue.ChangeDataTypeFromString(dataGridViewVarTab.Rows[nr].Cells[e.ColumnIndex].Value.ToString());
                        //myValues[nr].ChangeDataTypeFromString(dataGridViewVarTab.Rows[nr].Cells[e.ColumnIndex].Value.ToString());
                    else if (e.ColumnIndex == 2)
                        myValues[nr].LibNoDaveValue.ChangeDataTypeStringFormatFromString(dataGridViewVarTab.Rows[nr].Cells[e.ColumnIndex].Value.ToString());
                        //myValues[nr].ChangeDataTypeStringFormatFromString(dataGridViewVarTab.Rows[nr].Cells[e.ColumnIndex].Value.ToString());
                    else if (e.ColumnIndex == 4)
                        if (dataGridViewVarTab.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                            myValues[nr].LibNoDaveValue.ParseControlValueFromString(dataGridViewVarTab.Rows[nr].Cells[e.ColumnIndex].Value.ToString());
                            //myValues[nr].ParseControlValueFromString(dataGridViewVarTab.Rows[nr].Cells[e.ColumnIndex].Value.ToString());
                }

                dataGridViewVarTab.Rows[nr].Cells[0].Value = myValues[nr].LibNoDaveValue.S7FormatAddress;
                dataGridViewVarTab.Rows[nr].Cells[1].Value = myValues[nr].LibNoDaveValue.LibNoDaveDataType;
                dataGridViewVarTab.Rows[nr].Cells[2].Value = myValues[nr].LibNoDaveValue.DataTypeStringFormat;
                dataGridViewVarTab.Rows[nr].Cells[3].Value = myValues[nr].LibNoDaveValue.GetValueAsString();
                dataGridViewVarTab.Rows[nr].Cells[4].Value = myValues[nr].LibNoDaveValue.GetControlValueAsString();

                
                if (mySymtable != null)
                    foreach (var it in mySymtable.SymbolTableEntrys)
                    {
                        if (it.Operand.Trim().ToLower() == dataGridViewVarTab.Rows[nr].Cells[0].Value.ToString().Trim().ToLower())
                        {
                            dataGridViewVarTab.Rows[nr].Cells[5].Value = it.Symbol;
                        }
                    }
                 
            }
            catch (Exception)
            { }
        }

        private void refresh()
        {
            for (int nr = 0; nr < dataGridViewVarTab.Rows.Count - 1; nr++)
            {
                if (myValues[nr].LibNoDaveValue != null)
                {
                    dataGridViewVarTab.Rows[nr].Cells[0].Value = myValues[nr].LibNoDaveValue.S7FormatAddress;
                    dataGridViewVarTab.Rows[nr].Cells[1].Value = myValues[nr].LibNoDaveValue.LibNoDaveDataType;
                    dataGridViewVarTab.Rows[nr].Cells[2].Value = myValues[nr].LibNoDaveValue.DataTypeStringFormat;
                    dataGridViewVarTab.Rows[nr].Cells[3].Value = myValues[nr].LibNoDaveValue.GetValueAsString();
                    dataGridViewVarTab.Rows[nr].Cells[4].Value = myValues[nr].LibNoDaveValue.GetControlValueAsString();
                }
                //Symboltabelle
                
                if (mySymtable != null)
                    foreach (var it in mySymtable.SymbolTableEntrys)
                    {
                        string a = it.Operand.Trim().ToLower().Replace(" ", "");
                        string b = dataGridViewVarTab.Rows[nr].Cells[0].Value.ToString().Trim().ToLower().Replace(" ", "");
                        if (a == b)
                        {
                            dataGridViewVarTab.Rows[nr].Cells[5].Value = it.Symbol;
                        }
                    }
                 
            }
        }


        private List<PLCTag> getListVal()
        {
            List<PLCTag> retVal=new List<PLCTag>();
            foreach (S7VATRow myValue in myValues)
            {
                if (myValue.LibNoDaveValue != null)
                    retVal.Add(myValue.LibNoDaveValue);
            }
            return retVal;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (myConn != null)
                {
                    int wrt = progressBar1.Value + 33;
                    if (wrt >= 100)
                        progressBar1.Value = 0;
                    else
                        progressBar1.Value = wrt;
                    myConn.ReadValues(getListVal());
                    int i = 0;
                    foreach (var akVal in myValues)
                    {
                        if (akVal.LibNoDaveValue != null)
                            dataGridViewVarTab.Rows[i].Cells[3].Value = akVal.LibNoDaveValue.GetValueAsString();
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                errtxt.Text = ex.Message;
            }
        }


        private void cmdConfig_Click(object sender, EventArgs e)
        {
            errtxt.Text = "";

            var tmp = lstConnections.SelectedItem;
            
            Configuration.ShowConfiguration("Verbindung_1", false);
            
            lstConnections.Items.Clear();
            lstConnections.Items.AddRange(PLCConnectionConfiguration.GetConfigurationNames());

            if (tmp!=null && lstConnections.Items.Contains(tmp))
                lstConnections.SelectedItem = tmp;
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstConnections.SelectedItem != null)
                {
                    this.myConn = new PLCConnection(lstConnections.SelectedItem.ToString());
                    this.myConn.Connect();
                    cmdConnect.BackColor = Color.LightGreen;
                }
                else
                {
                    MessageBox.Show("Please Select the Connection Configuration wich i should use in the Listbox!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmdView_Click(object sender, EventArgs e)
        {
            
            errtxt.Text = "";
            if (myConn != null && myConn.Connected)
                timer1.Enabled = true;
        }

        private void cmdStopView_Click(object sender, EventArgs e)
        {
            errtxt.Text = "";
            timer1.Enabled = false;
            progressBar1.Value = 0;
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "*.vartab|*.vartab";
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                saveVal saveValueClass = new saveVal();
                saveValueClass.myValues = myValues;

                System.IO.FileStream jj = new FileStream(saveDlg.FileName, FileMode.Create);
                System.Xml.Serialization.XmlSerializer myXml = new XmlSerializer(typeof (saveVal));
                myXml.Serialize(jj, saveValueClass);
                jj.Close();
            }
        }

        //This class is there, because in the First Version the Control Values were not part of the LibNoDaveValue....
        [Serializable()]
        public class saveVal
        {
            public List<S7VATRow> myValues;
        }

        private void cmdLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "*.vartab|*.vartab";
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                dataGridViewVarTab.Rows.Clear();

                saveVal openValueClass;
                
                System.IO.FileStream jj = new FileStream(openDlg.FileName, FileMode.Open);
                System.Xml.Serialization.XmlSerializer myXml = new XmlSerializer(typeof(saveVal));
                openValueClass = (saveVal)myXml.Deserialize(jj);
                myValues = openValueClass.myValues;
                jj.Close();

                reload();
                //foreach (var libNoDaveValue in myValues)
                //    dataGridViewVarTab.Rows.Add(new object[] {libNoDaveValue.GetS7FormatAddress(), libNoDaveValue.LibNoDaveDataType, libNoDaveValue.DataTypeStringFormat, libNoDaveValue.GetValueAsString(), libNoDaveValue.GetControlValueAsString()});
            }
        }

        private void cmdControlValues_Click(object sender, EventArgs e)
        {
            try
            {
                errtxt.Text = "";
                if (myConn != null)
                {
                    int i = 0;
                    List<PLCTag> steu = new List<PLCTag>();
                    foreach (var row in myValues)
                    {
                        if (dataGridViewVarTab.Rows[i].Cells[4].Value != null && dataGridViewVarTab.Rows[i].Cells[4].Value.ToString() != "")
                        {
                            row.LibNoDaveValue.Controlvalue = dataGridViewVarTab.Rows[i].Cells[4].Value;
                            steu.Add(row.LibNoDaveValue);
                        }
                        i++;
                    }
                    myConn.WriteValues(steu);
                }
            }
            catch (Exception ex)
            {
                errtxt.Text = ex.Message;
            }
        }

        private void cmdDisconnect_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            progressBar1.Value = 0;
            myConn.Disconnect();
            cmdConnect.BackColor = cmdDisconnect.BackColor;
        }

      
        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            try
            {
                if (!reloadInProgress)
                    for (int n = 0; n < e.RowCount; n++)
                    {
                        myValues.RemoveAt(e.RowIndex);
                    }
            }
            catch (Exception)
            { }
        }

        
        private void cmdLoadVat_Click(object sender, EventArgs e)
        {            
            S7VATBlock tmpVat = SelectProjectPart.SelectVAT();
            if (tmpVat!=null)
            {
                
                myValues = tmpVat.VATRows;
                reload();
                mySymtable = (SymbolTable)((S7ProgrammFolder) tmpVat.ParentFolder.Parent).SymbolTable;
                refresh();
            }
        }

        private bool reloadInProgress = false;
        private void reload()
        {
            reloadInProgress = true;
            dataGridViewVarTab.Rows.Clear();
            foreach (var akVATRow in myValues)
            {
                var libNoDaveValue = akVATRow.LibNoDaveValue;
                if (libNoDaveValue != null)
                    dataGridViewVarTab.Rows.Add(new object[] { libNoDaveValue.S7FormatAddress, libNoDaveValue.LibNoDaveDataType, libNoDaveValue.DataTypeStringFormat, libNoDaveValue.GetValueAsString(), libNoDaveValue.GetControlValueAsString() });
                else if (!string.IsNullOrEmpty(akVATRow.Comment))
                    dataGridViewVarTab.Rows.Add(new object[] { "", "", "", "", "", "", akVATRow.Comment });
                else
                    dataGridViewVarTab.Rows.Add(new object[] { "", "", "", "", "", "", "" });
            }
            reloadInProgress = false;
        }

        private void lstConnections_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstConnections.SelectedItem != null)
                conninfo.Text = new PLCConnectionConfiguration((string)lstConnections.SelectedItem).ToString();
        }

        private SymbolTable mySymtable;

        private void cmdLoadSymboltable_Click(object sender, EventArgs e)
        {
            mySymtable = SelectProjectPart.SelectSymbolTable();
            refresh();            
        }

        private void dataGridViewVarTab_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

       
    }
}
