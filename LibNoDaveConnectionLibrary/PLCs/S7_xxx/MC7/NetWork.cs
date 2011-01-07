/*
 This implements a high level Wrapper between libnodave.dll and applications written
 in MS .Net languages.
 
 This ConnectionLibrary was written by Jochen Kuehner
 * http://jfk-solutuions.de/
 * 
 * Thanks go to:
 * Steffen Krayer -> For his work on MC7 decoding and the Source for his Decoder
 * Zottel         -> For LibNoDave
 * Thomas_V2.1    -> For the S7 Protocol Plugin for Wireshark and Information on Step7 Projectfiles

 LibNoDaveConnectionLibrary is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 LibNoDaveConnectionLibrary is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/
using System;
using System.Collections.Generic;
using LibNoDaveConnectionLibrary.DataTypes.Blocks;

namespace LibNoDaveConnectionLibrary.MC7
{
    internal static class NetWork
    {
        public static int[] GetNetworks(int Start, byte[] BD)
        {
            if (BD != null)
            {
                if (BD.Length == 0)
                    return null;
                int i;
                int[] Networks = new int[BitConverter.ToUInt16(BD, Start)]; // Helper.GetWord(BD[Start + 1], BD[Start])];
                for (i = 1; i <= BitConverter.ToUInt16(BD, Start) /*Helper.GetWord(BD[Start + 1], BD[Start])*/- 1; i++)
                {
                    Networks[i] = BitConverter.ToUInt16(BD, Start + (i*2)); // Helper.GetWord(BD[Start + (i * 2) + 1], BD[Start + (i * 2)]);
                }

                return Networks;
            }
            return null;
        }


        public static void NetworkCheck(ref int[] Networks, ref List<PLCFunctionBlockRow> myVal, ref int counter, int oldpos, int pos, ref int NNr)
        {
            counter += pos - oldpos;

            while (NNr <= Networks.Length && counter - Networks[NNr - 1] == 0)
            {
                myVal.Add(new PLCFunctionBlockRow() {Command = "NETWORK", Parameter = NNr.ToString()});
                counter = 0;
                NNr++;
            }
        }
    }
}
