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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFVarTab
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

        private void _OnShowSystemMenuCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Window _window = (Window)e.Parameter;
            Point _point = new Point(_window.Left + 24, _window.Top + 24);

            Microsoft.Windows.Shell.SystemCommands.ShowSystemMenu(_window, _point);
        }

        private void _OnSystemCommandCloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Windows.Shell.SystemCommands.CloseWindow((Window)e.Parameter);
        }

    }


}
