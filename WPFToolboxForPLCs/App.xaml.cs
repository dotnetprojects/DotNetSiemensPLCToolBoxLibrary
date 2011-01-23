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
 
        void Main(object sender, StartupEventArgs e)
        {

            clientForm = new MainWindow();

            clientForm.Show();

        }
    }
}
