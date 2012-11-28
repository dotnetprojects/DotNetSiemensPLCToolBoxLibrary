using System.Windows;
using DotNetSimaticDatabaseProtokollerLibrary;
using Microsoft.Win32;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace DotNetSimaticDatabaseProtokollerConfigurationTool.Windows
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Protocoling : UserControl
    {
        public Protocoling()
        {
            InitializeComponent();
        }     

        private void cmdsave_Click(object sender, RoutedEventArgs e)
        {
            ProtokollerConfiguration.Save();
        }

        private void cmdReload_Click(object sender, RoutedEventArgs e)
        {
            ProtokollerConfiguration.Load();
        }

        private void cmdClear_Click(object sender, RoutedEventArgs e)
        {
            ProtokollerConfiguration.Clear();
        }

        private void cmdCheck_Click(object sender, RoutedEventArgs e)
        {
            lblErrors.Content = null;
            string err = ProtokollerConfiguration.ActualConfigInstance.CheckConfiguration(true);
            if (err != null)
                lblErrors.Content = err;
            else
                lblErrors.Content = "Configuration seems OK";
        }

        private void cmdSaveToFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg=new SaveFileDialog();
            dlg.Filter = "*.conf|*.conf";
            if (dlg.ShowDialog() == true)
                ProtokollerConfiguration.SaveToFile(dlg.FileName);
        }

        private void cmdLoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "*.conf|*.conf";
            if (dlg.ShowDialog() == true)
                ProtokollerConfiguration.LoadFromFile(dlg.FileName);
        }

        private void cmdImport_Click(object sender, RoutedEventArgs e)
        {
            ProtokollerConfiguration.ImportFromRegistry();
        }               
    }
}
