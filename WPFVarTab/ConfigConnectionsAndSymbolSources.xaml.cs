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
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var dicEntry = (KeyValuePair<string, ISymbolTable>)btn.DataContext;

            var symb=SelectProjectPart.SelectSymbolTable();

            if (symb != null)
                MainWindow.DictonaryConnectionSymboltables[dicEntry.Key] = symb;
        }
    }
}
