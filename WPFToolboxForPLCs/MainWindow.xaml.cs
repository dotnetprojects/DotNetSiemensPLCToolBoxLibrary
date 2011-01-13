using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using AvalonDock;
using LibNoDaveConnectionLibrary.Projectfiles;
using Microsoft.Win32;
using WPFToolboxForSiemensPLCs.DockableWindows;

namespace WPFToolboxForSiemensPLCs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            lblVersion.Text = "Version: "+ String.Format("{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            winConnections.parentDockingManager = DockManager;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
           OpenProject(false);
        }

        void OpenProject(bool showDeleted)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "All supported types (*.zip, *.s7p, *.s5d)|*.s7p;*.zip;*.s5d|Step5 Project|*.s5d|Step7 V5.5 Project|*.s7p|Zipped Step5/Step7 Project|*.zip";

            var ret = op.ShowDialog(this);
            if (ret == true)
            {
                Project prj = Projects.LoadProject(op.FileName, showDeleted);
                ProjectTree.Projects.Add(prj.ProjectStructure);
            }

            ProjectTree.parentDockingManager = DockManager;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            OpenProject(true);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (winConnections.State == DockableContentState.Hidden)
            {
                //show content as docked content
                winConnections.Show(DockManager, AnchorStyle.Right);
            }

            winConnections.Activate();           
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            ContentWindowDiffWindow tmp = new ContentWindowDiffWindow();
            //tmp.parentDockingManager = parentDockingManager;
            tmp.Title = "DiffWindow";
            //tmp.ToolTip = fld.ToString();
            tmp.Show(DockManager);
            DockManager.ActiveDocument = tmp;
        }

        private void DockablePane_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {            
            ContentWindowWinCCTagVarCreator tmp = new ContentWindowWinCCTagVarCreator();
            //tmp.parentDockingManager = parentDockingManager;
            tmp.Title = "DB2WinCC Converter";
            //tmp.ToolTip = fld.ToString();
            tmp.Show(DockManager);
            DockManager.ActiveDocument = tmp;
        }
    }
}
