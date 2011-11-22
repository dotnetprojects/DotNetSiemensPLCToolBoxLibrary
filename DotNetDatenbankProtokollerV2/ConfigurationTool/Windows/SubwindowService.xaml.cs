using System;
using System.Windows;
using System.Windows.Threading;
using DotNetSimaticDatabaseProtokollerLibrary;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using UserControl = System.Windows.Controls.UserControl;

namespace DotNetSimaticDatabaseProtokollerConfigurationTool.Windows
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SubwindowService : UserControl, IDisposable
    {
        public SubwindowService()
        {
            InitializeComponent();            
        }

        private void cmdStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.ServiceProcess.ServiceController dienst = new System.ServiceProcess.ServiceController(StaticServiceConfig.MyServiceName);
                dienst.Start();
            }
            catch (Exception)
            { }
        }

        private void cmdStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.ServiceProcess.ServiceController dienst = new System.ServiceProcess.ServiceController(StaticServiceConfig.MyServiceName);
                dienst.Stop();
            }
            catch (Exception)
            { }
        }

        private void cmdInstall_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.FileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "JFK-ProtokollerService.exe";
            
            myProcess.StartInfo.Arguments = "/install";
            /*if (chkPostgres.Checked)
                myProcess.StartInfo.Arguments += " /postgres";
            if (chkMysql.Checked)
                myProcess.StartInfo.Arguments += " /mysql";
            if (chkMssql.Checked)
                myProcess.StartInfo.Arguments += " /mssql";*/
            myProcess.Start();
        }

        private void cmdUninstall_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.FileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "JFK-ProtokollerService.exe";

            myProcess.StartInfo.Arguments = "/uninstall";            
            myProcess.Start();
        }


        private DispatcherTimer myTimer;
        public void Dispose()
        {
            if (myTimer != null)
                myTimer.Stop();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (myTimer != null)
                myTimer.Stop();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            myTimer=new DispatcherTimer();
            myTimer.Tick += new EventHandler(myTimer_Tick);
            myTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            myTimer.Start();
        }

        private int oldcnt = 0;

        void myTimer_Tick(object sender, EventArgs e)
        {
            System.ServiceProcess.ServiceController dienst = new System.ServiceProcess.ServiceController(StaticServiceConfig.MyServiceName);

            try
            {
                if (oldcnt != Logging.LogEntries.Count)
                {
                    grdLogentries.ItemsSource = null;
                    grdLogentries.ItemsSource = Logging.LogEntries;
                    grdLogentries.Items.Refresh();
                    oldcnt = Logging.LogEntries.Count;
                }
            }
            catch (Exception ex)
            { }

            try
            {
                switch (dienst.Status)
                {
                    case System.ServiceProcess.ServiceControllerStatus.ContinuePending:
                        lblServiceState.Content = "Continue Pending";
                        break;
                    case System.ServiceProcess.ServiceControllerStatus.Paused:
                        lblServiceState.Content = "Paused";
                        break;
                    case System.ServiceProcess.ServiceControllerStatus.PausePending:
                        lblServiceState.Content = "Pause Pending";
                        break;
                    case System.ServiceProcess.ServiceControllerStatus.Running:
                        lblServiceState.Content = "Running";
                        break;
                    case System.ServiceProcess.ServiceControllerStatus.StartPending:
                        lblServiceState.Content = "Start Pending";
                        break;
                    case System.ServiceProcess.ServiceControllerStatus.Stopped:
                        lblServiceState.Content = "Stopped";
                        break;
                    case System.ServiceProcess.ServiceControllerStatus.StopPending:
                        lblServiceState.Content = "Stop Pending";
                        break;

                }
            }
            catch (InvalidOperationException)
            {
                lblServiceState.Content = "Service not installed!";
            }
        }

        private void cmdClearLog_Click(object sender, RoutedEventArgs e)
        {
            Logging.ClearLog();
        }
    }
}
