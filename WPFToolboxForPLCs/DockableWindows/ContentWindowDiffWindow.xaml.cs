using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using AvalonDock;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using LibNoDaveConnectionLibrary.DataTypes.Blocks;
using WPFToolboxForPLCs.AvalonEdit;

namespace WPFToolboxForPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class ContentWindowDiffWindow : DocumentContent
    {
        public ContentWindowDiffWindow()
        {
            InitializeComponent();

            string highlighterFile = "";

           
            InitFolding();

            DispatcherTimer foldingUpdateTimer1 = new DispatcherTimer();
            foldingUpdateTimer1.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer1.Tick += foldingUpdateTimerA_Tick;
            foldingUpdateTimer1.Start();

            DispatcherTimer foldingUpdateTimer2 = new DispatcherTimer();
            foldingUpdateTimer2.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer2.Tick += foldingUpdateTimerB_Tick;
            foldingUpdateTimer2.Start();


            this.DataContext = this;
        }

        #region Folding
        FoldingManager foldingManagerA;
        FoldingManager foldingManagerB;
        AbstractFoldingStrategy foldingStrategyA;
        AbstractFoldingStrategy foldingStrategyB;

        void InitFolding()
        {
            if (textEditorA.SyntaxHighlighting == null)
            {
                foldingStrategyA = null;
            }
            else
            {
                foldingStrategyA = new BraceFoldingStrategy();

            }
            if (foldingStrategyA != null)
            {
                if (foldingManagerA == null)
                    foldingManagerA = FoldingManager.Install(textEditorA.TextArea);
                foldingStrategyA.UpdateFoldings(foldingManagerA, textEditorA.Document);
            }
            else
            {
                if (foldingManagerA != null)
                {
                    FoldingManager.Uninstall(foldingManagerA);
                    foldingManagerA = null;
                }
            }



            if (textEditorB.SyntaxHighlighting == null)
            {
                foldingStrategyB = null;
            }
            else
            {
                foldingStrategyB = new BraceFoldingStrategy();

            }
            if (foldingStrategyB != null)
            {
                if (foldingManagerB == null)
                    foldingManagerB = FoldingManager.Install(textEditorB.TextArea);
                foldingStrategyB.UpdateFoldings(foldingManagerB, textEditorB.Document);
            }
            else
            {
                if (foldingManagerB != null)
                {
                    FoldingManager.Uninstall(foldingManagerB);
                    foldingManagerB = null;
                }
            }

        }

        void foldingUpdateTimerA_Tick(object sender, EventArgs e)
        {
            if (foldingStrategyA != null)
            {
                foldingStrategyA.UpdateFoldings(foldingManagerA, textEditorA.Document);
            }

            if (foldingManagerA!=null)
                foreach (var fld in foldingManagerA.AllFoldings)
                {
                    if (textEditorA.Document.Text.Substring(fld.StartOffset, 8) == "Netzwerk")
                        fld.Title = textEditorA.Document.Text.Substring(fld.StartOffset, 11) + " ...";
                    else
                        fld.Title = textEditorA.Document.Text.Substring(fld.StartOffset, 3) + "...";
                }

        }

        void foldingUpdateTimerB_Tick(object sender, EventArgs e)
        {
            if (foldingStrategyB != null)
            {
                foldingStrategyB.UpdateFoldings(foldingManagerB, textEditorB.Document);
            }
            if (foldingManagerB != null)
                foreach (var fld in foldingManagerB.AllFoldings)
                {
                    if (textEditorB.Document.Text.Substring(fld.StartOffset, 8) == "Netzwerk")
                        fld.Title = textEditorB.Document.Text.Substring(fld.StartOffset, 11) + " ...";
                    else
                        fld.Title = textEditorB.Document.Text.Substring(fld.StartOffset, 3) + "...";
                }

        }
        #endregion

        private void textEditorA_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        void LoadPrjA(object dta)
        {

            DataObject tmp = (DataObject) dta;
            ProjectBlockInfo blkInf = (ProjectBlockInfo)tmp.GetData("ProjectBlockInfo");

            Block myBlock = blkInf.GetBlock();
            string highlighterFile = "";
            
            if (myBlock is PLCFunctionBlock)
            {
                highlighterFile = "WPFToolboxForPLCs.AvalonEdit.AWL_Step7_Highlighting.xshd";

                if (((PLCFunctionBlock)myBlock).Parameter != null)
                    myTreeA.DataContext = ((PLCFunctionBlock)myBlock).Parameter.Children;

                textEditorA.Text = ((PLCFunctionBlock)myBlock).ToString(false);
            }
            else
            {
                mainGridA.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                //toppanel.Visibility = System.Windows.Visibility.Collapsed;
                highlighterFile = "WPFToolboxForPLCs.AvalonEdit.AWL_Step5_Highlighting.xshd";
                textEditorA.Text = myBlock.ToString();
            }

            IHighlightingDefinition customHighlighting;
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

            textEditorA.SyntaxHighlighting = customHighlighting;
           
        }

        void LoadPrjB(object dta)
        {

            DataObject tmp = (DataObject)dta;
            ProjectBlockInfo blkInf = (ProjectBlockInfo)tmp.GetData("ProjectBlockInfo");

            Block myBlock = blkInf.GetBlock();
            string highlighterFile = "";

            if (myBlock is PLCFunctionBlock)
            {
                highlighterFile = "WPFToolboxForPLCs.AvalonEdit.AWL_Step7_Highlighting.xshd";

                if (((PLCFunctionBlock)myBlock).Parameter != null)
                    myTreeB.DataContext = ((PLCFunctionBlock)myBlock).Parameter.Children;

                textEditorB.Text = ((PLCFunctionBlock)myBlock).ToString(false);
            }
            else
            {
                mainGridB.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                //toppanel.Visibility = System.Windows.Visibility.Collapsed;
                highlighterFile = "WPFToolboxForPLCs.AvalonEdit.AWL_Step5_Highlighting.xshd";
                textEditorB.Text = myBlock.ToString();
            }

            IHighlightingDefinition customHighlighting;
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

            textEditorB.SyntaxHighlighting = customHighlighting;

        }
        private void textEditorA_Drop(object sender, DragEventArgs e)
        {
            LoadPrjA(e.Data);
        }

        private void myDGridA_Drop(object sender, DragEventArgs e)
        {
            LoadPrjA(e.Data);
        }

        private void myTreeA_Drop(object sender, DragEventArgs e)
        {
            LoadPrjA(e.Data);
        }

        private void myTreeB_Drop(object sender, DragEventArgs e)
        {
            LoadPrjB(e.Data);
        }

        private void myDGridB_Drop(object sender, DragEventArgs e)
        {
            LoadPrjB(e.Data);
        }

        private void textEditorB_Drop(object sender, DragEventArgs e)
        {
            LoadPrjB(e.Data);
        }

        private void myTreeA_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (myTreeA.SelectedItem != null)
            {
                try
                {
                    var tmp = (PLCDataRow)myTreeA.SelectedItem;
                    if (tmp.Children != null && tmp.Children.Count >= 1)
                        myDGridA.DataContext = tmp;
                    else
                    {
                        myDGridA.DataContext = tmp.Parent;
                        myDGridA.SelectedItem = tmp;
                    }
                }
                catch (Exception)
                { }
            }
        }

        private void myTreeB_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (myTreeB.SelectedItem != null)
            {
                try
                {
                    var tmp = (PLCDataRow)myTreeB.SelectedItem;
                    if (tmp.Children != null && tmp.Children.Count >= 1)
                        myDGridB.DataContext = tmp;
                    else
                    {
                        myDGridB.DataContext = tmp.Parent;
                        myDGridB.SelectedItem = tmp;
                    }
                }
                catch (Exception)
                { }
            }
        }

        private void textEditorB_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

    }
}
