using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace DotNetSimaticDatabaseProtokollerConfigurationTool.Windows
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class EditStorages : UserControl
    {
        public EditStorages()
        {
            InitializeComponent();
        }     

        private void cmdRemoveConnection_Click(object sender, RoutedEventArgs e)
        {
            if (grdStorages.SelectedItem != null)
                ProtokollerConfiguration.ActualConfigInstance.Storages.Remove((StorageConfig) grdStorages.SelectedItem);
        }       

        private void cmdAddCSVStorage_Click(object sender, RoutedEventArgs e)
        {
            string val = "Storage_" + (grdStorages.Items.Count + 1);
            if (DotNetSiemensPLCToolBoxLibrary.General.InputBox.Show("Storage-Name", "Name of the Storage", ref val) == DialogResult.OK)
            {
                foreach (var tmp in ProtokollerConfiguration.ActualConfigInstance.Storages)
                {
                    if (tmp.Name.ToLower().Trim() == val.ToLower().Trim())
                    {
                        MessageBox.Show("A Storage with this Name already Exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                CSVConfig storage = new CSVConfig() {Name = val};
                ProtokollerConfiguration.ActualConfigInstance.Storages.Add(storage);
            }
        }

        private void cmdAddSQLiteStorage_Click(object sender, RoutedEventArgs e)
        {
            string val = "Storage_" + (grdStorages.Items.Count + 1);
            if (DotNetSiemensPLCToolBoxLibrary.General.InputBox.Show("Storage-Name", "Name of the Storage", ref val) == DialogResult.OK)
            {
                foreach (var tmp in ProtokollerConfiguration.ActualConfigInstance.Storages)
                {
                    if (tmp.Name.ToLower().Trim() == val.ToLower().Trim())
                    {
                        MessageBox.Show("A Storage with this Name already Exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                SQLiteConfig storage = new SQLiteConfig() { Name = val };
                ProtokollerConfiguration.ActualConfigInstance.Storages.Add(storage);
            }
        }

        private void cmdAddMySQLStorage_Click(object sender, RoutedEventArgs e)
        {
            string val = "Storage_" + (grdStorages.Items.Count + 1);
            if (DotNetSiemensPLCToolBoxLibrary.General.InputBox.Show("Storage-Name", "Name of the Storage", ref val) == DialogResult.OK)
            {
                foreach (var tmp in ProtokollerConfiguration.ActualConfigInstance.Storages)
                {
                    if (tmp.Name.ToLower().Trim() == val.ToLower().Trim())
                    {
                        MessageBox.Show("A Storage with this Name already Exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                MySQLConfig storage = new MySQLConfig() { Name = val };
                ProtokollerConfiguration.ActualConfigInstance.Storages.Add(storage);
            }
        }

        private void cmdAddPostgreSQLStorage_Click(object sender, RoutedEventArgs e)
        {
            string val = "Storage_" + (grdStorages.Items.Count + 1);
            if (DotNetSiemensPLCToolBoxLibrary.General.InputBox.Show("Storage-Name", "Name of the Storage", ref val) == DialogResult.OK)
            {
                foreach (var tmp in ProtokollerConfiguration.ActualConfigInstance.Storages)
                {
                    if (tmp.Name.ToLower().Trim() == val.ToLower().Trim())
                    {
                        MessageBox.Show("A Storage with this Name already Exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                PostgreSQLConfig storage = new PostgreSQLConfig() { Name = val };
                ProtokollerConfiguration.ActualConfigInstance.Storages.Add(storage);
            }
        }

        private void cmdAddMsSQLStorage_Click(object sender, RoutedEventArgs e)
        {
            string val = "Storage_" + (grdStorages.Items.Count + 1);
            if (DotNetSiemensPLCToolBoxLibrary.General.InputBox.Show("Storage-Name", "Name of the Storage", ref val) == DialogResult.OK)
            {
                foreach (var tmp in ProtokollerConfiguration.ActualConfigInstance.Storages)
                {
                    if (tmp.Name.ToLower().Trim() == val.ToLower().Trim())
                    {
                        MessageBox.Show("A Storage with this Name already Exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                MsSQLConfig storage = new MsSQLConfig() { Name = val };
                ProtokollerConfiguration.ActualConfigInstance.Storages.Add(storage);
            }
        }

        private void cmdAddODBCStorage_Click(object sender, RoutedEventArgs e)
        {
            string val = "Storage_" + (grdStorages.Items.Count + 1);
            if (DotNetSiemensPLCToolBoxLibrary.General.InputBox.Show("Storage-Name", "Name of the Storage", ref val) == DialogResult.OK)
            {
                foreach (var tmp in ProtokollerConfiguration.ActualConfigInstance.Storages)
                {
                    if (tmp.Name.ToLower().Trim() == val.ToLower().Trim())
                    {
                        MessageBox.Show("A Storage with this Name already Exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                ODBCConfig storage = new ODBCConfig() { Name = val };
                ProtokollerConfiguration.ActualConfigInstance.Storages.Add(storage);
            }
        }

        private void cmdAddExcelStorage_Click(object sender, RoutedEventArgs e)
        {
            string val = "Storage_" + (grdStorages.Items.Count + 1);
            if (DotNetSiemensPLCToolBoxLibrary.General.InputBox.Show("Storage-Name", "Name of the Storage", ref val) == DialogResult.OK)
            {
                foreach (var tmp in ProtokollerConfiguration.ActualConfigInstance.Storages)
                {
                    if (tmp.Name.ToLower().Trim() == val.ToLower().Trim())
                    {
                        MessageBox.Show("A Storage with this Name already Exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                ExcelConfig storage = new ExcelConfig() { Name = val };
                ProtokollerConfiguration.ActualConfigInstance.Storages.Add(storage);
            }
        }

        private void cmdAddExcel2007Storage_Click(object sender, RoutedEventArgs e)
        {
            string val = "Storage_" + (grdStorages.Items.Count + 1);
            if (DotNetSiemensPLCToolBoxLibrary.General.InputBox.Show("Storage-Name", "Name of the Storage", ref val) == DialogResult.OK)
            {
                foreach (var tmp in ProtokollerConfiguration.ActualConfigInstance.Storages)
                {
                    if (tmp.Name.ToLower().Trim() == val.ToLower().Trim())
                    {
                        MessageBox.Show("A Storage with this Name already Exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                Excel2007Config storage = new Excel2007Config() { Name = val };
                ProtokollerConfiguration.ActualConfigInstance.Storages.Add(storage);
            }
        }

        
    }
}
