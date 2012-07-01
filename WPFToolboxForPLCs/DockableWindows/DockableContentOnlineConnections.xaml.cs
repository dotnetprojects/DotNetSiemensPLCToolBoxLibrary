using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using JFKCommonLibrary.WPF;

namespace WPFToolboxForSiemensPLCs.DockableWindows
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
            this.Connections = new ObservableCollection<string>((IEnumerable<string>)PLCConnectionConfiguration.GetConfigurationNames());
         
            InitializeComponent();
            this.DataContext = this;
        }

        private void myConnectionsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {            
            if (myConnectionsList.SelectedItem != null)
            {
                OnlineBlocksFolder onl = new OnlineBlocksFolder((string) myConnectionsList.SelectedItem);
                IBlocksFolder fld = (IBlocksFolder) onl;
                DockableContentBlockList tmp = new DockableContentBlockList(fld);
                tmp.parentDockingManager = parentDockingManager;
                tmp.Title = fld.ToString(); //.Substring(fld.ToString().LastIndexOf("\\") + 1);
                tmp.ToolTip = fld.ToString();
                tmp.Show(parentDockingManager);
                tmp.ToggleAutoHide();

                //Set size of the parent DockablePane (it's automaticly been created!)
                DockablePane tmpPane = tmp.TryFindParent<DockablePane>();
                ResizingPanel.SetEffectiveSize(tmpPane, new Size(350, 0));

                parentDockingManager.ActiveDocument = tmp;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Configuration.ShowConfiguration();
            this.Connections = new ObservableCollection<string>((IEnumerable<string>)PLCConnectionConfiguration.GetConfigurationNames());         
        }

        private Point _startPoint;
        private bool IsDragging;
        private Cursor dragDropCursor;
        private void myConnectionsList_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);          
        }

        private void myConnectionsList_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    //Start DragDrop Action!
                    {
                        var row = UIHelpers.TryFindFromPoint<ListBoxItem>((UIElement)sender, e.GetPosition(myConnectionsList));
                        if (row != null)
                        {
                            DataObject dragData = new DataObject("ConnectionName", myConnectionsList.SelectedItem);                            
                            Mouse.OverrideCursor = dragDropCursor = new GhostCursor(row).Cursor;

                            DragDrop.DoDragDrop((DependencyObject)sender, dragData, DragDropEffects.Copy);
                            Mouse.OverrideCursor = null;
                        }
                    }
                }
            }  
        }

        private void myConnectionsList_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if ((e.Effects & DragDropEffects.Copy) != DragDropEffects.None)
            {
                Mouse.OverrideCursor = dragDropCursor;
                e.Handled = true;
            }
            else
                Mouse.OverrideCursor = null;
        }
          
    }
}
