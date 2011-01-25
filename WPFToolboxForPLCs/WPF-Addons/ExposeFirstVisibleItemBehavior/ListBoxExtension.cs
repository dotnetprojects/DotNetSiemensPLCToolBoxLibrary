using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFToolboxForSiemensPLCs.WPF_Addons.ExposeFirstVisibleItemBehavior
{
    /// <summary>
    /// Extensions to set and get a ListBox's first visible item.
    /// 
    /// Author: Dominik Schmidt (www.dominikschmidt.net)
    /// </summary>
    public static class ListBoxExtension
    {
        /// <summary>
        /// Sets the first visible item (i.e., scrolls to make
        /// given item the first visible).
        /// </summary>
        /// <param name="listBox">The ListBox</param>
        /// <param name="item">The item to be the first visible</param>
        public static void SetFirstVisibleItem(this ListBox listBox, object item)
        {
            PerformScroll(listBox, item);
        }

        /// <summary>
        /// Gets the first visible item.
        /// </summary>
        /// <param name="listBox">The ListBox</param>
        /// <returns>The first visible item or null of there are no items</returns>
        public static object GetFirstVisibleItem(this ListBox listBox)
        {
            return listBox.Items.Count > 0 ?
                listBox.Items[listBox.GetPanelOffset()] : null;
        }

        /// <summary>
        /// Gets horizontal or vertical offset (depending on panel orientation).
        /// </summary>
        /// <param name="listBox">The ListBox</param>
        /// <returns>The offset or 0 if no VirtualizingStackPanel was found</returns>
        private static int GetPanelOffset(this ListBox listBox)
        {
            VirtualizingStackPanel panel = listBox.GetPanel();
            if (panel != null)
                return (int)((panel.Orientation == Orientation.Horizontal) ? panel.HorizontalOffset : panel.VerticalOffset);
            else
                return 0;
        }

        /// <summary>
        /// Sets horizontal or vertical offset depending on panel orientation.
        /// </summary>
        /// <param name="listBox">The ListBox</param>
        /// <param name="offset">The offset</param>
        private static void SetPanelOffset(this ListBox listBox, int offset)
        {
            VirtualizingStackPanel panel = listBox.GetPanel();
            if (panel != null)
            {
                if (panel.Orientation == Orientation.Horizontal)
                    panel.SetHorizontalOffset(offset);
                else
                    panel.SetVerticalOffset(offset);
            }
        }

        /// <summary>
        /// Retrieves the ListBox's items panel as VirtualizingStackPanel.
        /// </summary>
        /// <param name="listBox">The ListBox</param>
        /// <returns>The item panel or null if no VirtualizingStackPanel was found</returns>
        public static VirtualizingStackPanel GetPanel(this ListBox listBox)
        {
            VirtualizingStackPanel panel = UIHelpers.TryFindChild<VirtualizingStackPanel>(listBox);
            if (panel == null)
                Debug.WriteLine("No VirtualizingStackPanel found for ListBox.");
            return panel;
        }

        /// <summary>
        /// Brings requested item into view as first visible item.
        /// </summary>
        /// <param name="listBox">The ListBox</param>
        /// <param name="item">The requested item</param>
        private static void PerformScroll(ListBox listBox, object item)
        {
            // get container for requested item
            DependencyObject container = listBox.ItemContainerGenerator.ContainerFromItem(item);

            // container does not exist yet (for example because it is not yet in view)
            if (container == null)
            {
                // scroll item into view...
                listBox.ScrollIntoView(item);
                // ...and get its container
                container = listBox.ItemContainerGenerator.ContainerFromItem(item);
            }

            // double-check that container was retrieved
            if (container != null)
            {
                // make sure that item is not only in view,
                // but that it is indeed the first item visible
                int index = listBox.ItemContainerGenerator.IndexFromContainer(container);

                // if panel offset is not equal to index, the
                // item is in view, but not as first item
                if (listBox.GetPanelOffset() != index)
                    listBox.SetPanelOffset(index);
            }
        }
    }
}
