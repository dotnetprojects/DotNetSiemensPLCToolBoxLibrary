using AvalonDock;
using LibNoDaveConnectionLibrary.DataTypes.Blocks;
using LibNoDaveConnectionLibrary.DataTypes.Step7Project;

namespace WPFToolboxForPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class ContentWindowVarTab : DocumentContent
    {
        public ContentWindowVarTab(VATBlock varTab)
        {
            InitializeComponent();

            myDataGrid.ItemsSource = varTab.VATRows;
        }

    }
}
