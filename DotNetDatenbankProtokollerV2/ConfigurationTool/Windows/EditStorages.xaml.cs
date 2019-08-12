using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;
using Xceed.Wpf.Toolkit.PropertyGrid;
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
                SQLiteConfig storage = new SQLiteConfig() {Name = val};
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
                MySQLConfig storage = new MySQLConfig() {Name = val};
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
                PostgreSQLConfig storage = new PostgreSQLConfig() {Name = val};
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
                MsSQLConfig storage = new MsSQLConfig() {Name = val};
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
                ODBCConfig storage = new ODBCConfig() {Name = val};
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
                ExcelConfig storage = new ExcelConfig() {Name = val};
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
                Excel2007Config storage = new Excel2007Config() {Name = val};
                ProtokollerConfiguration.ActualConfigInstance.Storages.Add(storage);
            }
        }

        private void cmdAddPLCStorage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Be carefull, writing to the PLC as a Storrage is BETA and not much tested yet!");
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
                PLCConnectionConfiguration myIntConfig = new PLCConnectionConfiguration(val.Trim(), LibNodaveConnectionConfigurationType.ObjectSavedConfiguration);
                myIntConfig = DotNetSiemensPLCToolBoxLibrary.Communication.Configuration.ShowConfiguration(myIntConfig);
                if (myIntConfig != null)
                {
                    var storage = new PLCConfig() {Name = val};
                    storage.Configuration = myIntConfig;
                    ProtokollerConfiguration.ActualConfigInstance.Storages.Add(storage);
                }
            }
        }

        private void PropertyGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((PropertyItem)grdConfig.SelectedPropertyItem).Value is PLCConnectionConfiguration)
            {
                var myConfig = (PLCConfig) grdStorages.SelectedItem;
                myConfig.Configuration = DotNetSiemensPLCToolBoxLibrary.Communication.Configuration.ShowConfiguration(myConfig.Configuration);
            }
        }

        private void CmdAddMultiStorage_OnClick(object sender, RoutedEventArgs e)
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
                MultiStorageConfig storage = new MultiStorageConfig() { Name = val };
                ProtokollerConfiguration.ActualConfigInstance.Storages.Add(storage);
            }
        }
    }
}
