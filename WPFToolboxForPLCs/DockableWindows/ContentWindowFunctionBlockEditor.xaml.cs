using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using WPFToolboxForSiemensPLCs.AvalonEdit;

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

            string highlighterFile = "";

            if (myBlock is PLCFunctionBlock)
            {
                highlighterFile="WPFToolboxForSiemensPLCs.AvalonEdit.AWL_Step7_Highlighting.xshd";

                if (((PLCFunctionBlock) myBlock).Parameter != null)
                    myTree.DataContext = ((PLCFunctionBlock) myBlock).Parameter.Children;

                textEditor.Text = ((PLCFunctionBlock) myBlock).ToString(false);
            }
            else
            {
                mainGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                //toppanel.Visibility = System.Windows.Visibility.Collapsed;
                highlighterFile = "WPFToolboxForSiemensPLCs.AvalonEdit.AWL_Step5_Highlighting.xshd";
                textEditor.Text = myBlock.ToString();                
            }
           
            
            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream(highlighterFile))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new string[] { ".cool" }, customHighlighting);

            textEditor.SyntaxHighlighting = customHighlighting;

            InitFolding();

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();

            
            this.DataContext = this;
        }

        #region Folding
        FoldingManager foldingManager;
        AbstractFoldingStrategy foldingStrategy;

        void InitFolding()
        {
            if (textEditor.SyntaxHighlighting == null)
            {
                foldingStrategy = null;
            }
            else
            {
                foldingStrategy = new BraceFoldingStrategy();
                
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

        ToolTip toolTip = new ToolTip();

        private void textEditor_MouseHover(object sender, MouseEventArgs e)
        {
            var pos = textEditor.GetPositionFromPoint(e.GetPosition(textEditor));
            if (pos != null)
            {
                int off = textEditor.Document.GetOffset(pos.Value.Line, pos.Value.Column);
                foreach (var fld in foldingManager.AllFoldings)
                {
                    if (fld.StartOffset <= off && off <= fld.EndOffset && fld.IsFolded)
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
                { }
            }
        }

        private void DocumentContent_IsActiveDocumentChanged(object sender, EventArgs e)
        {
            if (this.IsActiveDocument)
                App.clientForm.PrintData = myBlockString;
            else
                App.clientForm.PrintData = null;
        }
    }
}
