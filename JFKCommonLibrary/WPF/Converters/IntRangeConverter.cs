using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace JFKCommonLibrary.WPF.Converters
{
    public class IntRangeConverter<T> : IValueConverter
    {
        public class IntRangeValue
        {
            public string IntRange { get; set; }
            public T Value { get; set; }
        }

        private List<IntRangeValue> _intRangeValues = new List<IntRangeValue>();
        public List<IntRangeValue> IntRangeValues
        {
            get { return _intRangeValues; }
            set { _intRangeValues = value; }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            int intvalue = System.Convert.ToInt32(value);

            foreach (IntRangeValue intBrushValue in IntRangeValues)
            {
                if (intBrushValue.IntRange.Contains('-'))
                {
                    int start = System.Convert.ToInt32(intBrushValue.IntRange.Split('-')[0]);
                    int stop = System.Convert.ToInt32(intBrushValue.IntRange.Split('-')[1]);
                    if (intvalue >= start && intvalue <= stop)
                        return intBrushValue.Value;
                }
                else if (intBrushValue.IntRange.Length>=2 && intBrushValue.IntRange[0] == '<' && intBrushValue.IntRange[1] == '=')
                {
                    int val = System.Convert.ToInt32(intBrushValue.IntRange.Substring(1));
                    if (intvalue <= val)
                        return intBrushValue.Value;
                }
                else if (intBrushValue.IntRange.Length >= 2 && intBrushValue.IntRange[0] == '>' && intBrushValue.IntRange[1] == '=')
                {
                    int val = System.Convert.ToInt32(intBrushValue.IntRange.Substring(1));
                    if (intvalue >= val)
                        return intBrushValue.Value;
                }
                else if (intBrushValue.IntRange.Length >= 1 && intBrushValue.IntRange[0] == '<')
                {
                    int val = System.Convert.ToInt32(intBrushValue.IntRange.Substring(1));
                    if (intvalue < val)
                        return intBrushValue.Value;
                }
                else if (intBrushValue.IntRange.Length >= 1 && intBrushValue.IntRange[0] == '>')
                {
                    int val = System.Convert.ToInt32(intBrushValue.IntRange.Substring(1));
                    if (intvalue > val)
                        return intBrushValue.Value;
                }
                else if (System.Convert.ToInt32(intBrushValue.IntRange) == intvalue)
                    return intBrushValue.Value;
            }

            return null;          
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntRangeToStringConverter : IntRangeConverter<string> { } ;
    public class IntRangeToBrushConverter : IntRangeConverter<Brush> { } ;
    public class IntRangeToThicknessConverter : IntRangeConverter<Thickness> { } ;
    public class IntRangeToPointConverter : IntRangeConverter<Point> { } ;
    public class IntRangeToIntConverter : IntRangeConverter<int> { } ;
    public class IntRangeToDoubleConverter : IntRangeConverter<double> { } ;
}
