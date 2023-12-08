using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using System;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.General
{
    public class CrcHelper
    {
        public static string GetAccesKeyForSymbolTabelEntry(MemoryArea area, string name, uint litId)
        {
            string retVal = "000000";

            if (area == MemoryArea.Counter)
                retVal += "53";
            else if (area == MemoryArea.Timer)
                retVal += "54";
            else
            {
                retVal += (((int)area) - 0x31).ToString("X");
            }

            retVal += CrcHelper.GetCrc32(Encoding.ASCII.GetBytes(name)).ToString("X").PadLeft(8, '0');

            retVal += "4" + litId.ToString("X").PadLeft(7, '0');

            return retVal;
        }

        public static UInt32 GetCrc32(byte[] data)
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

        public static UInt16 GetCrc16(byte[] data)
        {
            UInt16 CRC16MASK = 0x9003; //Start of CRC found via Try and Error...

            UInt16 crc16 = 0;
            int i;
            int j;
            byte x;
            for (i = 0; i < data.Length; ++i)
            {
                x = data[i];
                for (j = 0; j < 8; ++j)
                {
                    if ((crc16 & 0x8000) == 0)
                    {
                        if ((x & 0x80) == 0)
                        {
                            crc16 <<= 1;
                        }
                        else
                        {
                            crc16 = (UInt16)((crc16 << 1) ^ CRC16MASK);
                        }
                    }
                    else
                    {
                        if ((x & 0x80) == 0)
                        {
                            crc16 = (UInt16)((crc16 << 1) ^ CRC16MASK);
                        }
                        else
                        {
                            crc16 <<= 1;
                        }
                    }
                    x <<= 1;
                }
            }
            return crc16;
        }
    }
}