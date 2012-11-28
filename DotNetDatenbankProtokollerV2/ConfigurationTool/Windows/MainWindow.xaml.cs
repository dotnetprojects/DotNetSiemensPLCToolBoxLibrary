using System;
using System.Windows;
using System.Windows.Controls;
using DotNetSimaticDatabaseProtokollerLibrary;

namespace DotNetSimaticDatabaseProtokollerConfigurationTool.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private UserControl subWindow = null;

        private void itmConnections_Selected(object sender, RoutedEventArgs e)
        {
            sideContent.Content = new EditConnections();
            e.Handled = true;
        }

        private void itmStorages_Selected(object sender, RoutedEventArgs e)
        {
            sideContent.Content = new EditStorages();
            e.Handled = true;
        }

        private void itmProtocolDatasets_Selected(object sender, RoutedEventArgs e)
        {
            sideContent.Content = new EditProtocolDatasets();
            e.Handled = true;
        }       

        private void itmProtocoling_Selected(object sender, RoutedEventArgs e)
        {
            sideContent.Content = new Protocoling();
            e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ProtokollerConfiguration.Load();
            }
            catch (Exception ex)
            { }
        }

        private void itmDatabaseViewer_Selected(object sender, RoutedEventArgs e)
        {
            sideContent.Content = new ViewStorrages();
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ProtokollerConfiguration.ActualConfigInstance.isDirty)
            {
                var ret = MessageBox.Show("Konfiguration wurde geändert und noch nicht gespeichert! Trotzdem Schließen ?", "Schließen ?", MessageBoxButton.YesNo);
                if (ret == MessageBoxResult.No)
                    e.Cancel = true;
            }
            IDisposable disp = sideContent.Content as IDisposable;
            if (disp != null)
                disp.Dispose();
        }

        private void itmTest_Selected(object sender, RoutedEventArgs e)
        {
            sideContent.Content = new SubwindowTest();
            e.Handled = true;            
        }

        private void itmInformation_Selected(object sender, RoutedEventArgs e)
        {
            sideContent.Content = new SubwindowInformation();
            e.Handled = true;    
        }

        private void itmService_Selected(object sender, RoutedEventArgs e)
        {
            sideContent.Content = new SubwindowService();
            e.Handled = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private void itmCommon_Selected(object sender, RoutedEventArgs e)
        {
            sideContent.Content = new EditCommonSettings();
            e.Handled = true;
        }

       

    }
}
