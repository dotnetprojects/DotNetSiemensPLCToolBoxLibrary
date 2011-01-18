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
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5;
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

           if (myBlock is S7FunctionBlock)
            {
                if (((S7FunctionBlock) myBlock).Parameter != null)
                    myTree.DataContext = ((S7FunctionBlock) myBlock).Parameter.Children;

                //textEditor.Text = ((S7FunctionBlock) myBlock).ToString(false);
                myLst.ItemsSource = ((S7FunctionBlock) myBlock).Networks;
            }
            else
            {
                mainGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                myLst.ItemsSource = ((S5FunctionBlock)myBlock).Networks;
                //toppanel.Visibility = System.Windows.Visibility.Collapsed;
                //textEditor.Text = myBlock.ToString();                
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

        private void DocumentContent_IsActiveDocumentChanged(object sender, EventArgs e)
        {
            if (this.IsActiveDocument)
                App.clientForm.PrintData = myBlockString;
            else
                App.clientForm.PrintData = null;
        }
    }
}
