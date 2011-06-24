using System.Windows.Controls;
using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace WPFToolboxForSiemensPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class ContentWindowReferenceData : DocumentContent
    {
        public DockingManager parentDockingManager { get; set; }

        public ContentWindowReferenceData(ReferenceData refData)
        {
            InitializeComponent();

            myDataGrid.ItemsSource = refData.ReferenceDataEntrys;
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            myDataGrid = (DataGrid) sender;

            if (myDataGrid.SelectedItem != null)
            {
                ReferencePoint referencePoint = (ReferencePoint)myDataGrid.SelectedItem;
                Block blk = referencePoint.Block;

                if (blk is S7FunctionBlock || blk is S5FunctionBlock)
                {
                    e.Handled = true;
                    ContentWindowFunctionBlockEditor tmp = new ContentWindowFunctionBlockEditor(blk, referencePoint.NetworkNumber, referencePoint.LineNumber);
                    tmp.Title = blk.BlockName;
                    //tmp.ToolTip = myFld.ToString() + "\\" + tmp.Title;
                    tmp.Show(parentDockingManager);
                    parentDockingManager.ActiveDocument = tmp;
                }
            }
        }

    }
}
