using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.Communication.S7_xxx;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;
using JFKCommonLibrary.WPF.Converters;

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

            App.clientForm.lblStatus.Text = "";
            try
            {
                SZLData szlData = App.clientForm.Connection.PLCGetSZL((short) szlid, (short) szlindex);

                myDataGrid.Columns.Clear();

                if (szlData.SZLDaten.Length>0)
                {
                    foreach (var prpInfo in szlData.SZLDaten[0].GetType().GetProperties())
                    {
                        myDataGrid.Columns.Add(new DataGridTextColumn() {Binding = new Binding(prpInfo.Name){Converter = new ByteIntArrayConverter() }, Header = prpInfo.Name});
                    } 
                }
                myDataGrid.ItemsSource = szlData.SZLDaten;
            }
            catch(Exception ex)
            {
                App.clientForm.lblStatus.Text = ex.Message;
            }

            
        }

    }
}
