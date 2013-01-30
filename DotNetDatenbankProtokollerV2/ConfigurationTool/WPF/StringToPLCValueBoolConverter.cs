using System;
using System.Windows.Data;

using DotNetSiemensPLCToolBoxLibrary.Communication;

namespace DotNetSimaticDatabaseProtokollerConfigurationTool.WPF
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
                return new PLCTag(value.ToString()){TagDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Bool};
           
        }
    }
}
