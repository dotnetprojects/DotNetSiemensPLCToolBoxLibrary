using AvalonDock;

namespace WPFToolboxForPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class ContentWindowDiffWindow : DocumentContent
    {
        public ContentWindowDiffWindow()
        {
            InitializeComponent();

            this.DataContext = this;
        }
    }
}
