using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;

using JFKCommonLibrary.Forms;

using MessageBox = System.Windows.MessageBox;

namespace BackupRestoreBlocks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*     
                
                myConn.Connect();
                timer.Enabled = true;
         */

        private PLCConnection myConn;

        public MainWindow()
        {
            InitializeComponent();

            lblFolder.Text = Environment.CurrentDirectory;
            LoadFiles();

            myConn = new PLCConnection("BackupRestoreBlocks");
            lblConnection.Content = myConn.Configuration.ToString();
        }

        private void cmdConfig_Click(object sender, RoutedEventArgs e)
        {
             Configuration.ShowConfiguration("BackupRestoreBlocks", true);

             myConn = new PLCConnection("BackupRestoreBlocks");
             lblConnection.Content = myConn.Configuration.ToString();
        }

        private void cmdLoadBlockList_Click(object sender, RoutedEventArgs e)
        {
            myConn.Connect();
            var blks = myConn.PLCListBlocks(PLCBlockType.AllEditableBlocks);
            lstBlocks.ItemsSource = blks;
            myConn.Disconnect();
        }

        private void cmdDownload_Click(object sender, RoutedEventArgs e)
        {
            string fldname = lblFolder.Text;

            myConn.Connect();

            foreach (string selectedBlock in lstBlocks.SelectedItems)
            {
                var blk = myConn.PLCGetBlockInMC7(selectedBlock);

                string file = Path.Combine(fldname, selectedBlock + ".blk");
                BinaryWriter wrt = new BinaryWriter(File.Open(file, FileMode.Create));

                wrt.Write(blk);
                wrt.Close();

                LoadFiles();
            }

            myConn.Disconnect();
        }

        private void cmdBrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.SelectedPath = lblFolder.Text;
            dlg.ShowDialog();
            lblFolder.Text = dlg.SelectedPath;

            LoadFiles();
        }

        private void LoadFiles()
        {
            var files = Directory.GetFiles(lblFolder.Text,"*.blk");

            List<string> fileNames = new List<string>();
            foreach (var file in files)
            {
                fileNames.Add(Path.GetFileName(file));
            }
            lstFiles.ItemsSource = fileNames;
        }

        private void cmdRefreshFolder_Click(object sender, RoutedEventArgs e)
        {
            this.LoadFiles();
        }

        private void cmdUpload_Click(object sender, RoutedEventArgs e)
        {
            string fldname = lblFolder.Text;

            myConn.Connect();

            foreach (string selectedFile in lstFiles.SelectedItems)
            {
                var fileName = Path.Combine(fldname, selectedFile);
                var blockName = selectedFile.Substring(0, selectedFile.Length - 4);

                var rd = new BinaryReader(File.Open(fileName, FileMode.Open));
                var bytes = rd.ReadBytes(Convert.ToInt32(rd.BaseStream.Length));

                myConn.PLCPutBlockFromMC7toPLC(blockName, bytes);
            }   

            myConn.Disconnect();
        }

        private void cmdUploadRename_Click(object sender, RoutedEventArgs e)
        {
            
            if (lstFiles.SelectedItem == null || lstFiles.SelectedItems.Count > 1)
            {
                MessageBox.Show("Plese Select a Block/only one Block");
                return;
            }

            string fldname = lblFolder.Text;

            var selectedFile = lstFiles.SelectedItem.ToString();
            var fileName = Path.Combine(fldname, selectedFile);
            var blockName = selectedFile.Substring(0, selectedFile.Length - 4);

            var blkName = InputBox.ShowWithStringResult("Upload Block (" + blockName + ")", "New Block Name:", blockName);

            if (string.IsNullOrEmpty(blkName))
                return;

            myConn.Connect();
            
            var rd = new BinaryReader(File.Open(fileName, FileMode.Open));
            var bytes = rd.ReadBytes(Convert.ToInt32(rd.BaseStream.Length));

            //var aa = MC7Converter.GetAWLBlock(bytes, 0);
            myConn.PLCPutBlockFromMC7toPLC(blkName, bytes);

            myConn.Disconnect();
        }


    }
}
