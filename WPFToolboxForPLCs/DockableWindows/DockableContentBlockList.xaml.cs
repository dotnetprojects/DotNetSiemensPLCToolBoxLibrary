using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.General;
using JFKCommonLibrary.WPF;

namespace WPFToolboxForSiemensPLCs.DockableWindows
{
    public partial class DockableContentBlockList : DockableContent
    {
        public DockingManager parentDockingManager { get; set; }

        private IBlocksFolder myFld;

        public DockableContentBlockList(IBlocksFolder fld)
        {
            InitializeComponent();

            myFld = fld;

            this.DataContext = this;
            try
            {
                var tmp = fld.readPlcBlocksList();
                tmp.Sort(new NumericComparer<ProjectBlockInfo>());

                var groupedItems = new ListCollectionView(tmp);
                groupedItems.GroupDescriptions.Add(new PropertyGroupDescription("BlockTypeString"));

                this.myDataGrid.ItemsSource = groupedItems;
            }
            catch (Exception ex)
            {
               //parentDockingManager. 
                App.clientForm.lblStatus.Text = ex.Message;
            }

        }

        private void myDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(myDataGrid.SelectedItem!=null)
            {
                Block blk = ((ProjectBlockInfo) myDataGrid.SelectedItem).GetBlock();

                if (blk is S7FunctionBlock || blk is S5FunctionBlock)
                {
                    e.Handled = true;
                    ContentWindowFunctionBlockEditor tmp = new ContentWindowFunctionBlockEditor(blk);
                    tmp.Title = blk.BlockName;
                    tmp.ToolTip = myFld.ToString() + "\\" + tmp.Title;
                    tmp.Show(parentDockingManager);
                    parentDockingManager.ActiveDocument = tmp;
                }
                else if (blk is S7DataBlock || blk is S5DataBlock)
                {
                    e.Handled = true;
                    ContentWindowDataBlockEditor tmp = new ContentWindowDataBlockEditor(blk);
                    tmp.Title = blk.BlockName;
                    tmp.ToolTip = myFld.ToString() + "\\" + tmp.Title;
                    tmp.Show(parentDockingManager);
                    parentDockingManager.ActiveDocument = tmp;
                }
                else if (blk is S7VATBlock)
                {
                    e.Handled = true;
                    ContentWindowVarTab tmp = new ContentWindowVarTab((S7VATBlock)blk);
                    tmp.Title = blk.BlockName;
                    tmp.ToolTip = myFld.ToString() + "\\" + tmp.Title;
                    tmp.Show(parentDockingManager);
                    parentDockingManager.ActiveDocument = tmp;
                }
                else if (blk is S7SourceBlock)
                {
                    e.Handled = true;
                    ContentWindowSourceBlock tmp = new ContentWindowSourceBlock((S7SourceBlock)blk);
                    tmp.Title = blk.Name;
                    tmp.ToolTip = myFld.ToString() + "\\" + tmp.Title;
                    tmp.Show(parentDockingManager);
                    parentDockingManager.ActiveDocument = tmp;
                }
                
            }
        }


        private Point _startPoint;
        private bool IsDragging;
        private Cursor dragDropCursor;
        private void myDataGrid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);                         
        }

        private void myDataGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    //Start DragDrop Action!
                    {
                        var row = UIHelpers.TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition(myDataGrid));
                        if (row != null)
                        { 
                            DataObject dragData = new DataObject("dataRow", row);
                            dragData.SetData("ProjectBlockInfo", myDataGrid.SelectedItem);
                            if (myDataGrid.SelectedItem is S7ProjectBlockInfo)
                            {                               
                                S7ProjectBlockInfo blkInfo = (S7ProjectBlockInfo) myDataGrid.SelectedItem;
                                string awlblk = blkInfo.GetSourceBlock();

                                dragData.SetData(FileDragDropHelper.CFSTR_FILEDESCRIPTORW, FileDragDropHelper.GetFileDescriptor(blkInfo.BlockName + ".awl", awlblk));
                                dragData.SetData(FileDragDropHelper.CFSTR_FILECONTENTS, FileDragDropHelper.GetFileContents(awlblk));                                                              
                                
                                byte[] unicodeText = Encoding.Unicode.GetBytes(awlblk); // UTF-16
                                byte[] utf8Text = Encoding.UTF8.GetBytes(awlblk);
                                byte[] utf32Text = Encoding.UTF32.GetBytes(awlblk);
                                dragData.SetData(DataFormats.Text,awlblk);
                                dragData.SetData(DataFormats.UnicodeText, unicodeText);
                                dragData.SetData("UTF-8", utf8Text);
                                dragData.SetData("UTF-32", utf32Text);
                            }
                            //dragData = new DataObject(DataFormats.Text,row.ToString());
                            Mouse.OverrideCursor = dragDropCursor = new GhostCursor(row).Cursor;
                                                                                    
                            DragDrop.DoDragDrop((DependencyObject)sender, dragData, DragDropEffects.Copy);
                            Mouse.OverrideCursor = null;
                        }
                    }                  
                }
            }  
        }

        private void myDataGrid_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {


            if ((e.Effects & DragDropEffects.Copy) != DragDropEffects.None)
            {
                Mouse.OverrideCursor = dragDropCursor;
                e.Handled = true;
            }
            else
                Mouse.OverrideCursor = null;
        }

        private void DockableContent_GotFocus(object sender, RoutedEventArgs e)
        {
            App.clientForm.lblStatus.Text = this.ToolTip.ToString();
        }

        private void myDataGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void menuCreateAwl_Click(object sender, RoutedEventArgs e)
        {
            if (sender is DependencyObject)
            {
                DependencyObject depObj = (DependencyObject) e.OriginalSource;
                ContextMenu contextMenu = depObj.TryFindParent<ContextMenu>();
                DataGridCell dataCell = (DataGridCell)contextMenu.PlacementTarget;
                DataGridRow dataRow = dataCell.TryFindParent<DataGridRow>();
                DataGrid dataGrid = dataCell.TryFindParent<DataGrid>();
                ProjectBlockInfo prjBlkInfo = (ProjectBlockInfo)dataGrid.ItemContainerGenerator.ItemFromContainer(dataRow);

                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".AWL";
                dlg.Filter = "AWL-Code (.AWL)|*.AWL";
                if (dlg.ShowDialog() == true)
                {
                    string filename = dlg.FileName;
                    StreamWriter wrt = new StreamWriter(filename, false, Encoding.GetEncoding("ISO-8859-1"));

                    wrt.Write(prjBlkInfo.GetSourceBlock());
                    wrt.Close();
                }
            }
        }

        private void myDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


    }
}
