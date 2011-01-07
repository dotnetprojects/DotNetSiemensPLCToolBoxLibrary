using AvalonDock;

namespace WPFToolboxForPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class BlockList : DockableContent
    {
        public BlockList()
        {
            InitializeComponent();
            this.DataContext = this;
        }       
    }
}
