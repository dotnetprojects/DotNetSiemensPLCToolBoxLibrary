using System.Windows;
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

        private void textEditorA_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }
    }
}
