using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace WPFToolboxForSiemensPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class ContentWindowSymbolTable : DocumentContent
    {
        public ContentWindowSymbolTable(ISymbolTable symTab)
        {
            InitializeComponent();

            myDataGrid.ItemsSource = symTab.SymbolTableEntrys;
        }

    }
}
