using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFVarTab
{
    public static class ComboBoxExtension
    {
        public static bool GetIsNullable(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsNullableProperty);
        }

        public static void SetIsNullable(DependencyObject obj, bool value)
        {
            obj.SetValue(IsNullableProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsNullable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNullableProperty =
            DependencyProperty.RegisterAttached("IsNullable", typeof(bool), typeof(ComboBoxExtension), new UIPropertyMetadata(false, OnIsNullableChanged));

        private static void OnIsNullableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var comboBox = (ComboBox)d;
                comboBox.SizeChanged += comboBox_SizeChanged;
            }
        }

        private static void comboBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            comboBox.SizeChanged -= comboBox_SizeChanged;

            ApplyIsNullable(comboBox);

            //also apply after the selection has changed
            comboBox.SelectionChanged += delegate { ApplyIsNullable(comboBox); };
        }

        private static void ApplyIsNullable(ComboBox comboBox)
        {
            var isNullable = GetIsNullable(comboBox);
            var clearButton = (Button)GetClearButton(comboBox);
            if (clearButton != null)
            {
                clearButton.Click -= clearButton_Click;
                clearButton.Click += clearButton_Click;

                if (isNullable && comboBox.SelectedIndex != -1)
                {
                    clearButton.Visibility = Visibility.Visible;
                }
                else
                {
                    clearButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private static void clearButton_Click(object sender, RoutedEventArgs e)
        {
            var clearButton = (Button)sender;
            var parent = VisualTreeHelper.GetParent(clearButton);

            while (!(parent is ComboBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var comboBox = (ComboBox)parent;
            //clear the selection
            comboBox.SelectedIndex = -1;
        }

        private static Button GetClearButton(DependencyObject reference)
        {
            for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(reference); childIndex++)
            {
                var child = VisualTreeHelper.GetChild(reference, childIndex);

                if (child is Button && ((Button)child).Name == "PART_ClearButton")
                {
                    return (Button)child;
                }

                var clearButton = GetClearButton(child);
                if (clearButton is Button)
                {
                    return clearButton;
                }
            }

            return null;
        }
    }
}
