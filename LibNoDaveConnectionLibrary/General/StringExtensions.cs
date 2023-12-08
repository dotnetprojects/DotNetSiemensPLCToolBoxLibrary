using System;

namespace DotNetSiemensPLCToolBoxLibrary.General
{
    public static class StringExtensions
    {
        public static byte[] ToByteArray(this string hex)
        {
            if (hex.Length % 2 == 1) throw new Exception("The binary key cannot have an odd number of digits");

            hex = hex.ToUpper();

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < (hex.Length >> 1); ++i)
            {
                var tmp1 = (GetHexVal(hex[i << 1]) << 4);
                var tmp2 = (GetHexVal(hex[(i << 1) + 1]));
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}