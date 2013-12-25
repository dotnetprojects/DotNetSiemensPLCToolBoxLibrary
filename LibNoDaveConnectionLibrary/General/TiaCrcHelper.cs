using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.General
{
    public class TiaCrcHelper
    {
        public static UInt32 getcrc(byte[] data)
        {
            UInt32 CRC32MASK = 0xF4ACFB13; //Start of CRC found via Try and Error...

            UInt32 crc32 = 0;
            int i;
            int j;
            byte x;
            for (i = 0; i < data.Length; ++i)
            {
                x = data[i];
                for (j = 0; j < 8; ++j)
                {
                    if ((crc32 & 0x80000000) == 0)
                    {
                        if ((x & 0x80) == 0)
                        {
                            crc32 <<= 1;
                        }
                        else
                        {
                            crc32 = (crc32 << 1) ^ CRC32MASK;
                        }
                    }
                    else
                    {
                        if ((x & 0x80) == 0)
                        {
                            crc32 = (crc32 << 1) ^ CRC32MASK;
                        }
                        else
                        {
                            crc32 <<= 1;
                        }
                    }
                    x <<= 1;
                }
            }
            return crc32;
        }
    }
}
