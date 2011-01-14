using System.Windows.Data;
using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;

namespace WPFToolboxForSiemensPLCs.DockableWindows
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
