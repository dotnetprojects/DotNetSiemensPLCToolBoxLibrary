using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace WPFToolboxForSiemensPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class ContentWindowWinCCTagVarCreator : DocumentContent
    {
        public ContentWindowWinCCTagVarCreator()
        {
            InitializeComponent();
            ConvertBlocks = new ObservableCollection<ProjectBlockInfo>();
            this.DataContext = this;
            myBlockList.ItemsSource = ConvertBlocks;
        }

        public ObservableCollection<ProjectBlockInfo> ConvertBlocks { get; set; }

        private void ListBox_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent("ProjectBlockInfo"))
                e.Effects = DragDropEffects.All;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("ProjectBlockInfo"))
                return;

            ProjectBlockInfo blkInf = (ProjectBlockInfo)e.Data.GetData("ProjectBlockInfo");
            if (blkInf.BlockType == PLCBlockType.DB)
                ConvertBlocks.Add(blkInf);
            else
                App.clientForm.lblStatus.Text = "Only DB's can be converted to Tags or Errors!";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string errors = "";
            string tags = "";

            foreach (var projectBlockInfo in ConvertBlocks)
            {
                S7DataBlock myDB = (S7DataBlock)projectBlockInfo.GetBlock();

                int cnt = 0;

                if (myDB.Structure != null && myDB.Structure.Children != null)
                    cnt = myDB.Structure.Children[myDB.Structure.Children.Count - 1].NextBlockAddress.ByteAddress;

                string varname = "STOERUNGEN_DB" + myDB.BlockNumber;

                tags += varname + ";" + txtConnectionName.Text + ";DB " + myDB.BlockNumber + " DBW 0;Int;;" +
                              ((cnt - 2) / 2).ToString() + ";2;1 s;;;;;0;10;0;100;0;;0;\r\n";

                int errNr = Convert.ToInt32(txtStartErrorNumber.Text);

                foreach (S7DataRow plcDataRow in S7DataRow.GetChildrowsAsList(myDB.Structure))
                // myDB.GetRowsAsList())
                {
                    if (plcDataRow.DataType == S7DataRowType.BOOL)
                    {
                        ByteBitAddress akAddr = plcDataRow.BlockAddress;

                        int bitnr = (akAddr.ByteAddress / 2) * 16 + akAddr.BitAddress; //akAddr.BitAddress;
                        if (akAddr.ByteAddress % 2 == 0)
                            bitnr += 8;

                        string stoeTxt = "";
                        string stoeTxtEn = "";

                        stoeTxt = plcDataRow.Comment;
                        if (stoeTxt.Contains(";"))
                        {
                            stoeTxt = "Störort: " + stoeTxt.Split(';')[0] + ", " + stoeTxt.Split(';')[1];
                        }

                        if (chkFixedErrorNumber.IsChecked.Value)
                            errNr = Convert.ToInt32(txtStartErrorNumber.Text) + akAddr.ByteAddress * 8 + akAddr.BitAddress;
                        errors += "\"D\"\t\"" + errNr.ToString() + "\"\t\"Alarms\"\t\"" + varname + "\"\t\"" +
                                  bitnr.ToString() + "\"\t\t\t\t\t\t\"0\"\t\"de-DE=" + stoeTxt + "\"\t\"en-US=" +
                                  stoeTxtEn + "\"\t\"de-DE=\"" + "\r\n";
                        if (!chkFixedErrorNumber.IsChecked.Value)
                            errNr++;
                    }
                }
            }

            FolderBrowserDialog fldDlg = null;

            fldDlg = new FolderBrowserDialog();
            fldDlg.Description = "Destination Diretory for Alarms.csv and Tags.csv!";
            if (fldDlg.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter swr;

                swr = new System.IO.StreamWriter(fldDlg.SelectedPath + "\\Alarms.csv");
                swr.Write(errors);
                swr.Close();

                swr = new System.IO.StreamWriter(fldDlg.SelectedPath + "\\Tags.csv");
                swr.Write(tags.Replace(";", "\t"));
                swr.Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            HMIGENOBJECTSLib.HMIGO HMIGOObject = null;
            try
            {
                HMIGOObject = new HMIGENOBJECTSLib.HMIGO();
            }
            catch (Exception ex)
            {
                MessageBox.Show("The WinCC Object could not be created!\n\n Error:" + ex.Message);
            }

            try
            {
                foreach (var projectBlockInfo in ConvertBlocks)
                {
                    S7DataBlock myDB = (S7DataBlock)projectBlockInfo.GetBlock();


                    int cnt = 0;

                    if (myDB.Structure != null && myDB.Structure.Children != null)
                        cnt = myDB.Structure.Children[myDB.Structure.Children.Count - 1].NextBlockAddress.ByteAddress;

                    string varname = "STOERUNGEN_DB" + myDB.BlockNumber;

                    for (int n = 0; n < cnt / 2; n++)
                    {
                        try
                        {
                            HMIGOObject.CreateTag(varname + "_" + (n + 1).ToString(),
                                                  HMIGENOBJECTSLib.HMIGO_TAG_TYPE.TAG_UNSIGNED_16BIT_VALUE,
                                                  txtConnectionName.Text,
                                                  "DB" + myDB.BlockNumber + ",DD" + (n * 2).ToString(), "Stoerungen");
                        }
                        catch (COMException ex)
                        {
                            if (ex.ErrorCode != -2147196408)
                                throw ex;
                        }

                    }

                    string errors = "";

                    int errNr = Convert.ToInt32(txtStartErrorNumber.Text);

                    foreach (S7DataRow plcDataRow in S7DataRow.GetChildrowsAsList(myDB.Structure))
                    // myDB.GetRowsAsList())
                    {
                        if (plcDataRow.DataType == S7DataRowType.BOOL)
                        {
                            ByteBitAddress akAddr = plcDataRow.BlockAddress;
                            int varnr = (akAddr.ByteAddress / 2) + 1;

                            int bitnr = akAddr.BitAddress;
                            if (akAddr.ByteAddress % 2 == 0)
                                bitnr += 8;

                            string stoeTxt = "";
                            string stoeOrt = "";
                            string stoeTxtEn = "";

                            stoeTxt = plcDataRow.Comment;
                            if (stoeTxt.Contains(";"))
                            {
                                stoeTxt = stoeTxt.Split(';')[1];
                                stoeOrt = stoeTxt.Split(';')[0];
                            }

                            if (chkFixedErrorNumber.IsChecked.Value)
                                errNr = Convert.ToInt32(txtStartErrorNumber.Text) + akAddr.ByteAddress * 8 +
                                        akAddr.BitAddress;

                            try
                            {
                                HMIGOObject.CreateSingleAlarm(errNr,
                                                              HMIGENOBJECTSLib.HMIGO_SINGLE_ALARM_CLASS_ID.
                                                                  SINGLE_ALARM_ERROR, 1, stoeTxt,
                                                              varname + "_" + varnr.ToString(), bitnr);
                                HMIGOObject.SingleAlarmInfoText = stoeTxt;
                                HMIGOObject.SingleAlarmText2ID = stoeOrt;
                                HMIGOObject.CommitSingleAlarm();
                            }
                            catch (System.Runtime.InteropServices.COMException ex)
                            {
                                if (ex.ErrorCode != -2147467259)
                                    throw ex;
                            }

                            //errors += "\"D\"\t\"" + errNr.ToString() + "\"\t\"Alarms\"\t\"" + varname + "\"\t\"" + bitnr.ToString() + "\"\t\t\t\t\t\t\"0\"\t\"de-DE=" + stoeTxt + "\"\t\"en-US=" + stoeTxtEn + "\"\t\"de-DE=\"" + "\r\n";
                            if (!chkFixedErrorNumber.IsChecked.Value)
                                errNr++;
                        }
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                if (ex.ErrorCode == -2147195889)
                    MessageBox.Show("Error: The Connection Name you specified does not exist!");
                else
                    MessageBox.Show("Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            HMIGENOBJECTSLib.HMIGO HMIGOObject = null;
            try
            {
                HMIGOObject = new HMIGENOBJECTSLib.HMIGO();
            }
            catch (Exception ex)
            {
                MessageBox.Show("The WinCC Object could not be created!\n\n Error:" + ex.Message);
            }

            foreach (var projectBlockInfo in ConvertBlocks)
            {
                S7DataBlock myDB = (S7DataBlock)projectBlockInfo.GetBlock();

                List<S7DataRow> myLst = null;
                if (chkExpandArrays.IsChecked.Value)
                    myLst =
                        S7DataRow.GetChildrowsAsList(myDB.GetArrayExpandedStructure(new S7DataBlockExpandOptions()));
                // ) myDB.GetRowsAsArrayExpandedList(ne);
                else
                    myLst = S7DataRow.GetChildrowsAsList(myDB.Structure); // myDB.GetRowsAsList();

                int cnt = 0;

                try
                {
                    foreach (S7DataRow plcDataRow in myLst)
                    {
                        string tagName = txtTagsPrefix.Text +
                                         plcDataRow.StructuredName.Replace(".", "_").Replace("[", "_").Replace("]", "").
                                             Replace(" ", "").Replace(",", "_");
                        try
                        {
                            switch (plcDataRow.DataType)
                            {
                                case S7DataRowType.BOOL:
                                    HMIGOObject.CreateTag(tagName, HMIGENOBJECTSLib.HMIGO_TAG_TYPE.TAG_BINARY_TAG,
                                                          txtConnectionName.Text,
                                                          "DB" + myDB.BlockNumber + ",D" +
                                                          plcDataRow.BlockAddress.ToString(),
                                                          "TAGS_DB" + myDB.BlockNumber);
                                    break;
                                case S7DataRowType.INT:
                                    HMIGOObject.CreateTag(tagName,
                                                          HMIGENOBJECTSLib.HMIGO_TAG_TYPE.TAG_SIGNED_16BIT_VALUE,
                                                          txtConnectionName.Text,
                                                          "DB" + myDB.BlockNumber + ",DW" +
                                                          plcDataRow.BlockAddress.ByteAddress.ToString(),
                                                          "TAGS_DB" + myDB.BlockNumber);
                                    break;
                                case S7DataRowType.DINT:
                                    HMIGOObject.CreateTag(tagName,
                                                          HMIGENOBJECTSLib.HMIGO_TAG_TYPE.TAG_SIGNED_32BIT_VALUE,
                                                          txtConnectionName.Text,
                                                          "DB" + myDB.BlockNumber + ",DD" +
                                                          plcDataRow.BlockAddress.ByteAddress.ToString(),
                                                          "TAGS_DB" + myDB.BlockNumber);
                                    break;
                                case S7DataRowType.WORD:
                                    HMIGOObject.CreateTag(tagName,
                                                          HMIGENOBJECTSLib.HMIGO_TAG_TYPE.TAG_UNSIGNED_16BIT_VALUE,
                                                          txtConnectionName.Text,
                                                          "DB" + myDB.BlockNumber + ",DW" +
                                                          plcDataRow.BlockAddress.ByteAddress.ToString(),
                                                          "TAGS_DB" + myDB.BlockNumber);
                                    break;
                                case S7DataRowType.DWORD:
                                    HMIGOObject.CreateTag(tagName,
                                                          HMIGENOBJECTSLib.HMIGO_TAG_TYPE.TAG_UNSIGNED_32BIT_VALUE,
                                                          txtConnectionName.Text,
                                                          "DB" + myDB.BlockNumber + ",DD" +
                                                          plcDataRow.BlockAddress.ByteAddress.ToString(),
                                                          "TAGS_DB" + myDB.BlockNumber);
                                    break;
                                case S7DataRowType.BYTE:
                                    HMIGOObject.CreateTag(tagName,
                                                          HMIGENOBJECTSLib.HMIGO_TAG_TYPE.TAG_UNSIGNED_8BIT_VALUE,
                                                          txtConnectionName.Text,
                                                          "DB" + myDB.BlockNumber + ",DBB" +
                                                          plcDataRow.BlockAddress.ByteAddress.ToString(),
                                                          "TAGS_DB" + myDB.BlockNumber);
                                    break;
                                case S7DataRowType.REAL:
                                    HMIGOObject.CreateTag(tagName,
                                                          HMIGENOBJECTSLib.HMIGO_TAG_TYPE.
                                                              TAG_FLOATINGPOINT_NUMBER_32BIT_IEEE_754,
                                                          txtConnectionName.Text,
                                                          "DB" + myDB.BlockNumber + ",DD" +
                                                          plcDataRow.BlockAddress.ByteAddress.ToString(),
                                                          "TAGS_DB" + myDB.BlockNumber);
                                    break;
                            }
                        }
                        catch (System.Runtime.InteropServices.COMException ex)
                        {
                            if (ex.ErrorCode != -2147196408)
                                throw ex;
                            //Tag existiert schoin                            
                        }
                    }
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    if (ex.ErrorCode == -2147195889)

                        MessageBox.Show("Error: The Connection Name you specified does not exist!");
                    else
                        MessageBox.Show("Error: " + ex.Message);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return;
                }
            }
        }
    }
}
