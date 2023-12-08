using System;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_1xyy
{
    public static class VariableLengthQuantityHelper
    {
        public static int DecodeInt(byte[] bytes, int offset)
        {
            int n;
            uint val = 0;
            byte b;

            for (n = 1; n <= 4 + 1; n++)
            {        /* große Werte benötigen 5 Bytes */
                b = bytes[offset];
                offset += 1;

                if ((n == 1) && ((b & 0x40) > 0))
                {   /* Vorzeichen prüfen */
                    b &= unchecked((byte)~0x40);
                    val = 0xffffffff;
                    val <<= 6;
                }
                else
                {
                    val <<= 7;
                }
                if ((b & 0x80) > 0)
                {                 /* es folgt noch ein Byte */
                    b &= unchecked((byte)~0x80);
                    val |= b;
                }
                else
                {                        /* alle Bytes gelesen */
                    val |= b;
                    break;
                }
            }

            return (int)val;
        }

        public static byte[] EncodeInt(int value)
        {
            var retVal = new List<byte>();

            var wr = value;
            if (value < 0)
                wr *= -1;

            var anzBytes = 1;

            var anzBits = (int)Math.Ceiling(Math.Log(wr) / Math.Log(2));
            if (anzBits >= 6)
            {
                anzBytes++;
                anzBits -= 6;
            }
            while (anzBits > 7)
            {
                anzBytes++;
                anzBits -= 7;
            }

            for (int i = 1; i <= anzBytes; i++)
            {
                byte wert = 0;

                var fakt = (anzBytes - i) * 7;
                var divisor = (int)Math.Pow(2, fakt);
                wert = (byte)(wr / divisor);
                wr = wr % divisor;

                if (value < 0)
                {
                    if (i == anzBytes || wr == 0)
                        wert -= 1;

                    wert = (byte)~(wert);
                    if (i == anzBytes)
                        wert &= 0x7F;

                    if (i == 1)
                        wert |= 0x40;
                }

                if (i != anzBytes)
                    wert |= 0x80;

                retVal.Add(wert);
            }

            return retVal.ToArray();
        }
    }
}