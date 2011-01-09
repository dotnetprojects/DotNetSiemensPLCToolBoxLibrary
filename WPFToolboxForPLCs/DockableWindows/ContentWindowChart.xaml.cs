using System.Windows.Data;
using AvalonDock;
using LibNoDaveConnectionLibrary.DataTypes.Blocks;
using LibNoDaveConnectionLibrary.DataTypes.Blocks.Step5;
using LibNoDaveConnectionLibrary.DataTypes.Projects;
using LibNoDaveConnectionLibrary.General;

namespace WPFToolboxForPLCs.DockableWindows
{
    public partial class ContentWindowChart : DocumentContent
    {
        public DockingManager parentDockingManager { get; set; }

        private IBlocksFolder myFld;

        public ContentWindowChart(IBlocksFolder fld)
        {
            InitializeComponent();
        }       
    }
}
