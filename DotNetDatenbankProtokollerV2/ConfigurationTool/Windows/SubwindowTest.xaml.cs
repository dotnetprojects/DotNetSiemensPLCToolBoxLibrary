using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using DotNetSimaticDatabaseProtokollerLibrary;
using DotNetSimaticDatabaseProtokollerLibrary.Protocolling;
using DotNetSimaticDatabaseProtokollerLibrary.Remoting;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using UserControl = System.Windows.Controls.UserControl;

namespace DotNetSimaticDatabaseProtokollerConfigurationTool.Windows
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SubwindowTest : UserControl,IDisposable
    {
        public SubwindowTest()
        {
            InitializeComponent();
        }

        private ProtokollerInstance myInstance = null;

        private void cmdTest_Click(object sender, RoutedEventArgs e)
        {
            lblError.Text = "";

            string err = ProtokollerConfiguration.ActualConfigInstance.CheckConfiguration(true);
            if (err != null)
            {
                lblError.Text = err;
                return;
            }
            
            try
            {
                using (myInstance = new ProtokollerInstance(ProtokollerConfiguration.ActualConfigInstance))
                {
                    myInstance.StartTestMode();
                    myInstance.TestDataRead((DatasetConfig)cmbTriggerConnection.SelectedValue);
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                return;
            }
            lblError.Text = "OK!";
        }

        private void cmdTestWithStorrage_Click(object sender, RoutedEventArgs e)
        {
            lblError.Text = "";

            string err = ProtokollerConfiguration.ActualConfigInstance.CheckConfiguration(true);
            if (err != null)
            {
                lblError.Text = err;
                return;
            }
                
            try
            {
                using (myInstance = new ProtokollerInstance(ProtokollerConfiguration.ActualConfigInstance))
                {
                    myInstance.StartTestMode();
                    myInstance.TestDataReadWrite((DatasetConfig)cmbTriggerConnection.SelectedValue);
                    Thread.Sleep(1500);
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                return;
            }
            lblError.Text = "OK!";
        }

        private ProtokollerInstance testinstance = null;
        
        private void cmdTestService_Click(object sender, RoutedEventArgs e)
        {
            lblError.Text = "";

            if (testinstance != null)
                testinstance.Dispose();

            string err = ProtokollerConfiguration.ActualConfigInstance.CheckConfiguration(false);
            if (err != null)
            {
                lblError.Text = err;
                return;
            }
            
            try
            {
                testinstance = new ProtokollerInstance(ProtokollerConfiguration.ActualConfigInstance);
                testinstance.ThreadExceptionOccured += new ThreadExceptionEventHandler(testinstance_ThreadExceptionOccured);
                testinstance.Start(false);
                cmdTestService.Background = Brushes.LightGreen;
                cmdTest.IsEnabled = false;
                cmdTestTriggers.IsEnabled = false;
                cmdTestWithStorrage.IsEnabled = false;
                cmbTriggerConnection.IsEnabled = false;
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }                        
        }

        void testinstance_ThreadExceptionOccured(object sender, ThreadExceptionEventArgs e)
        {
            lblError.Text = e.Exception.Message;
        }

        private void cmdTestTriggers_Click(object sender, RoutedEventArgs e)
        {
            string err = ProtokollerConfiguration.ActualConfigInstance.CheckConfiguration(true);
            if (err != null)
            {
                lblError.Text = err;
                return;
            }

            try
            {
                using (myInstance = new ProtokollerInstance(ProtokollerConfiguration.ActualConfigInstance))
                {
                    myInstance.TestTriggers((DatasetConfig) cmbTriggerConnection.SelectedValue);
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                return;
            }
            lblError.Text = "OK!";

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (testinstance != null)
                testinstance.Dispose();
        }       

        public void Dispose()
        {
            UserControl_Unloaded(null, null);
        }
    }
}
