using System;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using ICSharpCode.AvalonEdit.Highlighting;

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

            this.myBlock = (Block)myBlock;
            myBlockString = this.myBlock.ToString();

           if (myBlock is S7FunctionBlock)
            {
                if (((S7FunctionBlock) myBlock).Parameter != null)
                    myTree.DataContext = ((S7FunctionBlock) myBlock).Parameter.Children;

                myLst.ItemsSource = ((S7FunctionBlock) myBlock).Networks;
            }
            else
            {
                mainGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                myLst.ItemsSource = ((S5FunctionBlock)myBlock).Networks;
            }
            this.DataContext = this;
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

        public void viewBlockStatus()
        {
            App.clientForm.lblStatus.Text = "";

            if (myBlock is S7FunctionBlock)
            {
                try
                {
                    S7FunctionBlock myS7Blk = (S7FunctionBlock) myBlock;
                    MyDiagnosticData = App.clientForm.Connection.PLCstartRequestDiagnosticData(myS7Blk, 0,
                                                                                               S7FunctionBlockRow.
                                                                                                   SelectedStatusValues.
                                                                                                   ALL);

                    if (diagTimer == null)
                    {
                        diagTimer = new DispatcherTimer(); // DispatcherTimer();
                        diagTimer.Tick += new EventHandler(diagTimer_Tick);
                        diagTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                    }
                    diagTimer.Start();
                }
                catch(Exception ex)
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
            diagTimer.Stop();
            diagTimer = null;
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
