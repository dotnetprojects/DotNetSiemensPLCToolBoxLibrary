using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using LibNoDaveConnectionLibrary;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using LibNoDaveConnectionLibrary.DataTypes.Step7Project;
using LibNoDaveConnectionLibrary.DataTypes.Blocks;
using LibNoDaveConnectionLibrary;
using LibNoDaveConnectionLibrary.Projectfiles;

namespace TestWpfC
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
        IHighlightingDefinition customHighlighting;

        public MainWindow()
        {
            // Load our custom highlighting definition
            //IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("TestWpfC.CustomHighlighting.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new string[] { ".cool" }, customHighlighting);


            InitializeComponent();


            textEditor.SyntaxHighlighting = customHighlighting;

            HighlightingComboBox_SelectionChanged(null, null);

            //textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            //textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();
        }

        private LibNoDaveConnectionLibrary.LibNoDaveConnection _myconn;

        private const string _connname = "AWLEditPad";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LibNoDaveConnectionLibrary.Configuration.ShowConfiguration(_connname, true);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (_myconn == null)
            {
                _myconn = new LibNoDaveConnection(_connname);
                _myconn.Connect(1);

                foreach (string itm in _myconn.PLCListBlocks(LibNoDaveConnectionLibrary.DataTypes.PLCBlockType.AllEditableBlocks))
                    BlockList.Items.Add(itm);
            }
            else
            {
                if (dispatcherTimer!=null)
                    dispatcherTimer.Stop();
                if (myDiag != null)
                {
                    myDiag.Close();
                    myDiag.RemoveDiagnosticData();
                    textEditor.Text = myBlock.ToString();
                }
                _myconn.Dispose();
                _myconn = null;
                myDiag = null;
                BlockList.Items.Clear();
                blockName.Content = "FCxx";
                Upload.IsEnabled = false;
                Optimize.IsEnabled = false;
                Diag.IsEnabled = false;                             
            }

        }

        private PLCBlock myBlock;

        private void BlockList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string nm = BlockList.SelectedItem.ToString();
            if (dispatcherTimer != null)
                dispatcherTimer.Stop();
            if (myDiag != null)
            {
                myDiag.Close();
                myDiag = null;
            }
            myBlock = LibNoDaveConnectionLibrary.MC7.MC7Converter.GetAWLBlock(_myconn.PLCGetBlockInMC7(nm), 0, myblkFld);
            if (myBlock != null)
            {
                if (myBlock.BlockType == LibNoDaveConnectionLibrary.DataTypes.PLCBlockType.DB)
                {
                    toppanel.ClearValue(HeightProperty);
                    toppanel.ClearValue(DockPanel.DockProperty);
                    textEditor.Visibility = System.Windows.Visibility.Collapsed;

                }
                else
                {
                    toppanel.Height = 160;
                    toppanel.SetValue(DockPanel.DockProperty,Dock.Top);
                    textEditor.Visibility = System.Windows.Visibility.Visible;
                    
                }
                textEditor.Text = myBlock.ToString();
                blockName.Content = nm;
                Upload.IsEnabled = true;
                Optimize.IsEnabled = true;
                Diag.IsEnabled = true;

                if (myBlock.BlockType == LibNoDaveConnectionLibrary.DataTypes.PLCBlockType.DB)
                    myTree.DataContext = ((PLCDataBlock)myBlock).Structure.Children;
                else
                    myTree.DataContext = ((PLCFunctionBlock)myBlock).Parameter.Children;
                
            }
        }

        #region Folding
        FoldingManager foldingManager;
        AbstractFoldingStrategy foldingStrategy;

        void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (textEditor.SyntaxHighlighting == null)
            {
                foldingStrategy = null;
            }
            else
            {
                foldingStrategy = new BraceFoldingStrategy();
                /*
                switch (textEditor.SyntaxHighlighting.Name)
                {
                    case "XML":
                        foldingStrategy = new XmlFoldingStrategy();
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        break;
                    case "C#":
                    case "C++":
                    case "PHP":
                    case "Java":
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
                        
                        break;
                    default:
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        foldingStrategy = null;
                        break;
                }*/
            }
            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(textEditor.TextArea);
                foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            }
            else
            {
                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }
            }
        }

        void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (foldingStrategy != null)
            {
                foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            }
            foreach (var fld in foldingManager.AllFoldings)
            {
                if (textEditor.Document.Text.Substring(fld.StartOffset, 8) == "Netzwerk")
                    fld.Title = textEditor.Document.Text.Substring(fld.StartOffset, 11) + " ...";
                else
                    fld.Title = textEditor.Document.Text.Substring(fld.StartOffset, 3) + "...";
            }

        }
        #endregion

        private void Optimize_Click(object sender, RoutedEventArgs e)
        {
            LibNoDaveConnectionLibrary.MC7.AWLCodeOptimizer.OptimizeAWL((LibNoDaveConnectionLibrary.DataTypes.Blocks.PLCFunctionBlock) myBlock, 0);
            textEditor.Text = myBlock.ToString();
        }


        ToolTip toolTip = new ToolTip();

        private void textEditor_MouseHover(object sender, MouseEventArgs e)
        {
            var pos = textEditor.GetPositionFromPoint(e.GetPosition(textEditor));
            if (pos != null)
            {
                int off = textEditor.Document.GetOffset(pos.Value.Line, pos.Value.Column);
                foreach (var fld in foldingManager.AllFoldings)
                {
                    if (fld.StartOffset<=off && off<=fld.EndOffset && fld.IsFolded)
                    {
                        toolTip.PlacementTarget = this;

                        toolTip.Content = new ICSharpCode.AvalonEdit.TextEditor { Template = (ControlTemplate)this.Resources["TemplateEditor"], Text = textEditor.Document.Text.Substring(fld.StartOffset, fld.EndOffset - fld.StartOffset), SyntaxHighlighting = customHighlighting, FontFamily = new FontFamily("Consolas"), Opacity = 0.6 };

                        toolTip.IsOpen = true;
                        e.Handled = true;   
                    }
                }


                
            }
        }

        private void textEditor_MouseHoverStopped(object sender, MouseEventArgs e)
        {
            toolTip.IsOpen = false;
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            byte[] val = LibNoDaveConnectionLibrary.MC7.MC7Converter.GetMC7Block(myBlock);

        }

        private LibNoDaveConnection.DiagnosticData myDiag;

        private DispatcherTimer dispatcherTimer;
        
        private void Diag_Click(object sender, RoutedEventArgs e)
        {
            errors.Content = "";

            PLCFunctionBlockRow.SelectedStatusValues tmp = 0;
            tmp |= (bool)stw.IsChecked ? PLCFunctionBlockRow.SelectedStatusValues.STW : 0;
            tmp |= (bool)akku1.IsChecked ? PLCFunctionBlockRow.SelectedStatusValues.Akku1 : 0;
            tmp |= (bool)akku2.IsChecked ? PLCFunctionBlockRow.SelectedStatusValues.Akku2 : 0;
            tmp |= (bool)ar1.IsChecked ? PLCFunctionBlockRow.SelectedStatusValues.AR1 : 0;
            tmp |= (bool)ar2.IsChecked ? PLCFunctionBlockRow.SelectedStatusValues.AR2 : 0;
            tmp |= (bool)db.IsChecked ? PLCFunctionBlockRow.SelectedStatusValues.DB : 0;

            if (myDiag == null)
            {
                try
                {
                    if (myDiag == null)
                        myDiag = _myconn.startRequestDiagnosticData((PLCFunctionBlock) myBlock,
                                                                    Int32.Parse(startRow.Text), tmp);
                    myDiag.RequestDiagnosticData();
                    textEditor.Text = myBlock.ToString();
                }
                catch (Exception ex)
                {
                    errors.Content = ex.Message;
                    if (myDiag != null)
                    {
                        myDiag.Close();
                        myDiag.RemoveDiagnosticData();
                    }
                    textEditor.Text = myBlock.ToString();
                    myDiag = null;
                }

                if (myDiag != null)
                {
                    dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
                    dispatcherTimer.Start();
                }
            }
            else
            {
                dispatcherTimer.Stop();
                myDiag.Close();
                myDiag.RemoveDiagnosticData();
                textEditor.Text = myBlock.ToString();
                myDiag = null;
            }
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                myDiag.RequestDiagnosticData();
                textEditor.Text = myBlock.ToString();
            }
            catch (Exception ex)
            {
                errors.Content = ex.Message;
                if (myDiag != null)
                {
                    myDiag.Close();
                    myDiag.RemoveDiagnosticData();
                }
                dispatcherTimer.Stop();
                textEditor.Text = myBlock.ToString();
                myDiag = null;
            }
        }

        private void myTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (myTree.SelectedItem != null)
            {
                try
                {
                    var tmp = (PLCDataRow)myTree.SelectedItem;
                    if (tmp.Children != null && tmp.Children.Count >= 1)
                        myDGrid.DataContext = tmp;
                    else
                    {
                        myDGrid.DataContext = tmp.Parent;
                        myDGrid.SelectedItem = tmp;
                    }
                }
                catch (Exception)
                {}
            }
        }

        private void BlockList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private S7ProgrammFolder myblkFld;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            myblkFld = SelectProjectPart.SelectS7ProgrammFolder();
        }

        private void myDGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
