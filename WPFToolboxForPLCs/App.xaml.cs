using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace WPFToolboxForPLCs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow clientForm;
 
        void Main(object sender, StartupEventArgs e)
        {

            clientForm = new MainWindow();

            clientForm.Show();

        }
    }
}
