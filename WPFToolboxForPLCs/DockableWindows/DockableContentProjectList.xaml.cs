using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using JFKCommonLibrary.ExtensionMethods;
using JFKCommonLibrary.WPF;

namespace WPFToolboxForSiemensPLCs.DockableWindows
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
                    if (myTreeView.SelectedItem is BlocksOfflineFolder)
                        tmp.ToolTip = ((BlocksOfflineFolder) myTreeView.SelectedItem).Folder;
                    else
                        tmp.ToolTip = fld.ToString();                    
                    tmp.Show(parentDockingManager);
                    tmp.ToggleAutoHide();

                    //Set size of the parent DockablePane (it's automaticly been created!)
                    DockablePane tmpPane = tmp.TryFindParent<DockablePane>();                    
                    ResizingPanel.SetEffectiveSize(tmpPane,new Size(350,0));

                    parentDockingManager.ActiveDocument = tmp;

                }
                else if (myTreeView.SelectedItem is ISymbolTable)
                {
                    ISymbolTable fld = (ISymbolTable)myTreeView.SelectedItem;
                    ContentWindowSymbolTable tmp = new ContentWindowSymbolTable(fld);
                    tmp.Title = fld.ToString(); //.Substring(fld.ToString().LastIndexOf("\\") + 1);
                    tmp.ToolTip = fld.ToString();                   
                    tmp.Show(parentDockingManager);
                    parentDockingManager.ActiveDocument = tmp;
                }
                else if (myTreeView.SelectedItem is ReferenceData)
                {
                    ReferenceData fld = (ReferenceData)myTreeView.SelectedItem;
                    ContentWindowReferenceData tmp = new ContentWindowReferenceData(fld);
                    tmp.parentDockingManager = parentDockingManager;
                  
                    tmp.Title = fld.ToString(); //.Substring(fld.ToString().LastIndexOf("\\") + 1);
                    tmp.ToolTip = fld.ToString();
                    tmp.Show(parentDockingManager);
                    parentDockingManager.ActiveDocument = tmp;
                }
                else if (myTreeView.SelectedItem is S7VATBlock)
                {
                    S7VATBlock fld = (S7VATBlock)myTreeView.SelectedItem;
                    ContentWindowVarTab tmp = new ContentWindowVarTab(fld);
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

            if (blkFld != null)
            {
                string connName = (string) e.Data.GetData("ConnectionName");
                OnlineBlocksFolder oldFld = null;
                foreach (var projectFolder in blkFld.SubItems)
                {
                    if (projectFolder is OnlineBlocksFolder)
                        oldFld = (OnlineBlocksFolder) projectFolder;
                }
                if (oldFld != null)
                    blkFld.SubItems.Remove(oldFld);

                var onlBlkFld = new OnlineBlocksFolder(connName) {Parent = blkFld};
                blkFld.SubItems.Add(onlBlkFld);
                ((IProgrammFolder) blkFld).OnlineBlocksFolder = onlBlkFld;

                row.Items.Refresh();
            }

        }       
    }
}
