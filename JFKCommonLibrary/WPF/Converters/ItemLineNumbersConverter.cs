using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace JFKCommonLibrary.WPF.Converters
{
    public class ItemLineNumbersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as ListBoxItem;
            var view = (ListBox)ItemsControl.ItemsControlFromItemContainer(item);
            return view.ItemContainerGenerator.IndexFromContainer(item) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
