using System;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using ICSharpCode.AvalonEdit.Highlighting;
using JFKCommonLibrary.WPF;
using WPFToolboxForSiemensPLCs.Controls.NetworkEditor;

namespace WPFToolboxForSiemensPLCs.DockableWindows
{
    public partial class ContentWindowFunctionBlockEditor : DocumentContent
    {
        IHighlightingDefinition customHighlighting;

        private Block myBlock;
        private string myBlockString;

        public ContentWindowFunctionBlockEditor(object myBlock)
        {
            InitializeComponent();

            this.showBlock(myBlock, 0, 0);
        }

        public ContentWindowFunctionBlockEditor(object myBlock, int netzwerknr, int zeile)
        {
            InitializeComponent();

            this.showBlock(myBlock, netzwerknr, zeile);
        }

        private void showBlock(object myBlock, int netzwerknr, int zeile)
        {
            this.netzwerknr = netzwerknr;
            this.zeile = zeile;

            this.myBlock = (Block)myBlock;
            myBlockString = this.myBlock.ToString();

            if (myBlock is S7FunctionBlock)
            {
                if (((S7FunctionBlock)myBlock).Parameter != null)
                    myTree.DataContext = ((S7FunctionBlock)myBlock).Parameter.Children;

                myLst.ItemsSource = ((S7FunctionBlock)myBlock).Networks;
            }
            else
            {
                mainGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                myLst.ItemsSource = ((S5FunctionBlock)myBlock).Networks;
            }
            this.DataContext = this;

            if (netzwerknr > 0)
            {
                myLst.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
                
            }
        }

        private int netzwerknr=0;
        private int zeile = 0;
        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (myLst.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                myLst.ItemContainerGenerator.StatusChanged -= new EventHandler(ItemContainerGenerator_StatusChanged);

                myLst.ScrollIntoView(((S5FunctionBlock) myBlock).Networks[netzwerknr - 1]);
                DependencyObject depObj = myLst.ItemContainerGenerator.ContainerFromIndex(netzwerknr - 1);

                NetworkEditor nedt = depObj.TryFindChild<NetworkEditor>();

                Network netw = ((S5FunctionBlock) myBlock).Networks[netzwerknr - 1];
                int anz = 0;
                for (int q = 0; q < zeile-1; q++)
                    anz += netw.AWLCode[q].ToString().Length + 2;

                nedt.ShowLine(zeile, anz, netw.AWLCode[zeile-1].ToString().Length);
            }
        }

        private void myTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (myTree.SelectedItem != null)
            {
                try
                {
                    var tmp = (S7DataRow)myTree.SelectedItem;
                    if (tmp.Children != null && tmp.Children.Count >= 1)
                        myDGrid.DataContext = tmp;
                    else
                    {
                        myDGrid.DataContext = tmp.Parent;
                        myDGrid.SelectedItem = tmp;
                    }
                }
                catch (Exception)
                { }
            }
        }

        private PLCConnection.DiagnosticData MyDiagnosticData;

        DispatcherTimer diagTimer = null;

        /*
        {
            get { return (PLCConnection.DiagnosticData)GetValue(MyDiagnosticDataProperty); }
            set { SetValue(MyDiagnosticDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyDiagnosticData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyDiagnosticDataProperty =
            DependencyProperty.Register("MyDiagnosticData", typeof(PLCConnection.DiagnosticData), typeof(ContentWindowFunctionBlockEditor), new UIPropertyMetadata(null));
        */

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            
        }



        public S7FunctionBlockRow getFirstVisbleExpandedItem()
        {

            Network nw = null;

            VirtualizingStackPanel panel = UIHelpers.TryFindChild<VirtualizingStackPanel>(myLst);
            //ScrollViewer panel = UIHelpers.TryFindChild<ScrollViewer>(myLst);
            if (myLst.Items.Count > 0 && panel != null)
            {
                int offset = (panel.Orientation == Orientation.Horizontal) ? (int) panel.HorizontalOffset : (int) panel.VerticalOffset;

                //var item = UIHelpers.TryFindFromPoint<ListBoxItem>(myLst, new Point(myLst.po,  panel.HorizontalOffset));

                var item = myLst.Items[offset];

                int idx = myLst.Items.IndexOf(item);

                for (int n = idx; n < myLst.Items.Count; n++)
                {
                    var exp = UIHelpers.TryFindChild<Expander>(myLst.ItemContainerGenerator.ContainerFromIndex(n));
                    if (exp.IsExpanded)
                    {
                        return (S7FunctionBlockRow) ((S7FunctionBlockNetwork) myLst.Items[n]).AWLCode[0];
                    }
                }
            }
            return null;
        }

        public void viewBlockStatus()
        {
            App.clientForm.lblStatus.Text = "";

            //myLst.sc

            if (myBlock is S7FunctionBlock)
            {
                try
                {

                    var visRow = getFirstVisbleExpandedItem();
                    
                    int bytepos = 0;

                    if (visRow!=null)
                        foreach (var row in ((S7FunctionBlock)myBlock).AWLCode)
                        {
                            if (visRow == row)
                                break;
                            bytepos += ((S7FunctionBlockRow) row).ByteSize;
                        }


                    S7FunctionBlock myS7Blk = (S7FunctionBlock)myBlock;
                    MyDiagnosticData = App.clientForm.Connection.PLCstartRequestDiagnosticData(myS7Blk, bytepos, S7FunctionBlockRow.SelectedStatusValues.ALL);

                    if (diagTimer == null)
                    {
                        diagTimer = new DispatcherTimer(); // DispatcherTimer();
                        diagTimer.Tick += new EventHandler(diagTimer_Tick);
                        diagTimer.Interval = new TimeSpan(0, 0, 0, 0, 40);
                    }
                    diagTimer.Start();
                }
                catch (Exception ex)
                {
                    App.clientForm.lblStatus.Text = ex.Message;
                    if (diagTimer != null)
                    {
                        diagTimer.Stop();
                        diagTimer = null;
                    }
                }
                //diagTimer.Start();                                               
            }
        }

        void diagTimer_Tick(object sender, EventArgs e)
        {
            diagTimer.Stop();
            MyDiagnosticData.RequestDiagnosticData();
            diagTimer.Start();
        }

        public void unviewBlockStatus()
        {
            if (diagTimer != null)
            {
                diagTimer.Stop();
                diagTimer = null;
            }
            if (MyDiagnosticData != null)
            {
                MyDiagnosticData.Close();
                MyDiagnosticData.RemoveDiagnosticData();
                MyDiagnosticData = null;
            }
        }

        private void DocumentContent_IsActiveDocumentChanged(object sender, EventArgs e)
        {
            if (this.IsActiveDocument)
            {
                App.activeDocument = this;
                App.clientForm.PrintData = myBlockString;
            }
            else
            {
                App.activeDocument = null;
                App.clientForm.PrintData = null;
            }
        }

        private void DocumentContent_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            unviewBlockStatus();
        }
    }
}
