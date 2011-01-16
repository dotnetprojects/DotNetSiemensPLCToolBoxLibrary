using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace WPFToolboxForSiemensPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class ContentWindowVarTab : DocumentContent
    {
        public ContentWindowVarTab(S7VATBlock varTab)
        {
            InitializeComponent();

            myDataGrid.ItemsSource = varTab.VATRows;           
        }

    }
}
