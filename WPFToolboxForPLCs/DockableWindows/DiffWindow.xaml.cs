using AvalonDock;

namespace WPFToolboxForPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class DiffWindow : DocumentContent
    {
        public DiffWindow()
        {
            InitializeComponent();

            this.DataContext = this;
        }
    }
}
