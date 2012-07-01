using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Data;

namespace JFKCommonLibrary.WPF.Converters
{
    public class ByteIntArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is byte[])
            {
                string retval = null;
                foreach (var b in (byte[]) value)
                {
                    if (retval == null)
                        retval = "0x" + b.ToString("X").PadLeft(2, '0');
                    else
                        retval += ", " + "0x" + b.ToString("X").PadLeft(2, '0');
                }
                return retval;
            }
            else if (value is UInt16[] || value is UInt32[])
            {
                string retval = null;
                foreach (var b in (IEnumerable) value)
                {
                    if (retval == null)
                        retval = "0x" + ((IFormattable)b).ToString("X", null).PadLeft(Marshal.SizeOf(b)*2, '0');
                    else
                        retval += ", " + "0x" + ((IFormattable)b).ToString("X", null).PadLeft(Marshal.SizeOf(b)*2, '0');
                }
                return retval;
            }
            else if (value is Int16[] || value is Int32[])
            {
                string retval = null;
                foreach (var b in (IEnumerable) value)
                {
                    if (retval == null)
                        retval = b.ToString();
                    else
                        retval += ", " + b.ToString();
                }
                return retval;
            }
            else if (value is byte || value is UInt16 || value is UInt32)
            {
                string retval = "0x" + ((IFormattable) value).ToString("X", null).PadLeft(Marshal.SizeOf(value)*2,'0');
                return retval;
            }
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}