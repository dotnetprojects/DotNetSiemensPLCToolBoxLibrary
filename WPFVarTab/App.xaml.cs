using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace WPFVarTab
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow clientForm;
        
        private void Application_DispatcherUnhandledException(object sender,
                                                              System.Windows.Threading.
                                                                  DispatcherUnhandledExceptionEventArgs e)
        {
            var msg = e.Exception.Message;
            if (e.Exception.InnerException != null)
                msg += "\n\rInner:" + e.Exception.InnerException.Message;
            MessageBox.Show(msg + e.Exception.StackTrace, "Exception Caught",
                            MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            
            var msg = ex.Message;
            if (ex.InnerException != null)
                msg += "\n\rInner:" + ex.InnerException.Message;
            
            MessageBox.Show(msg + ex.StackTrace, "Uncaught Thread Exception",
                            MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Main(object sender, StartupEventArgs e)
        {

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            clientForm = new MainWindow();

            clientForm.Show();

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            clientForm.KillAllThreads();
        }
    }
}
