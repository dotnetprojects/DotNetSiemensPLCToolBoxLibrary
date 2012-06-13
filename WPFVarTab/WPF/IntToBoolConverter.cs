using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFToolboxForSiemensPLCs
{
    public class IntToBoolConverter : IValueConverter
    {
        public int Value { get; set; }
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || (int)value != Value)
                return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == true)
                return Value;
            return null;
        }
    }
}