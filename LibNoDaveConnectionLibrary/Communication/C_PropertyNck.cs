using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    class C_PropertyNck
    {
        private byte _SYNTAX_ID;
        private byte _bereich_u_einheit;
        private uint _spalte;
        private uint _zeile;
        private byte _bausteintyp;
        private byte _ZEILENANZAHL;
        private byte _typ;
        private byte _laenge;

        #region NCK
        [CategoryAttribute("PlcNckTag")]
        [TypeConverter(typeof(ByteHexTypeConverter))]
        public byte SYNTAX_ID
        {
            get { return _SYNTAX_ID; }
            set { _SYNTAX_ID = value; }
        }

        [CategoryAttribute("PlcNckTag")]
        [TypeConverter(typeof(ByteHexTypeConverter))]
        public byte bereich_u_einheit
        {
            get { return _bereich_u_einheit; }
            set { _bereich_u_einheit = value; }
        }

        [CategoryAttribute("PlcNckTag")]
        [TypeConverter(typeof(uintHexTypeConverter))]
        public uint spalte
        {
            get { return _spalte; }
            set { _spalte = value; }
        }

        [CategoryAttribute("PlcNckTag")]
        [TypeConverter(typeof(uintHexTypeConverter))]
        public uint zeile
        {
            get { return _zeile; }
            set { _zeile = value; }
        }

        [CategoryAttribute("PlcNckTag")]
        [TypeConverter(typeof(ByteHexTypeConverter))]
        public byte bausteintyp
        {
            get { return _bausteintyp; }
            set { _bausteintyp = value; }
        }

        [CategoryAttribute("PlcNckTag")]
        [TypeConverter(typeof(ByteHexTypeConverter))]
        public byte ZEILENANZAHL
        {
            get { return _ZEILENANZAHL; }
            set { _ZEILENANZAHL = value; }
        }

        [CategoryAttribute("PlcNckTag")]
        [TypeConverter(typeof(ByteHexTypeConverter))]
        public byte typ
        {
            get { return _typ; }
            set { _typ = value; }
        }

        [CategoryAttribute("PlcNckTag")]
        [TypeConverter(typeof(ByteHexTypeConverter))]
        public byte laenge
        {
            get { return _laenge; }
            set { _laenge = value; }
        }
        #endregion
    }

    //public class CustomFormatter : IFormatProvider, ICustomFormatter
    //{
    //    // implementing the GetFormat method of the IFormatProvider interface
    //    public object GetFormat(System.Type type)
    //    {
    //        return this;
    //    }
    //    // implementing the Format method of the ICustomFormatter interface
    //    public string Format(string format, object arg, IFormatProvider formatProvider)
    //    {
    //        return string.Format("{0:X}", Convert.ToInt32(arg));
    //    }
    //}

    public class ByteHexTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string s = (string)value;
                if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    s = s.Substring(2);

                    return Byte.Parse(s, NumberStyles.HexNumber, culture);
                }
                return Byte.Parse(s, NumberStyles.AllowThousands, culture);
                //byte s = (byte)value;
                //return value.ToString("X");
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value.GetType() == typeof(byte))
                return "0x" + ((byte)value).ToString("X2", culture);

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class uintHexTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string s = (string)value;
                if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    s = s.Substring(2);

                    return uint.Parse(s, NumberStyles.HexNumber, culture);
                }
                return uint.Parse(s, NumberStyles.AllowThousands, culture);
                //byte s = (byte)value;
                //return value.ToString("X");
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value.GetType() == typeof(uint))
                return "0x" + ((uint)value).ToString("X4", culture);

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
