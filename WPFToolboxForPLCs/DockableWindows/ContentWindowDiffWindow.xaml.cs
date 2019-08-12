using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using AvalonDock;
using DiffMatchPatch;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;

using JFKCommonLibrary.Diff;

using WPFToolboxForSiemensPLCs.AvalonEdit;

namespace WPFToolboxForSiemensPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class ContentWindowDiffWindow : DocumentContent
    {
        public ContentWindowDiffWindow()
        {
            InitializeComponent();

           

           
            InitFolding();

            DispatcherTimer foldingUpdateTimer1 = new DispatcherTimer();
            foldingUpdateTimer1.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer1.Tick += foldingUpdateTimerA_Tick;
            foldingUpdateTimer1.Start();

            string highlighterFile = "";
            highlighterFile = "WPFToolboxForSiemensPLCs.AvalonEdit.AWL_Step5_Highlighting.xshd";              
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
            txtResult.SyntaxHighlighting = customHighlighting;
           
            this.DataContext = this;
        }

        #region Folding
        FoldingManager foldingManagerA;
        BraceFoldingStrategy foldingStrategyA;
        
        void InitFolding()
        {
            if (txtResult.SyntaxHighlighting == null)
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
                    foldingManagerA = FoldingManager.Install(txtResult.TextArea);
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
        }

        void foldingUpdateTimerA_Tick(object sender, EventArgs e)
        {
            if (foldingStrategyA != null)
            {
                foldingStrategyA.UpdateFoldings(foldingManagerA, txtResult.Document);
            }

            if (foldingManagerA!=null)
                foreach (var fld in foldingManagerA.AllFoldings)
                {
                    if (txtResult.Document.Text.Substring(fld.StartOffset, 8) == "Netzwerk")
                        fld.Title = txtResult.Document.Text.Substring(fld.StartOffset, 11) + " ...";
                    else
                        fld.Title = txtResult.Document.Text.Substring(fld.StartOffset, 3) + "...";
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
            
            if (myBlock is S7FunctionBlock)
            {
                highlighterFile = "WPFToolboxForSiemensPLCs.AvalonEdit.AWL_Step7_Highlighting.xshd";

                if (((S7FunctionBlock)myBlock).Parameter != null)
                    myTreeA.DataContext = ((S7FunctionBlock)myBlock).Parameter.Children;

                textEditorA.Text = ((S7FunctionBlock)myBlock).ToString(false);
            }
            else
            {
                mainGridA.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                //toppanel.Visibility = System.Windows.Visibility.Collapsed;
                highlighterFile = "WPFToolboxForSiemensPLCs.AvalonEdit.AWL_Step5_Highlighting.xshd";
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

            if (myBlock is S7FunctionBlock)
            {
                highlighterFile = "WPFToolboxForSiemensPLCs.AvalonEdit.AWL_Step7_Highlighting.xshd";

                if (((S7FunctionBlock)myBlock).Parameter != null)
                    myTreeB.DataContext = ((S7FunctionBlock)myBlock).Parameter.Children;

                textEditorB.Text = ((S7FunctionBlock)myBlock).ToString(false);
            }
            else
            {
                mainGridB.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                //toppanel.Visibility = System.Windows.Visibility.Collapsed;
                highlighterFile = "WPFToolboxForSiemensPLCs.AvalonEdit.AWL_Step5_Highlighting.xshd";
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
                    var tmp = (S7DataRow)myTreeA.SelectedItem;
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
                    var tmp = (S7DataRow)myTreeB.SelectedItem;
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

        private void cmdCompare_Click(object sender, RoutedEventArgs e)
        {
            txtResult.TextArea.TextView.LineTransformers.Clear();

            var txt = "";
            diff_match_patch comparer=new diff_match_patch();
            var result = comparer.diff_main(textEditorA.Text, textEditorB.Text);

            txtResult.Document.Text = "";
            foreach (var diff in result)
            {
                if (diff.operation == Operation.INSERT)
                {
                    var st = txt.Length;
                    txt += diff.text;
                    var stp = txt.Length;

                    txtResult.TextArea.TextView.LineTransformers.Add(new TextColorizer(st, stp, Brushes.Green));
                }
                else if (diff.operation == Operation.DELETE)
                {
                    var st = txt.Length;
                    txt += diff.text;
                    var stp = txt.Length;

                    txtResult.TextArea.TextView.LineTransformers.Add(new TextColorizer(st, stp, Brushes.Red));
                }
                else
                {
                    txt += diff.text;
                }

            }
            txtResult.Document.Text = txt;

        }
        
        private void cmdCompare2_Click(object sender, RoutedEventArgs e)
        {
            txtResult.TextArea.TextView.LineTransformers.Clear();


            var cp1 = new DiffList_String(textEditorA.Text);
            var cp2 = new DiffList_String(textEditorB.Text);
            var df = new DiffEngine();
            df.ProcessDiff(cp1, cp2, DiffEngineLevel.SlowPerfect);
            var result = df.DiffReport();

            var txt = "";
            //diff_match_patch comparer=new diff_match_patch();
            //var result = comparer.diff_main(textEditorA.Text, textEditorB.Text);
            
            txtResult.Document.Text = "";
            foreach (var diff in result)
            {
                if (diff.Status == DiffResultSpanStatus.AddDestination)
                {
                    var st = txt.Length;
                    for (int i = diff.DestIndex; i < diff.DestIndex + diff.Length; i++)
                    {
                        txt += ((TextLine)cp2.GetByIndex(i)).Line + Environment.NewLine; ;
                    }
                    var stp = txt.Length;

                    txtResult.TextArea.TextView.LineTransformers.Add(new TextColorizer(st, stp, Brushes.Green));
                }
                else if (diff.Status == DiffResultSpanStatus.DeleteSource)
                {
                    var st = txt.Length;
                    for (int i = diff.SourceIndex; i < diff.SourceIndex + diff.Length; i++)
                    {
                        txt += ((TextLine)cp1.GetByIndex(i)).Line + Environment.NewLine; ;
                    }
                    var stp = txt.Length;
                    txtResult.TextArea.TextView.LineTransformers.Add(new TextColorizer(st, stp, Brushes.Red));
                }
                else if (diff.Status == DiffResultSpanStatus.Replace)
                {
                    var st = txt.Length;
                    for (int i = diff.DestIndex; i < diff.DestIndex + diff.Length; i++)
                    {
                        txt += ((TextLine)cp2.GetByIndex(i)).Line + Environment.NewLine;
                    }
                    var stp = txt.Length;
                    txtResult.TextArea.TextView.LineTransformers.Add(new TextColorizer(st, stp, Brushes.Orange));
                }
                else
                {
                    for (int i = diff.SourceIndex; i < diff.Length; i++)
                    {
                        txt += ((TextLine)cp1.GetByIndex(i)).Line + Environment.NewLine; ;
                    }  
                }
                
            }
            txtResult.Document.Text = txt;

        }

    }
}
