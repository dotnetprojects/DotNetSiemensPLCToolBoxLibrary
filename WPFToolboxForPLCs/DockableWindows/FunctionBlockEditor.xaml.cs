using AvalonDock;

namespace WPFToolboxForPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class FunctionBlockEditor : DockableContent
    {
        public FunctionBlockEditor()
        {
            InitializeComponent();

            this.DataContext = this;
        }

    }
}
