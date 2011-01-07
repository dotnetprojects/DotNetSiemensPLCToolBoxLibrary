using AvalonDock;

namespace WPFToolboxForPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class SymbolTable : DockableContent
    {
        public SymbolTable()
        {
            InitializeComponent();

            this.DataContext = this;
        }

    }
}
