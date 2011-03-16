using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace ExampleWPFVisualization.Converters
{
    public class IntRangeBrushValue
    {
        public string IntRange { get; set; }
        public Brush Brush { get; set; }
    }


    public class IntRangeToBrushConverter : IValueConverter
    {
        private List<IntRangeBrushValue> _intRangeBrushValues=new List<IntRangeBrushValue>();
        public List<IntRangeBrushValue> IntRangeBrushValues
        {
            get { return _intRangeBrushValues; }
            set { _intRangeBrushValues = value; }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            int intvalue = System.Convert.ToInt32(value);

            foreach (IntRangeBrushValue intBrushValue in IntRangeBrushValues)
            {
                if (intBrushValue.IntRange.Contains('-'))
                {
                    int start = System.Convert.ToInt32(intBrushValue.IntRange.Split('-')[0]);
                    int stop = System.Convert.ToInt32(intBrushValue.IntRange.Split('-')[1]);
                    if (intvalue >= start && intvalue <= stop)
                        return intBrushValue.Brush;
                }
                else if (intBrushValue.IntRange.Length>=2 && intBrushValue.IntRange[0] == '<' && intBrushValue.IntRange[1] == '=')
                {
                    int val = System.Convert.ToInt32(intBrushValue.IntRange.Substring(1));
                    if (intvalue <= val)
                        return intBrushValue.Brush;
                }
                else if (intBrushValue.IntRange.Length >= 2 && intBrushValue.IntRange[0] == '>' && intBrushValue.IntRange[1] == '=')
                {
                    int val = System.Convert.ToInt32(intBrushValue.IntRange.Substring(1));
                    if (intvalue >= val)
                        return intBrushValue.Brush;
                }
                else if (intBrushValue.IntRange.Length >= 1 && intBrushValue.IntRange[0] == '<')
                {
                    int val = System.Convert.ToInt32(intBrushValue.IntRange.Substring(1));
                    if (intvalue < val)
                        return intBrushValue.Brush;
                }
                else if (intBrushValue.IntRange.Length >= 1 && intBrushValue.IntRange[0] == '>')
                {
                    int val = System.Convert.ToInt32(intBrushValue.IntRange.Substring(1));
                    if (intvalue > val)
                        return intBrushValue.Brush;
                }
                else if (System.Convert.ToInt32(intBrushValue.IntRange) == intvalue)
                    return intBrushValue.Brush;
            }

            return null;          
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
