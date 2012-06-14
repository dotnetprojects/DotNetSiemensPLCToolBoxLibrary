using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using AvalonDock;

namespace WPFToolboxForSiemensPLCs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow clientForm;
        public static DocumentContent activeDocument;

        private void Application_DispatcherUnhandledException(object sender,
                                                              System.Windows.Threading.
                                                                  DispatcherUnhandledExceptionEventArgs e)
        {
            //Handling the exception within the UnhandledException handler.
            MessageBox.Show(e.Exception.Message + e.Exception.StackTrace, "Exception Caught",
                            MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show(ex.Message + ex.StackTrace, "Uncaught Thread Exception",
                            MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Main(object sender, StartupEventArgs e)
        {

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            clientForm = new MainWindow();

            clientForm.Show();

        }
    }
}
