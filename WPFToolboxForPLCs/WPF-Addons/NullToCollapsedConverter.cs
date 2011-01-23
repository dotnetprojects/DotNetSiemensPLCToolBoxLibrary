using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFToolboxForSiemensPLCs
{
    public class NullToCollapsedConverter : IValueConverter
    {       
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}