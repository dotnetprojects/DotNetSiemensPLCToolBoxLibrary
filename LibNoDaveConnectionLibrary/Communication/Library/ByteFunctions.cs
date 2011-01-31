using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library
{
    public static class ByteFunctions
    {
        public static byte getU8from(byte[] b, int pos)
        {
            return Convert.ToByte(b[pos]);
        }

        public static sbyte getS8from(byte[] b, int pos)
        {

            if (b[pos] > 127)
                return Convert.ToSByte((256 - b[pos]) * -1);
            else
                return Convert.ToSByte(b[pos]);
        }

        public static short getS16from(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[2];
                b1[1] = b[pos + 0];
                b1[0] = b[pos + 1];
                return BitConverter.ToInt16(b1, 0);
            }
            else
                return BitConverter.ToInt16(b, pos);
        }

        public static void putS16at(byte[] b, int pos, short value)
        {
            byte[] bytes = BitConverter.GetBytes((value));
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 1] = bytes[0];
                b[pos] = bytes[1];
            }
            else
                Array.Copy(bytes, 0, b, pos, 2);
        }

        public static ushort getU16from(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[2];
                b1[1] = b[pos + 0];
                b1[0] = b[pos + 1];
                return BitConverter.ToUInt16(b1, 0);
            }
            else
                return BitConverter.ToUInt16(b, pos);
        }

        public static void putU16at(byte[] b, int pos, ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 1] = bytes[0];
                b[pos] = bytes[1];
            }
            else
                Array.Copy(bytes, 0, b, pos, 2);
        }

        public static int getS32from(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[4];
                b1[3] = b[pos];
                b1[2] = b[pos + 1];
                b1[1] = b[pos + 2];
                b1[0] = b[pos + 3];
                return BitConverter.ToInt32(b1, 0);
            }
            else
                return BitConverter.ToInt32(b, pos);
        }

        public static void putS32at(byte[] b, int pos, int value)
        {
            byte[] bytes = BitConverter.GetBytes((value));
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 3] = bytes[0];
                b[pos + 2] = bytes[1];
                b[pos + 1] = bytes[2];
                b[pos] = bytes[3];
            }
            else
                Array.Copy(bytes, 0, b, pos, 4);
        }

        public static uint getU32from(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[4];
                b1[3] = b[pos];
                b1[2] = b[pos + 1];
                b1[1] = b[pos + 2];
                b1[0] = b[pos + 3];
                return BitConverter.ToUInt32(b1, 0);
            }
            else
                return BitConverter.ToUInt32(b, pos);
        }

        public static void putU32at(byte[] b, int pos, uint value)
        {
            byte[] bytes = BitConverter.GetBytes((value));
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 3] = bytes[0];
                b[pos + 2] = bytes[1];
                b[pos + 1] = bytes[2];
                b[pos] = bytes[3];
            }
            else
                Array.Copy(bytes, 0, b, pos, 4);
        }

        public static void putDateTimeat(byte[] b, int pos, DateTime mydatetime)
        {
            int tmp;

            tmp = mydatetime.Year / 100;
            tmp = tmp * 100;
            tmp = mydatetime.Year - tmp;
            b[pos] = Convert.ToByte(tmp << 4 | tmp / 10);

            tmp = mydatetime.Month;
            b[pos + 1] = Convert.ToByte(tmp << 4 | tmp / 10);

            tmp = mydatetime.Day;
            b[pos + 2] = Convert.ToByte(tmp << 4 | tmp / 10);

            tmp = mydatetime.Hour;
            b[pos + 3] = Convert.ToByte(tmp << 4 | tmp / 10);

            tmp = mydatetime.Minute;
            b[pos + 4] = Convert.ToByte(tmp << 4 | tmp / 10);

            tmp = mydatetime.Second;
            b[pos + 5] = Convert.ToByte(tmp << 4 | tmp / 10);

            tmp = mydatetime.Millisecond;
            b[pos + 6] = Convert.ToByte(tmp << 4 | tmp / 10);

            tmp = (int)mydatetime.DayOfWeek;
            b[pos + 7] = Convert.ToByte(tmp << 4 | tmp / 10);
        }

        public static void putS5Timeat(byte[] b, int pos, TimeSpan value)
        {
            byte basis;
            int wert;
            if (value.TotalMilliseconds <= 999 * 10)
            {
                basis = 0;
                wert = Convert.ToInt32(value.TotalMilliseconds) / 10;
            }
            else if (value.TotalMilliseconds <= 999 * 100)
            {
                basis = 1;
                wert = Convert.ToInt32(value.TotalMilliseconds) / 100;
            }
            else if (value.TotalMilliseconds <= 999 * 1000)
            {
                basis = 2;
                wert = Convert.ToInt32(value.TotalMilliseconds) / 1000;
            }
            else if (value.TotalMilliseconds <= 999 * 10000)
            {
                basis = 3;
                wert = Convert.ToInt32(value.TotalMilliseconds) / 10000;
            }
            else
            {
                basis = 3;
                wert = 999;
            }

            int p1, p2, p3;

            p3 = (wert / 100);
            p2 = ((wert - p3 * 100) / 10);
            p1 = (wert - p3 * 100 - p2 * 10);

            b[pos] = Convert.ToByte(basis << 4 | p3);
            b[pos + 1] = Convert.ToByte((p2 << 4 | p1));
        }

        public static void putTimeat(byte[] b, int pos, TimeSpan value)
        {
            putU32at(b, pos, Convert.ToUInt32(value.TotalMilliseconds));
        }

        public static void putTimeOfDayat(byte[] b, int pos, DateTime value)
        {
            var tmp = new TimeSpan(0, value.Hour, value.Minute, value.Second, value.Millisecond);
            putU32at(b, pos, Convert.ToUInt32(tmp.Milliseconds));
        }

        public static void putDateat(byte[] b, int pos, DateTime value)
        {
            DateTime tmp = new DateTime(1990, 1, 1);
            var tmp2 = value.Subtract(tmp);
            putU16at(b, pos, Convert.ToUInt16(tmp2.Days));
        }

        public static DateTime getDatefrom(byte[] b, int pos)
        {
            DateTime tmp = new DateTime(1990, 1, 1);
            var tmp2 = TimeSpan.FromDays(getU16from(b, pos));
            tmp.Add(tmp2);
            return tmp;
        }

        public static float getFloatfrom(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[4];
                b1[3] = b[pos];
                b1[2] = b[pos + 1];
                b1[1] = b[pos + 2];
                b1[0] = b[pos + 3];
                return BitConverter.ToSingle(b1, 0);
            }
            else
                return BitConverter.ToSingle(b, pos);
        }

        /// <summary>
        /// This put's a String as a S7 String to the PLC
        /// </summary>
        /// <param name="b"></param>
        /// <param name="pos"></param>
        /// <param name="value"></param>
        /// <param name="length"></param>
        public static void putS7Stringat(byte[] b, int pos, string value, int length)
        {
            b[pos] = (byte)length;
            b[pos + 1] = value.Length > length ? (byte)value.Length : (byte)length;
            Array.Copy(Encoding.ASCII.GetBytes(value), 0, b, pos + 2, value.Length > length ? length : value.Length);
        }

        /// <summary>
        /// This put's a String as a Char-Array to the PLC
        /// </summary>
        /// <param name="b"></param>
        /// <param name="pos"></param>
        /// <param name="value"></param>
        /// <param name="length"></param>
        public static void putStringat(byte[] b, int pos, string value, int length)
        {
            Array.Copy(Encoding.ASCII.GetBytes(value), 0, b, pos, value.Length > length ? length : value.Length);
        }

        public static void putFloatat(byte[] b, int pos, int value)
        {
            byte[] bytes = BitConverter.GetBytes(Convert.ToSingle(value));
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 3] = bytes[0];
                b[pos + 2] = bytes[1];
                b[pos + 1] = bytes[2];
                b[pos] = bytes[4];
            }
            else
                Array.Copy(bytes, 0, b, pos, 4);
        }

        public static int getBCD8from(byte[] b, int pos)
        {
            int bt1 = b[pos];
            bt1 = (((bt1 >> 4)) * 10) + ((bt1 & 0x0f));
            return bt1;
        }

        public static void putBCD8at(byte[] b, int pos, int value)
        {
            int b0 = 0, b1 = 0;
            string chars = Convert.ToString(value);
            if (chars.Length > 1)
            {
                b0 = Convert.ToInt32(chars[1].ToString());
                b1 = Convert.ToInt32(chars[0].ToString());
            }
            else if (chars.Length > 0)
                b0 = Convert.ToInt32(chars[0].ToString());

            b[pos] = (byte)(b0 + b1 * 16);
        }

        public static void putBCD16at(byte[] b, int pos, int value)
        {
            int b0 = 0, b1 = 0, b2 = 0, b3 = 0;
            string chars = Convert.ToString(value);
            if (chars.Length > 3)
                b3 = Convert.ToInt32(chars[0].ToString());
            if (chars.Length > 2)
                b2 = Convert.ToInt32(chars[1].ToString());
            if (chars.Length > 1)
                b1 = Convert.ToInt32(chars[3].ToString());
            if (chars.Length > 0)
                b0 = Convert.ToInt32(chars[4].ToString());

            b[pos] = (byte)(b2 + b3 * 16);
            b[pos + 1] = (byte)(b0 + b1 * 16);
        }

        public static int getBCD16from(byte[] b, int pos)
        {
            int bt1 = b[pos];
            int bt2 = b[pos + 1];
            bt1 = (((bt1 >> 4)) * 10) + ((bt1 & 0x0f));
            bt2 = (((bt1 >> 4)) * 10) + ((bt1 & 0x0f));
            return bt2 * 100 + bt1;
        }

        public static DateTime getDateTimefrom(byte[] b, int pos)
        {
            int jahr, monat, tag, stunde, minute, sekunde, mili;
            int bt = b[pos];
            //BCD Umwandlung
            bt = (((bt >> 4)) * 10) + ((bt & 0x0f));
            if (bt < 90)
                jahr = 2000;
            else
                jahr = 1900;
            jahr += bt;

            //Monat
            bt = b[pos + 1];
            monat = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Tag
            bt = b[pos + 2];
            tag = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Stunde
            bt = b[pos + 3];
            stunde = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Minute
            bt = b[pos + 4];
            minute = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Sekunde
            bt = b[pos + 5];
            sekunde = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Milisekunden (werden noch nicht verarbeitet...)
            bt = b[pos + 6];
            mili = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Wochentag
            bt = b[pos + 7];
            try
            {
                return new DateTime(jahr, monat, tag, stunde, minute, sekunde, mili);
            }
            catch (Exception ex)
            {
                return new DateTime(1900, 01, 01, 00, 00, 00);
            }
        }

        public static DateTime getTimeOfDayfrom(byte[] b, int pos)
        {
            long msval = getU32from(b, pos);
            return new DateTime(msval * 10000);
        }

        public static TimeSpan getTimefrom(byte[] b, int pos)
        {
            long msval = getS32from(b, pos);
            return new TimeSpan(msval * 10000);
        }

        public static TimeSpan getS5Timefrom(byte[] b, int pos)
        {
            int w1 = getBCD8from(b, pos + 1);
            int w2 = ((b[pos] & 0x0f));

            long zahl = w2 * 100 + w1;

            int basis = (b[pos] >> 4) & 0x03;

            switch (basis)
            {
                case 0:
                    zahl = zahl * 100000;
                    break;
                case 1:
                    zahl = zahl * 1000000;
                    break;
                case 2:
                    zahl = zahl * 10000000;
                    break;
                case 3:
                    zahl = zahl * 100000000;
                    break;

            }
            return new TimeSpan(zahl);
        }

        public static bool getBit(int Byte, int Bit)
        {
            int wrt = System.Convert.ToInt32(System.Math.Pow(2, Bit));
            return ((Byte & wrt) > 0);
        }

        public static string dec2bin(byte Bytewert)
        {
            byte[] bitwert = { 128, 64, 32, 16, 8, 4, 2, 1 };
            byte[] bits = new byte[8];

            string bitstring = string.Empty; for (int Counter = 0; Counter < 8; Counter++)
            {
                if (Bytewert >= bitwert[Counter])
                {
                    bits[Counter] = 1; Bytewert -= bitwert[Counter];
                }
                bitstring += Convert.ToString(bits[Counter]);
            }
            return bitstring;
        }
    }
}
