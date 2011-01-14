using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace WPFToolboxForSiemensPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class ContentWindowSymbolTable : DocumentContent
    {
        public ContentWindowSymbolTable(SymbolTable symTab)
        {
            InitializeComponent();

            myDataGrid.ItemsSource = symTab.Step7SymbolTableEntrys;
        }

    }
}
