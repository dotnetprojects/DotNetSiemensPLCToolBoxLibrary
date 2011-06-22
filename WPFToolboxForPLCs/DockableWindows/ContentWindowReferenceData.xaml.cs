using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace WPFToolboxForSiemensPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class ContentWindowReferenceData : DocumentContent
    {
        public ContentWindowReferenceData(ReferenceData refData)
        {
            InitializeComponent();

            myDataGrid.ItemsSource = refData.ReferenceDataEntrys;
        }

    }
}
