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
                return Convert.ToSByte((256 - b[pos])*-1);
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

            tmp = mydatetime.Year/100;
            tmp = tmp*100;
            tmp = mydatetime.Year - tmp;
            b[pos] = Convert.ToByte((tmp/10) << 4 | tmp%10);

            tmp = mydatetime.Month;
            b[pos + 1] = Convert.ToByte((tmp/10) << 4 | tmp%10);

            tmp = mydatetime.Day;
            b[pos + 2] = Convert.ToByte((tmp/10) << 4 | tmp%10);

            tmp = mydatetime.Hour;
            b[pos + 3] = Convert.ToByte((tmp/10) << 4 | tmp%10);

            tmp = mydatetime.Minute;
            b[pos + 4] = Convert.ToByte((tmp/10) << 4 | tmp%10);

            tmp = mydatetime.Second;
            b[pos + 5] = Convert.ToByte((tmp/10) << 4 | tmp%10);

            tmp = mydatetime.Millisecond;
            b[pos + 6] = Convert.ToByte((tmp/10) << 4 | tmp%10);

            tmp = (int) mydatetime.DayOfWeek;
            b[pos + 7] = Convert.ToByte((tmp/10) << 4 | tmp%10);
        }

        public static void putS5Timeat(byte[] b, int pos, TimeSpan value)
        {
            byte basis;
            int wert;
            if (value.TotalMilliseconds <= 999*10)
            {
                basis = 0;
                wert = Convert.ToInt32(value.TotalMilliseconds)/10;
            }
            else if (value.TotalMilliseconds <= 999*100)
            {
                basis = 1;
                wert = Convert.ToInt32(value.TotalMilliseconds)/100;
            }
            else if (value.TotalMilliseconds <= 999*1000)
            {
                basis = 2;
                wert = Convert.ToInt32(value.TotalMilliseconds)/1000;
            }
            else if (value.TotalMilliseconds <= 999*10000)
            {
                basis = 3;
                wert = Convert.ToInt32(value.TotalMilliseconds)/10000;
            }
            else
            {
                basis = 3;
                wert = 999;
            }

            int p1, p2, p3;

            p3 = (wert/100);
            p2 = ((wert - p3*100)/10);
            p1 = (wert - p3*100 - p2*10);

            b[pos] = Convert.ToByte(basis << 4 | p3);
            b[pos + 1] = Convert.ToByte((p2 << 4 | p1));
        }

        public static void putTimeat(byte[] b, int pos, TimeSpan value)
        {
            putS32at(b, pos, Convert.ToInt32(value.TotalMilliseconds));
        }

        public static void putTimeOfDayat(byte[] b, int pos, DateTime value)
        {
            var tmp = new TimeSpan(0, value.Hour, value.Minute, value.Second, value.Millisecond);
            putU32at(b, pos, Convert.ToUInt32(tmp.TotalMilliseconds));
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
            tmp = tmp.Add(tmp2);
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
            b[pos] = (byte) length;
            b[pos + 1] = length > value.Length ? (byte) value.Length : (byte) length;
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

        public static void putFloatat(byte[] b, int pos, Single value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
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

        public static int getBCD8from(byte[] b, int pos)
        {
            //Acepted Values 00 to 99
            int bt1 = b[pos];
            bool neg = (bt1 & 0xf0) == 0xf0 ? true : false;
            if (neg)
            {
                bt1 = -1*(bt1 & 0x0f);
                //bt1 = 0;
            }
            else
            {
                bt1 = (bt1 >> 4)*10 + (bt1 & 0x0f);
            }
            return bt1;
        }

        public static void putBCD8at(byte[] b, int pos, int value)
        {
            int b0 = 0, b1 = 0;

            //setze höchstes bit == negativer wert!
            if (value >= 0)
            {
                b1 = (value%100/10);
                b0 = value%10;
            }
            b[pos] = (byte) ((b1 << 4) + b0);
        }

        public static void putBCD16at(byte[] b, int pos, int value)
        {
            //Acepted Values -999 to +999
            int b0 = 0, b1 = 0, b2 = 0, b3 = 0;

            if (value < 0)
            {
                b3 = 0x0f;
                value = -1*value;
            }
            else
            {
                b3 = 0x00;
            }
            b2 = (value%1000/100);
            b1 = (value%100/10);
            b0 = (value%10);
            b[pos] = (byte) ((b3 << 4) + b2);
            b[pos + 1] = (byte) ((b1 << 4) + b0);
        }

        public static void putBCD32at(byte[] b, int pos, int value)
        {
            //Acepted Values -9999999 to +9999999
            int b0 = 0, b1 = 0, b2 = 0, b3 = 0, b4 = 0, b5 = 0, b6 = 0, b7 = 0;

            if (value < 0)
            {
                b7 = 0x0f;
                value = -1*value;
            }
            else
            {
                //b7 = (value % 100000000 / 10000000);
                b7 = 0x00;
            }
            b6 = (value%10000000/1000000);
            b5 = (value%1000000/100000);
            b4 = (value%100000/10000);
            b3 = (value%10000/1000);
            b2 = (value%1000/100);
            b1 = (value%100/10);
            b0 = (value%10);

            b[pos] = (byte) ((b7 << 4) + b6);
            b[pos + 1] = (byte) ((b5 << 4) + b4);
            b[pos + 2] = (byte) ((b3 << 4) + b2);
            b[pos + 3] = (byte) ((b1 << 4) + b0);
        }

        public static int getBCD16from(byte[] b, int pos)
        {
            int bt1 = b[pos];
            int bt2 = b[pos + 1];
            bool neg = (bt1 & 0xf0) == 0xf0 ? true : false;

            bt1 = bt1 & 0x0f;
            bt2 = (bt2/0x10)*10 + (bt2 & 0x0f%0x10);

            return neg ? (bt1*100 + bt2)*-1 : bt1*100 + bt2;
        }

        public static int getBCD32from(byte[] b, int pos)
        {
            int bt1 = b[pos];
            int bt2 = b[pos + 1];
            int bt3 = b[pos + 2];
            int bt4 = b[pos + 3];
            bool neg = (bt1 & 0xf0) == 0xf0 ? true : false;

            bt1 = bt1 & 0x0f;
            bt2 = (bt2/0x10)*10 + (bt2%0x10);
            bt3 = (bt3/0x10)*10 + (bt3%0x10);
            bt4 = (bt4/0x10)*10 + (bt4%0x10);
            return neg ? (bt1*1000000 + bt2*10000 + bt3*100 + bt4)*-1 : bt1*1000000 + bt2*10000 + bt3*100 + bt4;
        }

        public static DateTime getDateTimefrom(byte[] b, int pos)
        {
            int jahr, monat, tag, stunde, minute, sekunde, mili;
            int bt = b[pos];
            //BCD Umwandlung
            bt = (((bt >> 4))*10) + ((bt & 0x0f));
            if (bt < 90)
                jahr = 2000;
            else
                jahr = 1900;
            jahr += bt;

            //Monat
            bt = b[pos + 1];
            monat = (((bt >> 4))*10) + ((bt & 0x0f));

            //Tag
            bt = b[pos + 2];
            tag = (((bt >> 4))*10) + ((bt & 0x0f));

            //Stunde
            bt = b[pos + 3];
            stunde = (((bt >> 4))*10) + ((bt & 0x0f));

            //Minute
            bt = b[pos + 4];
            minute = (((bt >> 4))*10) + ((bt & 0x0f));

            //Sekunde
            bt = b[pos + 5];
            sekunde = (((bt >> 4))*10) + ((bt & 0x0f));

            //Milisekunden
            //Byte 6 BCD + MSB (Byte 7)
            bt = b[pos + 6];
            int bt1 = b[pos + 7];
            mili = (((bt >> 4))*10) + ((bt & 0x0f));
            mili = mili*10 + (bt1 >> 4);

            //Wochentag
            //LSB (Byte 7) 1=Sunday
            //bt = b[pos + 7];
            //wochentag = (bt1 & 0x0f); 
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
            return new DateTime(msval*10000);
        }

        public static TimeSpan getTimefrom(byte[] b, int pos)
        {
            long msval = getS32from(b, pos);
            return new TimeSpan(msval*10000);
        }

        public static TimeSpan getS5Timefrom(byte[] b, int pos)
        {
            int w1 = getBCD8from(b, pos + 1);
            int w2 = ((b[pos] & 0x0f));

            long zahl = w2*100 + w1;

            int basis = (b[pos] >> 4) & 0x03;

            switch (basis)
            {
                case 0:
                    zahl = zahl*100000;
                    break;
                case 1:
                    zahl = zahl*1000000;
                    break;
                case 2:
                    zahl = zahl*10000000;
                    break;
                case 3:
                    zahl = zahl*100000000;
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
            byte[] bitwert = {128, 64, 32, 16, 8, 4, 2, 1};
            byte[] bits = new byte[8];

            string bitstring = string.Empty;
            for (int Counter = 0; Counter < 8; Counter++)
            {
                if (Bytewert >= bitwert[Counter])
                {
                    bits[Counter] = 1;
                    Bytewert -= bitwert[Counter];
                }
                bitstring += Convert.ToString(bits[Counter]);
            }
            return bitstring;
        }

    }
}
