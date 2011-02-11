using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace WPFToolboxForSiemensPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class ContentWindowSourceBlock : DocumentContent
    {
        public ContentWindowSourceBlock(S7SourceBlock sBlk)
        {
            InitializeComponent();

            textEditor.Text = sBlk.Text;
        }

    }
}
