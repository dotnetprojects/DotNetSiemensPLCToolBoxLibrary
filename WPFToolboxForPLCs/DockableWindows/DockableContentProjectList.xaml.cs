using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AvalonDock;
using LibNoDaveConnectionLibrary.DataTypes.Blocks;
using LibNoDaveConnectionLibrary.DataTypes.Projects;
using LibNoDaveConnectionLibrary.DataTypes.Step7Project;
using LibNoDaveConnectionLibrary.Projectfiles;
using WPFToolboxForPLCs.WPF_Addons;

namespace WPFToolboxForPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class DockableContentProjectList : DockableContent
    {
        public ObservableCollection<ProjectFolder> Projects
        {
            get { return (ObservableCollection<ProjectFolder>)GetValue(ProjectsProperty); }
            set { SetValue(ProjectsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Projects.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProjectsProperty =
            DependencyProperty.Register("Projects", typeof(ObservableCollection<ProjectFolder>), typeof(DockableContentProjectList), new UIPropertyMetadata(null));

        public DockingManager parentDockingManager { get; set; }

        public DockableContentProjectList()
        {
            this.Projects = new ObservableCollection<ProjectFolder>();

            InitializeComponent();
            this.DataContext = this;
        }

        private void TreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (myTreeView.SelectedItem!=null)
            {
                if (myTreeView.SelectedItem is IBlocksFolder)
                {
                    IBlocksFolder fld = (IBlocksFolder) myTreeView.SelectedItem;
                    DockableContentBlockList tmp = new DockableContentBlockList(fld);
                    tmp.parentDockingManager = parentDockingManager;
                    tmp.Title = fld.ToString(); //.Substring(fld.ToString().LastIndexOf("\\") + 1);
                    tmp.ToolTip = fld.ToString();
                    tmp.Show(parentDockingManager);
                    tmp.ToggleAutoHide();
                    parentDockingManager.ActiveDocument = tmp;

                }
                else if (myTreeView.SelectedItem is SymbolTable)
                {
                    SymbolTable fld = (SymbolTable)myTreeView.SelectedItem;
                    ContentWindowSymbolTable tmp = new ContentWindowSymbolTable(fld);
                    tmp.Title = fld.ToString(); //.Substring(fld.ToString().LastIndexOf("\\") + 1);
                    tmp.ToolTip = fld.ToString();
                    tmp.Show(parentDockingManager);
                    parentDockingManager.ActiveDocument = tmp;
                }
            }
        }

        private void myTreeView_Drop(object sender, DragEventArgs e)
        {
            TreeViewItem row = UIHelpers.TryFindFromPoint<TreeViewItem>((UIElement)sender, e.GetPosition(myTreeView));

            ProjectFolder blkFld = (ProjectFolder) myTreeView.ItemFromContainer(row);

            while (blkFld != null && !(blkFld is S7ProgrammFolder))
            {
                blkFld = ((ProjectFolder) blkFld).Parent;
                row = row.TryFindParent<TreeViewItem>();
            }

            if (blkFld!=null)
            {
                string connName = (string)e.Data.GetData("ConnectionName");
                OnlineBlocksFolder oldFld = null;
                foreach (var projectFolder in blkFld.SubItems)
                {
                    if (projectFolder is OnlineBlocksFolder)
                        oldFld = (OnlineBlocksFolder) projectFolder;
                }
                if (oldFld != null)
                    blkFld.SubItems.Remove(oldFld);
                blkFld.SubItems.Add(new OnlineBlocksFolder(connName){Parent = blkFld});                
                row.Items.Refresh();                
            }          

        }       
    }
}
