using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using CustomChromeLibrary;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
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
        private static Dictionary<string, PLCConnection> _connectionDictionary = new Dictionary<string, PLCConnection>();
        public event PropertyChangedEventHandler PropertyChanged;

        private Thread BackgroundReadingThread;

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

        public string DefaultConnection
        {
            get { return _defaultConnection; }
            set { _defaultConnection = value; NotifyPropertyChanged("DefaultConnection"); }
        }

        public static string DefaultConnectionStatic
        {
            get { return _defaultConnection; }
            set { _defaultConnection = value; }
        }
        private ObservableCollection<VarTabRowWithConnection> varTabRows;
        private static string _defaultConnection;

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

        private void cmdOnlineView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as ToggleButton;
                if (btn.IsChecked.Value)
                {
                    _connectionDictionary = new Dictionary<string, PLCConnection>();

                    var conns = from rw in varTabRows group rw by rw.ConnectionName;

                    foreach (var conn in conns)
                    {
                        if (!string.IsNullOrEmpty(conn.Key))
                            _connectionDictionary.Add(conn.Key, new PLCConnection(conn.Key));
                    }

                    if (!string.IsNullOrEmpty(DefaultConnection) &&
                        !_connectionDictionary.ContainsKey(DefaultConnection))
                        _connectionDictionary.Add(DefaultConnection, new PLCConnection(DefaultConnection));

                    lblStatus.Content = "";


                    foreach (var varTabRowWithConnection in varTabRows)
                    {
                        //Register Notify Changed Handler vor <connected Property
                        var conn = varTabRowWithConnection.Connection;
                        if (conn != null)
                            conn.Configuration.ConnectionName = conn.Configuration.ConnectionName;
                    }


                    foreach (var plcConnection in _connectionDictionary)
                    {
                        plcConnection.Value.AutoConnect = false;
                        plcConnection.Value.Connect();
                    }
                    
                    var st = new ThreadStart(BackgroundReadingProc);
                    BackgroundReadingThread = new Thread(st);
                    BackgroundReadingThread.Name = "Background Reading Thread";
                    BackgroundReadingThread.Start();

                    ProgressBarOnlineStatus.IsIndeterminate = true;
                }
                else
                {
                    if (BackgroundReadingThread != null)
                        BackgroundReadingThread.Abort();

                    Thread.Sleep(100);

                    foreach (var plcConnection in _connectionDictionary)
                    {
                        plcConnection.Value.Disconnect();
                    }

                    ProgressBarOnlineStatus.IsIndeterminate = false;
                }
            }
            catch(Exception ex)
            {
                lblStatus.Content = ex.Message;
            }
        }

        private void BackgroundReadingProc()
        {
            try
            {

                Parallel.ForEach(_connectionDictionary, itm =>
                                                            {
                                                                try
                                                                {
                                                                    var conn = itm.Value;

                                                                    var values = from rw in varTabRows
                                                                                 where
                                                                                     rw.Connection == conn &&
                                                                                     rw.LibNoDaveValue != null
                                                                                 select rw.LibNoDaveValue;

                                                                    PLCConnection.VarTabReadData rq = null;
                                                                    if (ReadTagsConfig != 0)
                                                                    {
                                                                        rq =
                                                                            conn.ReadValuesWithVarTabFunctions(
                                                                                values,
                                                                                (PLCTriggerVarTab)
                                                                                ReadTagsConfig + 1);
                                                                    }
                                                                    while (true)
                                                                    {

                                                                        if (ReadTagsConfig == 0)
                                                                            conn.ReadValues(values);
                                                                        else
                                                                        {
                                                                            if (rq != null)
                                                                                rq.RequestData();
                                                                        }

                                                                    }
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                }
                                                            });

            }
            catch (ThreadAbortException ex)
            {

            }
        }
       
        private void ReadPlcTagsFromConnection(PLCConnection conn)
        {
            var values = from rw in varTabRows
                         where rw.Connection == conn && rw.LibNoDaveValue != null
                         select rw.LibNoDaveValue;

            if (ReadTagsConfig == 0)
                conn.ReadValues(values);
            else
            {
                var rq = conn.ReadValuesWithVarTabFunctions(values, (PLCTriggerVarTab) ReadTagsConfig + 1);
                rq.RequestData();
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

        private void cmdControlValues_Click(object sender, RoutedEventArgs e)
        {
            if (!ProgressBarOnlineStatus.IsIndeterminate)
            {
                MessageBox.Show("You need to be in Viewing State toi Control Values!");
                return;
            }
            Parallel.ForEach(_connectionDictionary, itm =>
                                                        {
                                                            try
                                                            {
                                                                var conn = itm.Value;

                                                                var values = from rw in varTabRows
                                                                             where
                                                                                 rw.Connection == conn &&
                                                                                 rw.LibNoDaveValue != null &&
                                                                                 rw.LibNoDaveValue.Controlvalue != null
                                                                             select rw.LibNoDaveValue;


                                                                if (WriteTagsConfig == 0)
                                                                    conn.WriteValues(values);
                                                                else
                                                                {
                                                                    conn.WriteValuesWithVarTabFunctions(values,
                                                                                                        (
                                                                                                        PLCTriggerVarTab
                                                                                                        )
                                                                                                        WriteTagsConfig +
                                                                                                        1);
                                                                }


                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                MessageBox.Show("Error Writing to PLC: " +
                                                                                ex.Message);
                                                            }
                                                        });

        }

        private void cmdOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opnDlg = new OpenFileDialog();
            opnDlg.Filter = "Alle Unterstützten Datentypen (*.wpfvartab)|*.wpfvartab";
            var retVal = opnDlg.ShowDialog();
            if (retVal == true)
            {
                varTabRows.Clear();

                System.IO.FileStream jj = new FileStream(opnDlg.FileName, FileMode.Open);
                System.Xml.Serialization.XmlSerializer myXml = new XmlSerializer(typeof(ObservableCollection<VarTabRowWithConnection>));
                var saved = (ObservableCollection<VarTabRowWithConnection>)myXml.Deserialize(jj);
                foreach (var varTabRowWithConnection in saved)
                {
                    varTabRows.Add(varTabRowWithConnection);
                }
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "*.wpfvartab|*.wpfvartab";
            if (saveDlg.ShowDialog().Value)
            {
                System.IO.FileStream jj = new FileStream(saveDlg.FileName, FileMode.Create);
                System.Xml.Serialization.XmlSerializer myXml = new XmlSerializer(typeof(ObservableCollection<VarTabRowWithConnection>));
                myXml.Serialize(jj, varTabRows);
                jj.Close();
            }
        }
    }
}
