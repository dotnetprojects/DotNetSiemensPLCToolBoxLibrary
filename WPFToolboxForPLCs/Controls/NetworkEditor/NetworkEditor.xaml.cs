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

namespace WPFToolboxForSiemensPLCs.Controls.NetworkEditor
{
    public partial class NetworkEditor : UserControl 
    {



        public int NetworkNumber
        {
            get { return (int)GetValue(NetworkNumberProperty); }
            set { SetValue(NetworkNumberProperty, value); }
        }

        public void ShowLine(int linenr, int selstart, int sellen)
        {
            myExpander.IsExpanded = true;
            textEditor.ScrollToLine(linenr);

            /*
            int zeile = 0;
            int start = 0;
            int anz = 0;
            int len = 0;
            
            foreach (char c in textEditor.Text)
            {
                anz++;
                if (c == 13)
                {
                    zeile++;

                    if (zeile == linenr - 1)
                        start = anz;
                    else if (zeile == linenr)
                    {
                        len = anz - start;
                        break;
                    }
                }
            }
            textEditor.SelectionStart = start + 1;
            if (len == 0)
                len = textEditor.Text.Length - start;
            textEditor.SelectionLength = len - 1;
             * */

            textEditor.SelectionStart = selstart;
            textEditor.SelectionLength = sellen;
            
            //textEditor.SelectionStart = textEditor.TextArea.TextView.GetVisualLine(linenr).StartOffset;
            //textEditor.SelectionLength = textEditor.TextArea.TextView.GetVisualLine(linenr + 1).StartOffset - textEditor.SelectionStart;
            textEditor.Focus();
        }

        // Using a DependencyProperty as the backing store for NetworkNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NetworkNumberProperty =
            DependencyProperty.Register("NetworkNumber", typeof(int), typeof(NetworkEditor), new UIPropertyMetadata(0));

        

        public Network DisplayNetwork
        {
            get { return (Network)GetValue(DisplayNetworkProperty); }
            set { SetValue(DisplayNetworkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayNetwork.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayNetworkProperty =
            DependencyProperty.Register("DisplayNetwork", typeof(Network), typeof(NetworkEditor), new FrameworkPropertyMetadata(null, OnDisplayNetworkChanged, CoerceValueCallback));
        
        private static Object CoerceValueCallback(DependencyObject d,Object baseValue)
        {
            return baseValue;
        }

        private static void OnDisplayNetworkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Network nw = (Network) e.NewValue;

            NetworkEditor nwEdt = (NetworkEditor) d;

            string highlighterFile = "";

            if (nw.Parent is S7FunctionBlock)
            {
                highlighterFile = "WPFToolboxForSiemensPLCs.AvalonEdit.AWL_Step7_Highlighting.xshd";

                nwEdt.textEditor.Text =((Network)nw).AWLCodeToString();
            }
            else
            {
                //toppanel.Visibility = System.Windows.Visibility.Collapsed;
                highlighterFile = "WPFToolboxForSiemensPLCs.AvalonEdit.AWL_Step5_Highlighting.xshd";
                nwEdt.textEditor.Text = ((Network)nw).AWLCodeToString();
            }

            

            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream(highlighterFile))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    nwEdt.customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new string[] { ".cool" }, nwEdt.customHighlighting);

            nwEdt.textEditor.SyntaxHighlighting = nwEdt.customHighlighting;

            nwEdt.InitFolding();
            
        }


        internal IHighlightingDefinition customHighlighting;
        public NetworkEditor()
        {
            InitializeComponent();

            foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;           
        }

        /*
        public Network DisplayNetwork
        {
            set
            {
                Network nw = value;

                NetworkEditor nwEdt = this;
                if (nw.Parent is S7FunctionBlock)
                {
                    nwEdt.highlighterFile = "WPFToolboxForSiemensPLCs.AvalonEdit.AWL_Step7_Highlighting.xshd";

                    nwEdt.textEditor.Text = nw.ToString();
                }
                else
                {
                    //toppanel.Visibility = System.Windows.Visibility.Collapsed;
                    nwEdt.highlighterFile = "WPFToolboxForSiemensPLCs.AvalonEdit.AWL_Step5_Highlighting.xshd";
                    nwEdt.textEditor.Text = nw.ToString();
                }
            }
        }
        */

        #region Folding

        private DispatcherTimer foldingUpdateTimer;
        FoldingManager foldingManager;
        BraceFoldingStrategy foldingStrategy;

        internal void InitFolding()
        {

            foldingUpdateTimer.Stop();

            if (textEditor.SyntaxHighlighting == null)
            {
                foldingStrategy = null;
            }
            else
            {
                foldingStrategy = new BraceFoldingStrategy();
                foldingUpdateTimer.Start();
                
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
                if (foldingManager != null)
                    foreach (var fld in foldingManager.AllFoldings)
                    {
                        if (fld.StartOffset <= off && off <= fld.EndOffset && fld.IsFolded)
                        {
                            toolTip.PlacementTarget = this;

                            toolTip.Content = new ICSharpCode.AvalonEdit.TextEditor
                                                  {
                                                      Template = (ControlTemplate) this.Resources["TemplateEditor"],
                                                      Text =
                                                          textEditor.Document.Text.Substring(fld.StartOffset,
                                                                                             fld.EndOffset -
                                                                                             fld.StartOffset),
                                                      SyntaxHighlighting = customHighlighting,
                                                      FontFamily = new FontFamily("Consolas"),
                                                      Opacity = 0.6
                                                  };
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
    }
}
