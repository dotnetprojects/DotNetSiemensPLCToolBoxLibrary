using System;
using System.Windows.Data;
using DotNetSiemensPLCToolBoxLibrary.Communication;

namespace JFKCommonLibrary.WPF.Converters
{
    public class StringToPLCValueBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            else
                return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            else
                return new PLCTag(value.ToString()){LibNoDaveDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Bool};
           
        }
    }
}
