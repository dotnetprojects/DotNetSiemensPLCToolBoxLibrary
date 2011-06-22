using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Win32;

namespace WPFVarTab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : CustomChromeWindow
    {
        private PLCConnection myConn = new PLCConnection("JFK-WPFVarTab");

        public int ReadTagsConfig { get; set; }
        public int WriteTagsConfig { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            ObservableCollection<S7VATRow> vatRows = new ObservableCollection<S7VATRow>();
            vatRows.Add(new S7VATRow() { LibNoDaveValue=new PLCTag() });
            vatRows.Add(new S7VATRow() { LibNoDaveValue = new PLCTag() });
            vatRows.Add(new S7VATRow() { LibNoDaveValue = new PLCTag() });
            vatRows.Add(new S7VATRow() {Comment = "aa"});
            vatRows.Add(new S7VATRow() { LibNoDaveValue = new PLCTag() });
            vatRows.Add(new S7VATRow() { LibNoDaveValue = new PLCTag() });
            dataGrid.ItemsSource = vatRows;
            
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
            Configuration.ShowConfiguration("JFK-WPFVarTab", true);
            myConn = new PLCConnection("JFK-WPFVarTab");
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
                myConn.Connect();
                if (myConn.Connected)
                    ProgressBarOnlineStatus.IsIndeterminate = true;
            }
            catch(Exception ex)
            {
                lblStatus.Content = ex.Message;
            }
        }

        private void cmdConfigVarTab_Click(object sender, RoutedEventArgs e)
        {
            ConfigVarTab subWin=new ConfigVarTab();
            subWin.ShowDialog();
        }





    }


}
