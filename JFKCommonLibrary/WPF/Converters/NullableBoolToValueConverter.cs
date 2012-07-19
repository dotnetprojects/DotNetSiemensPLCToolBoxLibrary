using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace JFKCommonLibrary.WPF.Converters
{
    public class NullableBoolToValueConverter<T> : IValueConverter
    {
        public T NullValue { get; set; }
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return NullValue;
            else
                return ((bool?)value).Value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (value.Equals(TrueValue))
                    return true;
                else if (value.Equals(FalseValue))
                    return false;
            }
            return null;
        }
    }

    public class NullableBoolToStringConverter : NullableBoolToValueConverter<String> { }
    public class NullableBoolToBoolConverter : NullableBoolToValueConverter<bool> { }
    public class NullableBoolToBrushConverter : NullableBoolToValueConverter<Brush> { }
    public class NullableBoolToVisibilityConverter : NullableBoolToValueConverter<Visibility> { }
    public class NullableBoolToObjectConverter : NullableBoolToValueConverter<Object> { }
    public class NullableBoolToThicknessConverter : NullableBoolToValueConverter<Thickness> { }
}