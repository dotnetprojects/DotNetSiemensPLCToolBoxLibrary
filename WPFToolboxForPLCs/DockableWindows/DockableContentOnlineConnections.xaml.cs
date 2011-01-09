using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using AvalonDock;
using LibNoDaveConnectionLibrary;
using LibNoDaveConnectionLibrary.DataTypes.Projects;
using LibNoDaveConnectionLibrary.DataTypes.Step7Project;
using LibNoDaveConnectionLibrary.Projectfiles;

namespace WPFToolboxForPLCs.DockableWindows
{
    /// <summary>
    /// Interaction logic for SampleDockableContent.xaml
    /// </summary>
    public partial class DockableContentOnlineConnections : DockableContent
    {
        public ObservableCollection<string> Connections
        {
            get { return (ObservableCollection<string>)GetValue(ConnectionsProperty); }
            set { SetValue(ConnectionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Projects.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectionsProperty =
            DependencyProperty.Register("Connections", typeof(ObservableCollection<string>), typeof(DockableContentOnlineConnections), new UIPropertyMetadata(null));

        public DockingManager parentDockingManager { get; set; }

        public DockableContentOnlineConnections()
        {
            this.Connections = new ObservableCollection<string>((IEnumerable<string>)LibNoDaveConnectionConfiguration.GetConfigurationNames());
         
            InitializeComponent();
            this.DataContext = this;
        }

        private void myConnectionsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        /*
        private void TreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (myTreeView.SelectedItem!=null)
            {
                if (myTreeView.SelectedItem is IBlocksFolder)
                {
                    IBlocksFolder fld = (IBlocksFolder) myTreeView.SelectedItem;
                    BlockList tmp = new BlockList(fld);
                    tmp.parentDockingManager = parentDockingManager;
                    tmp.Title = fld.ToString();
                    tmp.Show(parentDockingManager);

                }
                else if (myTreeView.SelectedItem is SymbolTable)
                {
                    SymbolTable fld = (SymbolTable)myTreeView.SelectedItem;
                    ContentWindowSymbolTable tmp = new ContentWindowSymbolTable(fld);
                    tmp.Title = fld.ToString();
                    tmp.Show(parentDockingManager);

                }
            }
        }
        */      
    }
}
