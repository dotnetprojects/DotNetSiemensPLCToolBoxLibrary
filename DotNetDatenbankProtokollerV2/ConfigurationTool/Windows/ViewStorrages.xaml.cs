using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using DotNetSimaticDatabaseProtokollerLibrary.Databases;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using UserControl = System.Windows.Controls.UserControl;

namespace DotNetSimaticDatabaseProtokollerConfigurationTool.Windows
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ViewStorrages : UserControl
    {
        private IDBInterface dbIf;
        private IDBViewable dbView;
        private IDBViewableSQL dbViewSQL;
        private DatasetConfig datasetConfig;

        private long CurrentNumber = 0;
        public ViewStorrages()
        {
            InitializeComponent();
        }
       
        private void cmbStorage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblError.Content = null;
            if (dbIf != null)
                dbIf.Dispose();
            dbIf = null;
            dbView = null;

            lblDataCount.Content = null;
            grdDatasetFields.ItemsSource = null;

            datasetConfig = cmbStorage.SelectedItem as DatasetConfig;
            dbIf = StorageHelper.GetStorage(datasetConfig);
            dbIf.Connect_To_Database(datasetConfig.Storage);
            dbView = dbIf as IDBViewable;
            dbViewSQL = dbIf as IDBViewableSQL;

            txtSQL.IsEnabled = false;
            cmdSQL.IsEnabled = false;

            try
            {
                if (dbView != null)
                {
                    txtSQL.Text = "";
                    CurrentNumber = 0;
                    DataTable tbl = dbView.ReadData(datasetConfig, CurrentNumber, 1000);
                    if (tbl != null)
                        grdDatasetFields.ItemsSource = tbl.DefaultView;
                    lblDataCount.Content = dbView.ReadCount(datasetConfig);
                }


                if (dbViewSQL != null)
                {
                    txtSQL.Text = "SELECT * FROM " + datasetConfig.Name + " LIMIT " + 1000.ToString();
                    txtSQL.IsEnabled = true;
                    cmdSQL.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Content = ex.Message;
            }
        }

        
        private void cmbStorage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (dbIf != null)
                dbIf.Dispose();
        }

       

        private void cmdBeginn_Click(object sender, RoutedEventArgs e)
        {
            
            if (dbView != null)
            {
                CurrentNumber = 0;

                txtFromDataset.Text = CurrentNumber.ToString();
                lblToDataset.Content = (CurrentNumber + 1000).ToString();

                DataTable tbl = dbView.ReadData(datasetConfig, CurrentNumber, 1000);
                if (tbl != null)
                    grdDatasetFields.ItemsSource = tbl.DefaultView;
                lblDataCount.Content = dbView.ReadCount(datasetConfig);
            }
        }

        private void cmdBack_Click(object sender, RoutedEventArgs e)
        {
           
            if (dbView != null)
            {
                CurrentNumber -= 1000;
                if (CurrentNumber < 0)
                    CurrentNumber = 0;

                txtFromDataset.Text = CurrentNumber.ToString();
                lblToDataset.Content = (CurrentNumber + 1000).ToString();

                DataTable tbl = dbView.ReadData(datasetConfig, CurrentNumber, 1000);
                if (tbl != null)
                    grdDatasetFields.ItemsSource = tbl.DefaultView;
                lblDataCount.Content = dbView.ReadCount(datasetConfig);
            }
        }

        private void cmdFwd_Click(object sender, RoutedEventArgs e)
        {
           
            if (dbView != null)
            {
                long cnt = dbView.ReadCount(datasetConfig);
                CurrentNumber += 1000;
                if (CurrentNumber > cnt - 1000)
                    CurrentNumber = cnt - 1000;
                
                txtFromDataset.Text = CurrentNumber.ToString();
                lblToDataset.Content = (CurrentNumber + 1000).ToString();
                lblDataCount.Content = cnt;

                DataTable tbl = dbView.ReadData(datasetConfig, CurrentNumber, 1000);
                if (tbl != null)
                    grdDatasetFields.ItemsSource = tbl.DefaultView;                
            }
        }

        private void cmdEnd_Click(object sender, RoutedEventArgs e)
        {
            if (dbView != null)
            {
                long cnt = dbView.ReadCount(datasetConfig);
                CurrentNumber = cnt - 1000;

                txtFromDataset.Text = CurrentNumber.ToString();
                lblToDataset.Content = (CurrentNumber + 1000).ToString();
                lblDataCount.Content = cnt;
                DataTable tbl = dbView.ReadData(datasetConfig, CurrentNumber, 1000);
                if (tbl != null)
                    grdDatasetFields.ItemsSource = tbl.DefaultView;
            }
        }

        private void txtFromDataset_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (dbView != null)
                {
                    long cnt = dbView.ReadCount(datasetConfig);
                    CurrentNumber = 0;
                    try
                    {
                        CurrentNumber = long.Parse(txtFromDataset.Text);
                    }
                    catch (Exception)
                    { }
                    if (CurrentNumber > cnt - 1000)
                        CurrentNumber = cnt - 1000;
                    if (CurrentNumber < 0)
                        CurrentNumber = 0;

                    txtFromDataset.Text = CurrentNumber.ToString();
                    lblToDataset.Content = (CurrentNumber + 1000).ToString();
                    lblDataCount.Content = cnt;
                    DataTable tbl = dbView.ReadData(datasetConfig, CurrentNumber, 1000);
                    if (tbl != null)
                        grdDatasetFields.ItemsSource = tbl.DefaultView;
                }
            }
        }

        private void txtFromDataset_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void cmdSQL_Click(object sender, RoutedEventArgs e)
        {
            if (dbViewSQL != null)
            {
                txtFromDataset.Text = "";
                lblToDataset.Content = null;

                DataTable tbl = dbViewSQL.ReadData(datasetConfig, txtSQL.Text, 1000);
                if (tbl != null)
                    grdDatasetFields.ItemsSource = tbl.DefaultView;
                lblDataCount.Content = dbView.ReadCount(datasetConfig);
            }
        }

        private void txtSQL_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                cmdSQL_Click(null, null);
        }

    }
}
