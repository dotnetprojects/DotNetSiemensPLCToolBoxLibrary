using AvalonDock;

namespace WPFToolboxForPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class DataBlockEditor : DockableContent
    {
        public DataBlockEditor()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        
    }
}
