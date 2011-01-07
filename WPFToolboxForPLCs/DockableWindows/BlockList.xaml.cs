using System.Windows.Data;
using AvalonDock;
using LibNoDaveConnectionLibrary.DataTypes.Blocks;
using LibNoDaveConnectionLibrary.DataTypes.Projects;
using LibNoDaveConnectionLibrary.General;

namespace WPFToolboxForPLCs.DockableWindows
{
    public partial class BlockList : DocumentContent
    {
        public DockingManager parentDockingManager { get; set; }

        public BlockList(IBlocksFolder fld)
        {
            InitializeComponent();

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
                
                if (blk is PLCFunctionBlock)
                {
                    FunctionBlockEditor tmp = new FunctionBlockEditor((PLCFunctionBlock)blk);
                    tmp.Title = blk.BlockName;
                    tmp.Show(parentDockingManager);
                    parentDockingManager.ActiveDocument = tmp;
                }
            }
        }

        
    }
}
