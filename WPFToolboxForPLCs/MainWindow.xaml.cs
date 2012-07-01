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
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.Communication.S7_xxx;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;
using JFKCommonLibrary.WPF;
using WPFToolboxForSiemensPLCs.DockableWindows;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace WPFToolboxForSiemensPLCs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public PLCConnection Connection
        {
            get { return (PLCConnection)GetValue(ConnectionProperty); }
            set { SetValue(ConnectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Connection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectionProperty =
            DependencyProperty.Register("Connection", typeof(PLCConnection), typeof(MainWindow), new UIPropertyMetadata(null));


        public string PrintData { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            lblVersion.Text = "Version: "+ String.Format("{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());

            this.DataContext = this;

            Connection = new PLCConnection("WPFToolboxForSiemensPLCs");           
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
                if (prj != null)
                    ProjectTree.Projects.Add(prj.ProjectStructure);
            }

            ProjectTree.parentDockingManager = DockManager;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            OpenProject(true);
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

        private void mnuPrint_Click(object sender, RoutedEventArgs e)
        {
            if (PrintData != null)
            {
                Paragraph p = new Paragraph();
                p.Inlines.Add(PrintData);
                FlowDocument fd = new FlowDocument(p);
                fd.FontFamily = new FontFamily("Courier New");
                fd.FontSize = 14.0;

                PrintDialog pd = new PrintDialog();
                fd.PageHeight = pd.PrintableAreaHeight;
                fd.PageWidth = pd.PrintableAreaWidth;
                fd.PagePadding = new Thickness(50);
                fd.ColumnGap = 0;
                fd.ColumnWidth = pd.PrintableAreaWidth;

                IDocumentPaginatorSource dps = fd;
                if (pd.ShowDialog().Value == true)
                    pd.PrintDocument(dps.DocumentPaginator, "WPFToolboxForSiemensPLCs");
            }
            else
            {
                MessageBox.Show(
                    "Activate the Window with the Block you wish to Print, maybe the current Window doesn't support printing!");
            }
        }

        private void mnuConfig_Click(object sender, RoutedEventArgs e)
        {
            App.clientForm.lblStatus.Text = "";
            
                Configuration.ShowConfiguration("WPFToolboxForSiemensPLCs", true);
                Connection = new PLCConnection("WPFToolboxForSiemensPLCs");
            
            
        }

        private void mnuOnlineBlocks_Click(object sender, RoutedEventArgs e)
        {
            OnlineBlocksFolder onl = new OnlineBlocksFolder(Connection);
            IBlocksFolder fld = (IBlocksFolder)onl;
            DockableContentBlockList tmp = new DockableContentBlockList(fld);
            tmp.parentDockingManager = DockManager;
            tmp.Title = fld.ToString();
            tmp.ToolTip = fld.ToString();
            tmp.Show(DockManager);
            tmp.ToggleAutoHide();

            //Set size of the parent DockablePane (it's automaticly been created!)
            DockablePane tmpPane = tmp.TryFindParent<DockablePane>();
            ResizingPanel.SetEffectiveSize(tmpPane, new Size(350, 0));

            DockManager.ActiveDocument = tmp;
        }

        private void mnuConnect_Click(object sender, RoutedEventArgs e)
        {
            App.clientForm.lblStatus.Text = "";
            try
            {
                Connection.Connect();
                var szl = Connection.PLCGetSZL(0x111, 1);
                if (szl!=null && szl.SZLDaten[0] is xy11Dataset)
                    lblStatus.Text = "MlfB der CPU: " + ((xy11Dataset) szl.SZLDaten[0]).MlfB;
            }
            catch (Exception ex)
            {
                App.clientForm.lblStatus.Text = ex.Message;
            }
        }

        private void mnuDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Connection.Disconnect();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Connection.Dispose();
        }

        private void mnuViewBlock_Click(object sender, RoutedEventArgs e)
        {
            if ( App.activeDocument is ContentWindowFunctionBlockEditor)
            {
                ContentWindowFunctionBlockEditor blkEdit = (ContentWindowFunctionBlockEditor) App.activeDocument;
                blkEdit.viewBlockStatus();
            }
        }

        private void mnuSZLWindow_Click(object sender, RoutedEventArgs e)
        {
            ContentWindowSZL tmp = new ContentWindowSZL();
            //tmp.parentDockingManager = parentDockingManager;
            tmp.Title = "SZL-Window";
            //tmp.ToolTip = fld.ToString();
            tmp.Show(DockManager);
            DockManager.ActiveDocument = tmp;
        }

        private void mnuUnViewBlock_Click(object sender, RoutedEventArgs e)
        {

            if (App.activeDocument is ContentWindowFunctionBlockEditor)
            {
                ContentWindowFunctionBlockEditor blkEdit = (ContentWindowFunctionBlockEditor)App.activeDocument;
                blkEdit.unviewBlockStatus();
            }
        }
    }
}
