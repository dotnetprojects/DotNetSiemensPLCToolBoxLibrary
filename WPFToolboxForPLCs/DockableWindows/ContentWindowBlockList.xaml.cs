using System.Windows.Data;
using AvalonDock;
using LibNoDaveConnectionLibrary.DataTypes.Blocks;
using LibNoDaveConnectionLibrary.DataTypes.Blocks.Step5;
using LibNoDaveConnectionLibrary.DataTypes.Projects;
using LibNoDaveConnectionLibrary.General;

namespace WPFToolboxForPLCs.DockableWindows
{
    public partial class ContentWindowBlockList : DocumentContent
    {
        public DockingManager parentDockingManager { get; set; }

        private IBlocksFolder myFld;

        public ContentWindowBlockList(IBlocksFolder fld)
        {
            InitializeComponent();

            myFld = fld;

            this.DataContext = this;

            var tmp = fld.readPlcBlocksList(true);
            tmp.Sort(new NumericComparer<ProjectBlockInfo>());

            var groupedItems = new ListCollectionView(tmp);
            groupedItems.GroupDescriptions.Add(new PropertyGroupDescription("BlockTypeString"));

            this.myDataGrid.ItemsSource = groupedItems;

            
        }

        private void myDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(myDataGrid.SelectedItem!=null)
            {
                Block blk = ((ProjectBlockInfo) myDataGrid.SelectedItem).GetBlock();

                if (blk is PLCFunctionBlock || blk is S5FunctionBlock)
                {
                    e.Handled = true;
                    ContentWindowFunctionBlockEditor tmp = new ContentWindowFunctionBlockEditor(blk);
                    tmp.Title = blk.BlockName;
                    tmp.ToolTip = myFld.ToString() + "\\" + tmp.Title;
                    tmp.Show(parentDockingManager);
                    parentDockingManager.ActiveDocument = tmp;
                }
                else if (blk is PLCDataBlock || blk is S5DataBlock)
                {
                    e.Handled = true;
                    ContentWindowDataBlockEditor tmp = new ContentWindowDataBlockEditor(blk);
                    tmp.Title = blk.BlockName;
                    tmp.ToolTip = myFld.ToString() + "\\" + tmp.Title;
                    tmp.Show(parentDockingManager);
                    parentDockingManager.ActiveDocument = tmp;
                }
            }
        }

        
    }
}
