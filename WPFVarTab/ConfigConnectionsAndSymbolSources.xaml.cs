using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFVarTab
{
    /// <summary>
    /// Interaction logic for ConfigConnectionsAndSymbolSources.xaml
    /// </summary>
    public partial class ConfigConnectionsAndSymbolSources : CustomChromeLibrary.CustomChromeWindow
    {
        private MainWindow _mainWindow;
        public MainWindow MainWindow
        {
            get { return _mainWindow; }
        }

        public ConfigConnectionsAndSymbolSources(MainWindow mainWindow)
        {
            this._mainWindow = mainWindow;

            this.DataContext = this;

            InitializeComponent();
        }
    }
}
