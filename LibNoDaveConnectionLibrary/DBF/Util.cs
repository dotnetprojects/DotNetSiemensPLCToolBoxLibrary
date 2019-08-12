using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DBF
{
    public static class Util
    {
        public static double DoubleDate(String s)
        {
            int i;

            if (s.Trim().Length == 0)
                return 1e100;

            int year = int.Parse(s.Substring(0, 4));
            if (year == 0)
                return 1e100;

            int[] days = {0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};

            int month = int.Parse(s.Substring(4, 6));
            int day = int.Parse(s.Substring(6, 8));

            int daydif = 2378497;

            if ((year/4) == 0)
                days[2] = 29;

            if (year > 1799)
            {
                daydif += day - 1;
                for (i = 2; i <= month; i++)
                    daydif += days[i - 1];
                daydif += (year - 1800)*365;
                daydif += ((year - 1800)/4);
                daydif -= ((year - 1800)%100); // leap years don't occur in 00
                // years
                if (year > 1999) // except in 2000
                    daydif++;
            }
            else
            {
                daydif -= (days[month] - day + 1);
                for (i = 11; i >= month; i--)
                    daydif -= days[i + 1];
                daydif -= (1799 - year)*365;
                daydif -= (1799 - year)/4;
            }

            int retInt = daydif;

            return Convert.ToDouble(retInt);
        }

#if NETSTANDARD
        internal static Encoding DefaultEncoding = CodePagesEncodingProvider.Instance.GetEncoding(1252);
#else
        internal static Encoding DefaultEncoding = System.Text.Encoding.GetEncoding("Windows-1252");
#endif
    }

    /// <summary>
    /// Contains functions to convert dBase data types to .NET types and the other way around
    /// </summary>
    public static class dBaseConverter
    {

        /// <summary>
        /// Converts a logical byte ('L') to a boolean value
        /// </summary>
        /// <param name="dBaseByte">the logical byte from dBase</param>
        /// <returns>The boolean value</returns>
        public static bool L_ToBool(byte dBaseByte)
        {
            if (dBaseByte == 'Y' || dBaseByte == 'y' || dBaseByte == 'T' || dBaseByte == 't')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Converts a boolean value to a dBase logical byte
        /// </summary>
        /// <param name="dBaseByte">The boolean value to convert</param>
        /// <returns>A dBase logical byte</returns>
        public static byte L_FromBool(bool BoolToConvert)
        {
            if (BoolToConvert)
            {
                return (byte) 'Y';
            }
            else
            {
                return (byte) 'N';
            }
        }

        /// <summary>
        /// Converts a float byte array('F') to a double value
        /// </summary>
        /// <param name="dBaseByte">The float byte array from dBase</param>
        /// <returns>The double value</returns>
        public static double F_ToDouble(byte[] dBaseFloatBytes)
        {
            string NumberString = Encoding.ASCII.GetString(dBaseFloatBytes);
            double ReturnDouble;
            if (double.TryParse(NumberString, out ReturnDouble))
            {
                return ReturnDouble;
            }
            else
            {
                return ReturnDouble = 0.0F;
            }
        }

        /// <summary>
        /// Converts a double value to a dBase Float byte array
        /// </summary>
        /// <param name="DoubleToConvert">The double value to convert</param>
        /// <returns>A dBase float byte array</returns>
        public static byte[] F_FromDouble(double DoubleToConvert)
        {
            //Convert to String
            string DoubleString = DoubleToConvert.ToString("N9");
            //Replace comma by point
            DoubleString = DoubleString.Replace(',', '.');
            //Cut everything >20 digits
            if (DoubleString.Length > 20)
                DoubleString = DoubleString.PadRight(20);

            return ASCIIEncoding.ASCII.GetBytes(DoubleString);
        }

        /// <summary>
        /// Converts a timestamp byte array('T') to a datetime value
        /// </summary>
        /// <param name="dBaseTimeBytes">The timestamp byte array from dBase(8 bytes long)</param>
        /// <returns>The datetime value</returns>
        public static DateTime T_ToDateTime(byte[] dBaseTimeBytes)
        {
            // Date is the number of days since 01/01/4713 BC (Julian Days)
            // Time is hours * 3600000L + minutes * 60000L + Seconds * 1000L (Milliseconds since midnight)

            long lDate = BitConverter.ToInt32(dBaseTimeBytes, 0);
            long lTime = BitConverter.ToInt32(dBaseTimeBytes, 4);
            return JulianToDateTime(lDate).AddTicks(lTime);
        }

        /// <summary>
        /// Converts a datetime value to a dBase timestamp byte array
        /// </summary>
        /// <param name="DateTimeToConvert">The double value to convert</param>
        /// <returns>A dBase float byte array</returns>
        public static byte[] T_FromDateTime(DateTime DateTimeToConvert)
        {
            return new byte[0];
        }



        /// <summary>
        /// Converts a character byte array('C') to a string value
        /// </summary>
        /// <param name="dBaseCharacterBytes">The character byte array from dBase</param>
        /// <returns>A string value</returns>
        public static string C_ToString(byte[] dBaseCharacterBytes, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Util.DefaultEncoding;

            return encoding.GetString(dBaseCharacterBytes).TrimEnd(new char[] {' '});
        }

        /// <summary>
        /// Converts a string value to a dBase character byte array
        /// </summary>
        /// <param name="StringToConvert">The string value to convert</param>
        /// <returns>A dBase character byte array</returns>
        public static byte[] C_FromString(string StringToConvert)
        {
            return System.Text.Encoding.GetEncoding("Windows-1252").GetBytes(StringToConvert + ' ');
        }

        /// <summary>
        /// Converts a datetime byte array('D') to a DateTime value
        /// </summary>
        /// <param name="dBaseDateTimeBytes">The datetime byte array from dBase</param>
        /// <returns>A datetime value</returns>
        public static DateTime D_ToDateTime(byte[] dBaseDateTimeBytes)
        {
            DateTime ReturnDateTime = DateTime.MinValue;
            string DateTimeString = Encoding.ASCII.GetString(dBaseDateTimeBytes);
            string sYear = DateTimeString.Substring(0, 4);
            int iYear;
            string sMonth = DateTimeString.Substring(4, 2);
            int iMonth;
            string sDay = DateTimeString.Substring(6, 2);
            int iDay;
            if (Int32.TryParse(sYear, out iYear) && Int32.TryParse(sMonth, out iMonth) && Int32.TryParse(sDay, out iDay))
            {
                if (iYear > 1900)
                {
                    ReturnDateTime = new DateTime(iYear, iMonth, iDay);
                }
            }

            return ReturnDateTime;
        }

        /// <summary>
        /// Converts a DateTime value to a dBase datetime byte array
        /// </summary>
        /// <param name="DateTimeToConvert">The DateTime value to convert</param>
        /// <returns>A dBase DateTime byte array</returns>
        public static byte[] D_FromDateTime(DateTime DateTimeToConvert)
        {
            string DateTimeString = DateTimeToConvert.Year.ToString() + DateTimeToConvert.Month.ToString() + DateTimeToConvert.Day.ToString();
            return Encoding.ASCII.GetBytes(DateTimeString);
        }

        /// <summary>
        /// Differentiate between Integer or Decimal numbers comming from dBase
        /// </summary>
        /// <param name="dBaseNumberBytes">the number byte array from dBase</param>
        /// <returns>True if the submittet byte array is a decimal number</returns>
        public static bool N_IsDecimal(byte[] dBaseNumberBytes)
        {
            if (Encoding.ASCII.GetString(dBaseNumberBytes).Contains("."))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Converts a number byte array('N') to a Decimal value
        /// </summary>
        /// <param name="dBaseNumberBytes">The number byte array from dBase</param>
        /// <returns>A Decimal value</returns>
        public static decimal N_ToDecimal(byte[] dBaseNumberBytes)
        {
            decimal ReturnDecimal;
            if (decimal.TryParse(Encoding.ASCII.GetString(dBaseNumberBytes).Trim(), out ReturnDecimal))
            {
                return ReturnDecimal;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Converts a number byte array('N') to a Integer value
        /// </summary>
        /// <param name="dBaseNumberBytes">The number byte array from dBase</param>
        /// <returns>A Integer value</returns>
        public static int N_ToInt(byte[] dBaseNumberBytes)
        {
            int ReturnInt;
            if (int.TryParse(Encoding.ASCII.GetString(dBaseNumberBytes).Trim(), out ReturnInt))
            {
                return ReturnInt;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Converts a Decimal value to a dBase datetime byte array
        /// </summary>
        /// <param name="DecimalToConvert">The decimal value to convert</param>
        /// <returns>A dBase number byte array</returns>
        public static byte[] N_FromNumber(Decimal DecimalToConvert)
        {
            string DecimalString = DecimalToConvert.ToString();
            return Encoding.ASCII.GetBytes(DecimalString);
        }

        /// <summary>
        /// Converts a Integer value to a dBase datetime byte array
        /// </summary>
        /// <param name="IntToConvert">The integer value to convert</param>
        /// <returns>A dBase number byte array</returns>
        public static byte[] N_FromNumber(int IntToConvert)
        {
            string IntString = IntToConvert.ToString();
            return Encoding.ASCII.GetBytes(IntString);
        }

        /// <summary>
        /// Convert a Julian Date to a .NET DateTime structure
        /// Implemented from pseudo code at http://en.wikipedia.org/wiki/Julian_day
        /// </summary>
        /// <param name="lJDN">Julian Date to convert (days since 01/01/4713 BC)</param>
        /// <returns>DateTime</returns>
        private static DateTime JulianToDateTime(long lJDN)
        {
            double p = Convert.ToDouble(lJDN);
            double s1 = p + 68569;
            double n = Math.Floor(4*s1/146097);
            double s2 = s1 - Math.Floor((146097*n + 3)/4);
            double i = Math.Floor(4000*(s2 + 1)/1461001);
            double s3 = s2 - Math.Floor(1461*i/4) + 31;
            double q = Math.Floor(80*s3/2447);
            double d = s3 - Math.Floor(2447*q/80);
            double s4 = Math.Floor(q/11);
            double m = q + 2 - 12*s4;
            double j = 100*(n - 49) + i + s4;
            return new DateTime(Convert.ToInt32(j), Convert.ToInt32(m), Convert.ToInt32(d));
        }
    }
}
