using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using JFKCommonLibrary.ExtensionMethods;

namespace JFKCommonLibrary.WPF.Behaviors
{
    /// <summary>
    /// Exposes a ListBox's first visible item for (two-way)
    /// binding to a view model.
    /// 
    /// Author: Dominik Schmidt (www.dominikschmidt.net)
    /// </summary>
    public class ExposeFirstVisibleItemBehavior
    {
        #region fields

        // keeping track of list boxes not to add handler multiple times
        private static List<ListBox> _listBoxes = new List<ListBox>();

        #endregion

        #region attached behavior

        public static readonly DependencyProperty FirstVisibleItemProperty =
            DependencyProperty.RegisterAttached("FirstVisibleItem", typeof(object), typeof(ExposeFirstVisibleItemBehavior),
               new FrameworkPropertyMetadata() { PropertyChangedCallback = OnFirstVisibleItemChanged, BindsTwoWayByDefault = true });

        public static object GetFirstVisibleItem(DependencyObject d)
        {
            if (d == null) throw new ArgumentNullException();
            return d.GetValue(FirstVisibleItemProperty);
        }

        public static void SetFirstVisibleItem(DependencyObject d, object value)
        {
            if (d == null) throw new ArgumentNullException();
            d.SetValue(FirstVisibleItemProperty, value);
        }

        private static void OnFirstVisibleItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListBox listBox = d as ListBox;

            if (d == null)
                throw new InvalidOperationException("The FirstVisibleItem attached property can only be applied to ListBox controls.");

            // add scroll changed handler only if not yet added
            if (!_listBoxes.Contains(listBox))
            {
                _listBoxes.Add(listBox);
                listBox.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(ScrollChanged));
            }

            if (e.OldValue != e.NewValue)
                listBox.SetFirstVisibleItem(e.NewValue);
        }

        #endregion

        #region scrolling

        // updates first visible item after scrolling
        private static void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            if (listBox.Items.Count > 0 && listBox.GetPanel() != null)
                SetFirstVisibleItem(listBox, listBox.GetFirstVisibleItem());
        }

        #endregion
    }
}
