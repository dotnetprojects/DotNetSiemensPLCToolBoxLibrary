using System;

namespace Kopplungstester
{
    public static class StringExtension
    {
        /// <summary>
        /// This Function Adds the Filling Char to the String when it is to small,
        /// or it removes Chars from the Right, when it is to Large!        
        /// </summary>
        /// <param name="mystr"></param>
        /// <param name="FixedLength"></param>
        /// <param name="FillChar"></param>
        /// <returns></returns>
        public static String FixSize(this string mystr, int FixedLength, char FillChar)
        {           
            if (FixedLength > mystr.Length)
                return mystr.PadRight(FixedLength, ' ');
            else if (FixedLength < mystr.Length)
                return mystr.Substring(0, FixedLength);
            return mystr;
        }
    }
}
