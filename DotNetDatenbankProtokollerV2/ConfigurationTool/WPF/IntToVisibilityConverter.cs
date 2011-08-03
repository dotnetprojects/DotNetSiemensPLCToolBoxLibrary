using System;
using System.Windows;
using System.Windows.Data;
using DotNetSiemensPLCToolBoxLibrary.Communication;

namespace DotNetSimaticDatabaseProtokollerConfigurationTool.WPF
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public int VisibleValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (System.Convert.ToInt32(value) != VisibleValue)
                return Visibility.Hidden;
            return Visibility.Visible;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
