using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;
using CustomChromeLibrary;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;
using Microsoft.Win32;

namespace WPFVarTab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : CustomChromeWindow, INotifyPropertyChanged
    {
        private int _readTagsConfig;
        private int _writeTagsConfig;
        private ObservableCollection<string> _connections;
        private static Dictionary<string, PLCConnection> _connectionDictionary;
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
                PropertyChanged(this, new PropertyChangedEventArgs("ObjectAsString"));
            }
        }

        public int ReadTagsConfig
        {
            get { return _readTagsConfig; }
            set { _readTagsConfig = value; NotifyPropertyChanged("ReadTagsConfig"); }
        }

        public int WriteTagsConfig
        {
            get { return _writeTagsConfig; }
            set { _writeTagsConfig = value; NotifyPropertyChanged("WriteTagsConfig"); }
        }

        public ObservableCollection<string> Connections
        {
            get { return _connections; }
            set { _connections = value; NotifyPropertyChanged("Connections"); }
        }

        public static Dictionary<string, PLCConnection> ConnectionDictionary
        {
            get { return _connectionDictionary; }
        }

        private ObservableCollection<VarTabRowWithConnection> varTabRows;

        public MainWindow()
        {
            InitializeComponent();

            BuildConnectionList();

            this.DataContext = this;

            varTabRows = new ObservableCollection<VarTabRowWithConnection>();
            dataGrid.ItemsSource = varTabRows;
        }

        private void _OnShowSystemMenuCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Window _window = (Window)e.Parameter;
            Point _point = new Point(_window.Left + 24, _window.Top + 24);
            if (_window.WindowState == WindowState.Maximized)
                _point = new Point(18, 18);
            Microsoft.Windows.Shell.SystemCommands.ShowSystemMenu(_window, _point);
        }

        private void _OnSystemCommandCloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Windows.Shell.SystemCommands.CloseWindow((Window)e.Parameter);
        }

        private void cmdConfigConnection_Click(object sender, RoutedEventArgs e)
        {
            Configuration.ShowConfiguration("JFK-WPFVarTab Connection 1", false);
        }

        
        private void BuildConnectionList()
        {
            Connections = new ObservableCollection<string>(PLCConnectionConfiguration.GetConfigurationNames());
        }


        private void cmdOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opnDlg = new OpenFileDialog();
            opnDlg.Filter = "Alle Unterstützten Datentypen (*.vartab; *.s7p; *.zip)|*.vartab;*s7p;*.zip";
            var retVal = opnDlg.ShowDialog();
            if (retVal == true)
            {
                string file = opnDlg.FileName;
                string ext = System.IO.Path.GetExtension(file).ToLower();
                if (ext == "vartab")
                {

                }
                else
                {
                    DotNetSiemensPLCToolBoxLibrary.Projectfiles.SelectProjectPart.SelectVAT(file);
                }
            }
        }

        private void cmdOnlineView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblStatus.Content = "";
                //myConn.Connect();
                //if (myConn.Connected)
                    ProgressBarOnlineStatus.IsIndeterminate = true;
            }
            catch(Exception ex)
            {
                lblStatus.Content = ex.Message;
            }
        }

        private void cmdConfigVarTab_Click(object sender, RoutedEventArgs e)
        {
            ConfigVarTab subWin = new ConfigVarTab(this);
            subWin.ShowDialog();
        }

        private void cmdImportVarTab_Click(object sender, RoutedEventArgs e)
        {
            var s7Vat = SelectProjectPart.SelectVAT();

            foreach (var S7VatRow in s7Vat.VATRows)
            {
                varTabRows.Add(new VarTabRowWithConnection(S7VatRow));
            }
        }
    }
}
