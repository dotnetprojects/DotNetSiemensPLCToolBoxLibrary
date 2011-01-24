using System;
using System.Collections.Generic;
using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.Communication.S7_xxx;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;

namespace WPFToolboxForSiemensPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class ContentWindowSZL : DocumentContent
    {
        public ContentWindowSZL()
        {
            InitializeComponent();

            
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int szlid = Helper.GetIntFromHexString(txtSZLid.Text);
            int szlindex = Helper.GetIntFromHexString(txtSZLindex.Text);
            try
            {
                SZLData szlData = App.clientForm.Connection.PLCGetSZL((short) szlid, (short) szlindex);
                myDataGrid.ItemsSource = szlData.SZLDaten;
            }
            catch(Exception ex)
            {
                App.clientForm.lblStatus.Text = ex.Message;
            }

            
        }

    }
}
