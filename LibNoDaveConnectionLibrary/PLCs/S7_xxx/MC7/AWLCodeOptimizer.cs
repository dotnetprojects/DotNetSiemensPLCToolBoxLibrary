/*
 This implements a high level Wrapper between libnodave.dll and applications written
 in MS .Net languages.
 
 This ConnectionLibrary was written by Jochen Kuehner
 * http://jfk-solutuions.de/
 * 
 * Thanks go to:
 * Steffen Krayer -> For his work on MC7 decoding and the Source for his Decoder
 * Zottel         -> For LibNoDave

 WPFToolboxForSiemensPLCs is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 WPFToolboxForSiemensPLCs is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/
using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    /*
     * This class is for optimizing AWL Code! It will remove unessecary Commands like multiple opens and so on!     
     */

    public static class AWLCodeOptimizer
    {
        public static void OptimizeAWL(S7FunctionBlock myPLCBlock, int akMemnoic)
        {
            int MN = 0;
            List<S7FunctionBlockRow> delLst = new List<S7FunctionBlockRow>();

            string akdb = "";

            for (int n = 0; n < myPLCBlock.AWLCode.Count; n++)
            //foreach (var myAkVal in myPLCBlock.AWLCode)
            {
                S7FunctionBlockRow myAkVal = (S7FunctionBlockRow)myPLCBlock.AWLCode[n];

                if (myAkVal.Command == Mnemonic.opNOP[MN] || myAkVal.Command == Mnemonic.opBLD[MN])
                {
                    if (myAkVal.Label == "")
                        delLst.Add(myAkVal);
                    else if (myAkVal.Label != "" && n < myPLCBlock.AWLCode.Count - 1 && myPLCBlock.AWLCode[n + 1].Label == "")
                    {
                        myPLCBlock.AWLCode[n + 1].Label = myAkVal.Label;
                        delLst.Add(myAkVal);
                    }
                }
                else if (myAkVal.Command == Mnemonic.opAUF[MN] && myAkVal.Label == "")
                    if (akdb == myAkVal.Parameter && !akdb.Contains("[") && myAkVal.Label == "")
                        delLst.Add(myAkVal);
                    else
                        akdb = myAkVal.Parameter;
                else if (myAkVal.Label != "" || myAkVal.Command == Mnemonic.opUC[akMemnoic] || myAkVal.Command == Mnemonic.opCC[akMemnoic]) //If there is a Jump or Call, reset the actual DB!
                    akdb = "";

            }
            foreach (var myAkVal in delLst)
                myPLCBlock.AWLCode.Remove(myAkVal);
        }
    }
}
