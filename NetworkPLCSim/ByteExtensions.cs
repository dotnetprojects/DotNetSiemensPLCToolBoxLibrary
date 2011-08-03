using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkPLCSim
{
    public static class ByteExtensions
    {
        public static String ToHexString(byte[] array)
        {
            string retVal = "";
            foreach (byte c in array)
            {
                if (retVal != "")
                    retVal += ", ";
                retVal += "0x" + c.ToString("X");
            }
            return retVal;
        }
    }
}
