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
using System.Xml;
using System.Xml.Serialization;
using CustomChromeLibrary;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;
using JFKCommonLibrary.Serialization;
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
        private static ObservableDictionary<string, PLCConnection> _connectionDictionary = new ObservableDictionary<string, PLCConnection>();
        public event PropertyChangedEventHandler PropertyChanged;

        private static ObservableDictionary<string, ISymbolTable> _dictonaryConnectionSymboltables = new ObservableDictionary<string, ISymbolTable>();
        public ObservableDictionary<string, ISymbolTable> DictonaryConnectionSymboltables
        {
            get { return _dictonaryConnectionSymboltables; }
            set { _dictonaryConnectionSymboltables = value; NotifyPropertyChanged("DictonaryConnectionSymboltables"); }
        }

        private ObservableCollection<PLCConnectionConfiguration> _configuredConnections = new ObservableCollection<PLCConnectionConfiguration>();        
        public ObservableCollection<PLCConnectionConfiguration> ConfiguredConnections
        {
            get { return _configuredConnections; }
            set { _configuredConnections = value; }
        }

        private volatile bool readFresh = false;
        
        private Thread BackgroundReadingThread;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
                PropertyChanged(this, new PropertyChangedEventArgs("ObjectAsString"));
            }
        }

        public static bool IsOnline { get; set; }

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

        public static ObservableDictionary<string, PLCConnection> ConnectionDictionary
        {
            get { return _connectionDictionary; }
        }

        public string DefaultConnection
        {
            get { return _defaultConnection; }
            set
            {
                _defaultConnection = value;
                NotifyPropertyChanged("DefaultConnection");
                RefreshSymbols();
            }
        }

        private KeyValuePair<string, PLCConnection>? _defaultConnectionEntry;
        public KeyValuePair<string, PLCConnection>? DefaultConnectionEntry
        {
            get { return _defaultConnectionEntry; }
            set
            {
                _defaultConnectionEntry = value;
                NotifyPropertyChanged("DefaultConnectionEntry");
                if (value.HasValue)
                    DefaultConnection = value.Value.Value.Name;
                else
                    DefaultConnection = null;
            }
        }

        public static string DefaultConnectionStatic
        {
            get { return _defaultConnection; }
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
            Configuration.ShowConfiguration("JFK-WPFVarTab Connection 1", false); //ConfiguredConnections); //
            BuildConnectionList();
        }

        
        private void BuildConnectionList()
        {
            _connectionDictionary.Clear();
            Connections = new ObservableCollection<string>(PLCConnectionConfiguration.GetConfigurationNames());
            foreach (var item in Connections)
            {
                if (!DictonaryConnectionSymboltables.ContainsKey(item))
                    DictonaryConnectionSymboltables.Add(item, null);
                _connectionDictionary.Add(item, new PLCConnection(item));
            }

            RefreshSymbols();
        }

        private void cmdOnlineView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as ToggleButton;
                if (btn.IsChecked.Value)
                {
                    _connectionDictionary = new ObservableDictionary<string, PLCConnection>();

                    var conns = from rw in varTabRows group rw by rw.ConnectionName;

                    /*foreach (var conn in conns)
                    {
                        if (!string.IsNullOrEmpty(conn.Key))
                            _connectionDictionary.Add(conn.Key, new PLCConnection(conn.Key));
                    }*/

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

                    Parallel.ForEach(_connectionDictionary, plcConnection =>
                                                                {
                                                                    try
                                                                    {
                                                                        plcConnection.Value.AutoConnect = false;
                                                                        plcConnection.Value.Connect();
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                    }
                                                                });
                    
                    var st = new ThreadStart(BackgroundReadingProc);
                    BackgroundReadingThread = new Thread(st);
                    BackgroundReadingThread.Name = "Background Reading Thread";
                    BackgroundReadingThread.Start();

                    ProgressBarOnlineStatus.IsIndeterminate = true;
                    IsOnline = true;
                }
                else
                {
                    this.StopOnlineView();
                }
            }
            catch(Exception ex)
            {
                lblStatus.Content = ex.Message;
            }
        }

        private void StopOnlineView()
        {
            if (this.BackgroundReadingThread != null)
            {
                this.BackgroundReadingThread.Abort();
            }

            Thread.Sleep(100);

            foreach (KeyValuePair<string, PLCConnection> plcConnection in _connectionDictionary)
            {
                plcConnection.Value.Disconnect();
            }

            this.ProgressBarOnlineStatus.IsIndeterminate = false;
            IsOnline = false;
        }

        private void BackgroundReadingProc()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            ParallelOptions po = new ParallelOptions();
            po.CancellationToken = cts.Token;

            try
            {
                Parallel.ForEach(_connectionDictionary, po, itm =>
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

                                                                    while (true)
                                                                    {
                                                                        if (readFresh && ReadTagsConfig != 0 &&
                                                                            rq != null)
                                                                        {
                                                                            readFresh = false;
                                                                            rq.Dispose();
                                                                            rq = null;
                                                                        }

                                                                        if (rq == null && ReadTagsConfig != 0)
                                                                        {
                                                                            rq =
                                                                                conn.ReadValuesWithVarTabFunctions(
                                                                                    values,
                                                                                    (PLCTriggerVarTab)
                                                                                    ReadTagsConfig + 1);
                                                                        }

                                                                        if (ReadTagsConfig == 0)
                                                                            conn.ReadValues(values);
                                                                        else
                                                                        {
                                                                            if (rq != null)
                                                                                rq.RequestData();
                                                                        }

                                                                        po.CancellationToken.ThrowIfCancellationRequested();
                                                                    }
                                                                }
                                                                catch (ThreadAbortException ex)
                                                                {
                                                                    throw;
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                }
                                                            });

            }
            catch (ThreadAbortException ex)
            {
               cts.Cancel();
            }
            catch (Exception ex)
            {
            }
            cts.Cancel();
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

            if (s7Vat!=null)
                foreach (var S7VatRow in s7Vat.VATRows)
                {
                    varTabRows.Add(new VarTabRowWithConnection(S7VatRow));
                }

            RefreshSymbols();
        }

        private void cmdControlValues_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.AllowTagsControl)
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

                            var values = from rw in varTabRows where rw.Connection == conn && rw.LibNoDaveValue != null && rw.LibNoDaveValue.Controlvalue != null select rw.LibNoDaveValue;


                            if (WriteTagsConfig == 0)
                                conn.WriteValues(values);
                            else
                            {
                                conn.WriteValuesWithVarTabFunctions(values, (PLCTriggerVarTab)WriteTagsConfig + 1);
                            }


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error Writing to PLC: " + ex.Message);
                        }
                    });

            }
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


                var settings = new XmlReaderSettings(){CheckCharacters = false};
                var read = XmlTextReader.Create(jj, settings);
                System.Xml.Serialization.XmlSerializer myXml = new XmlSerializer(typeof(ObservableCollection<VarTabRowWithConnection>));
                var saved = (ObservableCollection<VarTabRowWithConnection>)myXml.Deserialize(read);

                foreach (var varTabRowWithConnection in saved)
                {
                    varTabRows.Add(varTabRowWithConnection);
                }

                jj.Close();
            }
        }

        private void cmdImportAllConnections_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opnDlg = new OpenFileDialog();
            opnDlg.Filter = "Alle Unterstützten Datentypen (*.connconf)|*.connconf";
            var retVal = opnDlg.ShowDialog();
            if (retVal == true)
            {
                System.IO.FileStream jj = new FileStream(opnDlg.FileName, FileMode.Open);
                System.Xml.Serialization.XmlSerializer myXml = new XmlSerializer(typeof(List<PLCConnectionConfiguration>));
                var saved = (List<PLCConnectionConfiguration>)myXml.Deserialize(jj);
                PLCConnectionConfiguration.ImportConfigurations(saved);   
             
                this.BuildConnectionList();
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

        private void cmdExportAllConnections_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "*.connconf|*.connconf";
            if (saveDlg.ShowDialog().Value)
            {
                System.IO.FileStream jj = new FileStream(saveDlg.FileName, FileMode.Create);
                System.Xml.Serialization.XmlSerializer myXml = new XmlSerializer(typeof(List<PLCConnectionConfiguration>));
                myXml.Serialize(jj, PLCConnectionConfiguration.ExportConfigurations());
                jj.Close();
            }
        }

        public void KillAllThreads()
        {
            if (BackgroundReadingThread != null)
                BackgroundReadingThread.Abort();
        }

        private void dataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (Properties.Settings.Default.AllowTagsControl)
            {
                foreach (KeyValuePair<string, PLCConnection> plcConnection in ConnectionDictionary)
                    plcConnection.Value.WriteQueueClear();

                foreach (var selectedItem in dataGrid.SelectedItems)
                {
                    var rw = selectedItem as VarTabRowWithConnection;

                    if (e.Key == Key.D1 && ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
                    {
                        if (rw != null && rw.Connection != null && rw.LibNoDaveValue != null)
                        {
                            rw.LibNoDaveValue.ControlValueAsString = "1";
                            rw.Connection.WriteQueueAdd(rw.LibNoDaveValue);
                        }
                    }
                    else if (e.Key == Key.D0 && ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
                    {
                        if (rw != null && rw.Connection != null && rw.LibNoDaveValue != null)
                        {
                            rw.LibNoDaveValue.ControlValueAsString = "0";
                            rw.Connection.WriteQueueAdd(rw.LibNoDaveValue);
                        }
                    }
                }

                foreach (KeyValuePair<string, PLCConnection> plcConnection in ConnectionDictionary)
                {
                    if (WriteTagsConfig == 0)
                        plcConnection.Value.WriteQueueWriteToPLC();
                    else

                        plcConnection.Value.WriteQueueWriteToPLCWithVarTabFunctions((PLCTriggerVarTab)WriteTagsConfig + 1);
                }
            }
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header.ToString() == "Address")
            {
                readFresh = true;
            }
            //var rw = e.Row.DataContext as VarTabRowWithConnection;
            //RefreshSymbol(rw);
        }

        private void cmdAddNumberOfRows_Click(object sender, RoutedEventArgs e)
        {
            var rw = dataGrid.SelectedItem as VarTabRowWithConnection;

            if (rw != null && rw.LibNoDaveValue != null)
            {
                int idx = varTabRows.IndexOf(rw);
                for (int n = 0; n < numberOfRows.Value; n++)
                {
                    rw = rw.GetNextRow();
                    varTabRows.Insert(++idx, rw);
                }
            }
        }

        private void RefreshSymbols()
        {
            if (varTabRows!=null)
                foreach (var varTabRowWithConnection in varTabRows)
                {
                    RefreshSymbol(varTabRowWithConnection);
                }
        }

        public static void RefreshSymbol(VarTabRowWithConnection varTabRowWithConnection)
        {
            string akConn = DefaultConnectionStatic;
            if (!string.IsNullOrEmpty(varTabRowWithConnection.ConnectionName))
                akConn = varTabRowWithConnection.ConnectionName;

            if (akConn != null)
            {
                var symTab = _dictonaryConnectionSymboltables[akConn];

                if (symTab == null)
                    varTabRowWithConnection.Symbol = null;
                else
                {
                    if (varTabRowWithConnection.S7FormatAddress != null)
                    {
                        var entry = symTab.GetEntryFromOperand(varTabRowWithConnection.S7FormatAddress);
                        if (entry != null)
                            varTabRowWithConnection.Symbol = entry.Symbol;
                        else
                            varTabRowWithConnection.Symbol = null;
                    }
                    else
                        varTabRowWithConnection.Symbol = null;
                }
            }
            else
                varTabRowWithConnection.Symbol = null;
        }

        private void cmdSetSymbolTabels_Click(object sender, RoutedEventArgs e)
        {
            var frm = new ConfigConnectionsAndSymbolSources(this);
            frm.ShowDialog();

            RefreshSymbols();
        }

        private void cmdNew_Click(object sender, RoutedEventArgs e)
        {
            varTabRows.Clear();
        }

        private void ThisWindow_Closing(object sender, CancelEventArgs e)
        {
            StopOnlineView();
        }
   
    }
}
