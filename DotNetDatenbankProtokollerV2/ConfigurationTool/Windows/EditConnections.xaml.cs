using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections;
using Xceed.Wpf.Toolkit.PropertyGrid;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace DotNetSimaticDatabaseProtokollerConfigurationTool.Windows
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class EditConnections : UserControl
    {
        public EditConnections()
        {
            InitializeComponent();
        }

        private void cmdAddConnection_Click(object sender, RoutedEventArgs e)
        {
            string val = "Connection_" + (grdConnections.Items.Count + 1);
            if (DotNetSiemensPLCToolBoxLibrary.General.InputBox.Show("Connectionname", "Name of the PLC Connection", ref val) == DialogResult.OK)
            {
                foreach (ConnectionConfig plcConnectionConfiguration in ProtokollerConfiguration.ActualConfigInstance.Connections)
                {
                    if (plcConnectionConfiguration.Name.ToLower().Trim() == val.ToLower().Trim())
                    {
                        MessageBox.Show("A Connection with this Name already Exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                PLCConnectionConfiguration myIntConfig = new PLCConnectionConfiguration(val.Trim(), LibNodaveConnectionConfigurationType.ObjectSavedConfiguration);
                myIntConfig = DotNetSiemensPLCToolBoxLibrary.Communication.Configuration.ShowConfiguration(myIntConfig);
                if (myIntConfig != null)
                {
                    LibNoDaveConfig myConfig = new LibNoDaveConfig();
                    myConfig.Configuration = myIntConfig;
                    ProtokollerConfiguration.ActualConfigInstance.Connections.Add(myConfig);
                }
                //grdConnections.Items.Add(myConfig);
            }
        }

        private void cmdRemoveConnection_Click(object sender, RoutedEventArgs e)
        {
            if (grdConnections.SelectedItem != null)
                ProtokollerConfiguration.ActualConfigInstance.Connections.Remove((ConnectionConfig) grdConnections.SelectedItem);
        }

        private void grdConnections_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (grdConnections.SelectedItem != null)
            {
                if (grdConnections.SelectedItem is LibNoDaveConfig)
                {
                    LibNoDaveConfig myConfig = (LibNoDaveConfig) grdConnections.SelectedItem;
                    myConfig.Configuration = DotNetSiemensPLCToolBoxLibrary.Communication.Configuration.ShowConfiguration(myConfig.Configuration);
                }
            }
        }

        private void cmdAddTCPIPConnection_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This Connection type is for Communication via AG_SEND/AG_RECIEVE to a siemens PLC, for use with direct reading over TCP/IP use the PLC-Connection !");
            string val = "Connection_" + (grdConnections.Items.Count + 1);
            if (DotNetSiemensPLCToolBoxLibrary.General.InputBox.Show("Connectionname", "Name of the Connection", ref val) == DialogResult.OK)
            {
                foreach (ConnectionConfig plcConnectionConfiguration in ProtokollerConfiguration.ActualConfigInstance.Connections)
                {
                    if (plcConnectionConfiguration.Name.ToLower().Trim() == val.ToLower().Trim())
                    {
                        MessageBox.Show("A Connection with this Name already Exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                TCPIPConfig myConfig = new TCPIPConfig() {Name = val};
                ProtokollerConfiguration.ActualConfigInstance.Connections.Add(myConfig);
                //grdConnections.Items.Add(myConfig);
            }
        }

        private void PropertyGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((PropertyItem)prpGrid.SelectedPropertyItem).Value is PLCConnectionConfiguration)
            {
                LibNoDaveConfig myConfig = (LibNoDaveConfig) grdConnections.SelectedItem;
                myConfig.Configuration = DotNetSiemensPLCToolBoxLibrary.Communication.Configuration.ShowConfiguration(myConfig.Configuration);
            }
        }

        private void cmdAddDatabaseConnection_Click(object sender, RoutedEventArgs e)
        {
            string val = "Connection_" + (grdConnections.Items.Count + 1);
            if (
                DotNetSiemensPLCToolBoxLibrary.General.InputBox.Show("Connectionname", "Name of the Database Connection",
                                                                     ref val) == DialogResult.OK)
            {
                foreach (
                    ConnectionConfig plcConnectionConfiguration in
                        ProtokollerConfiguration.ActualConfigInstance.Connections)
                {
                    if (plcConnectionConfiguration.Name.ToLower().Trim() == val.ToLower().Trim())
                    {
                        MessageBox.Show("A Connection with this Name already Exists!", "Error", MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                        return;
                    }
                }
                var myConfig = new DatabaseConfig();
                myConfig.Name = val;
                ProtokollerConfiguration.ActualConfigInstance.Connections.Add(myConfig);
            }
        }
    }
}
