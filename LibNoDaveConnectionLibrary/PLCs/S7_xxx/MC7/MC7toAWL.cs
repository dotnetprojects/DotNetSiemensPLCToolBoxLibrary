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
using System;
using System.Collections.Generic;
using System.Globalization;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    internal static class MC7toAWL
    {
        static MC7toAWL()
        {
            numberFormat = new NumberFormatInfo();
            numberFormat.NumberDecimalSeparator = ".";
            numberFormat.NumberGroupSeparator = "";
            numberFormat.CurrencyDecimalSeparator = ".";
            numberFormat.CurrencyGroupSeparator = "";
            numberFormat.PercentDecimalSeparator = ".";
            numberFormat.PercentGroupSeparator = "";
        }
        private static NumberFormatInfo numberFormat;
       
        internal static List<FunctionBlockRow> GetAWL(int Start, int Count, int MN, byte[] BD, int[] Networks, List<string> ParaList, S7ProgrammFolder prjBlkFld, S7FunctionBlock block, S7DataRow blockInterface)
        {
            var retVal = new List<FunctionBlockRow>();

            //bool CombineDBOpenAndCommand = false; // true; // false; //If DB Open and Acess should be One AWL Line (like in Step 7). This should be a Parameter.

            int NWNr = 1;

            int pos = Start;

            int counter = 0;
            int oldpos = pos;

            while (pos <= (Start + Count) - 2)
            {
                //if (retVal.Count >103)  //For Bugfixing, The Rownumber in the AWL Editor - 2
                //    pos=pos;

                if (Networks != null)
                    NetWork.NetworkCheck(ref Networks, ref retVal, ref counter, oldpos, pos, ref NWNr);
                oldpos = pos;

                if (BD[pos] == 0x00 && BD[pos + 1] == 0x00)
                {
                    retVal.Add(new S7FunctionBlockRow()
                                   {
                                       Command = Mnemonic.opNOP[MN],
                                       Parameter = "0"
                                   });

                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                    pos += 2;
                }
                else if (BD[pos] == 0x01 && BD[pos + 1] == 0x00)
                {
                    retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opINVI[MN] });
                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                    pos += 2;
                }
                else if (BD[pos] == 0x05 && BD[pos + 1] == 0x00)
                {
                    retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opBEB[MN] });
                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                    pos += 2;
                }
                else if (BD[pos] == 0x09 && BD[pos + 1] == 0x00)
                {
                    retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opNEGI[MN] });
                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                    pos += 2;
                }
                else if (BD[pos] == 0x49 && BD[pos + 1] == 0x00)
                {
                    retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opOW[MN] });
                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                    pos += 2;
                }
                else if (BD[pos] == 0x00 || BD[pos] == 0x01 || BD[pos] == 0x05 || BD[pos] == 0x09 || BD[pos] == 0x49)
                {
                    string curr_op = "";
                    string curr_ad = "";
                    int HighByte = BD[pos + 1] & 0xF0; //Only Highbyte
                    switch (BD[pos])
                    {
                        case (0x00):
                            curr_op = HighByte < 0x90 ? Mnemonic.opU[MN] : Mnemonic.opUN[MN];
                            break;
                        case (0x01):
                            curr_op = HighByte < 0x90 ? Mnemonic.opO[MN] : Mnemonic.opON[MN];
                            break;
                        case (0x05):
                            curr_op = HighByte < 0x90 ? Mnemonic.opX[MN] : Mnemonic.opXN[MN];
                            break;
                        case (0x09):
                            curr_op = HighByte < 0x90 ? Mnemonic.opS[MN] : Mnemonic.opR[MN];
                            break;
                        case (0x49):
                            curr_op = HighByte < 0x90 ? Mnemonic.opFP[MN] : Mnemonic.opFN[MN];
                            break;
                    }

                    string par = "";
                    byte[] DBByte = null;

                    switch (HighByte)
                    {
                        case (0x10):
                        case (0x90):
                            curr_ad = Mnemonic.adE[MN];
                            break;
                        case (0x20):
                        case (0xA0):
                            curr_ad = Mnemonic.adA[MN];
                            break;
                        case (0x30):
                        case (0xB0):
                            curr_ad = Mnemonic.adM[MN];
                            break;
                        case (0x40):
                        case (0xC0):
                            if (retVal[retVal.Count - 1].Command == Mnemonic.opAUF[MN] && !((S7FunctionBlockRow)retVal[retVal.Count - 1]).Parameter.Contains("["))// && CombineDBOpenAndCommand)
                            {
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).CombineDBAccess = true;                                
                            }
                            curr_ad = Mnemonic.adDBX[MN];
                            break;
                        case (0x50):
                        case (0xD0):
                            curr_ad = Mnemonic.adDIX[MN];
                            break;
                        case (0x60):
                        case (0xE0):
                            curr_ad = Mnemonic.adL[MN];
                            break;
                    }
                    int LowByte = BD[pos + 1] & 0x0F;

                    retVal.Add(new S7FunctionBlockRow()
                                   {
                                       Command = curr_op,
                                       Parameter =
                                           par + curr_ad + " " + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "." +
                                           Convert.ToString(BD[pos + 1] - HighByte)
                                   });
                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                    if (DBByte != null)
                        ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = Helper.CombineByteArray(DBByte, ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7);

                    pos += 4;

                }
                else
                    switch (BD[pos])
                    {
                        case 0x00:
                            {
                                switch (BD[pos + 1])
                                {
                                    case 0x00:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opNOP[MN], Parameter = "0" });
                                        ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                        pos += 2;
                                        break;
                                    default:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "errx" });
                                        ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                        pos += 4;
                                        break;
                                }
                            }
                            break;
                        case 0x02:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opL[MN], Parameter = Mnemonic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x04:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opFR[MN], Parameter = Mnemonic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x0A:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opL[MN], Parameter = Mnemonic.adMB[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x0B:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opT[MN], Parameter = Mnemonic.adMB[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x0C:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opLC[MN], Parameter = Mnemonic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x10:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opBLD[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x11:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opINC[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x12:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opL[MN], Parameter = Mnemonic.adMW[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x13:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opT[MN], Parameter = Mnemonic.adMW[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x14:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSA[MN], Parameter = Mnemonic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x19:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opDEC[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x1A:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opL[MN], Parameter = Mnemonic.adMD[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x1b:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opT[MN], Parameter = Mnemonic.adMD[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x1C:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSV[MN], Parameter = Mnemonic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x1D:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opCC[MN], Parameter = Mnemonic.adFC[MN] + Convert.ToString(BD[pos + 1]) });
                                //Result = Result + Memnoic.opCC[MN] + Memnoic.adFC[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };
                                pos += 6;
                            }
                            break;
                        case 0x20:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opAUF[MN], Parameter = Mnemonic.adDB[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x21:
                            {
                                switch (BD[pos + 1])
                                {
                                    case 0x20:
                                        retVal.Add(new S7FunctionBlockRow() { Command = ">I" });
                                        break;
                                    case 0x40:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "<I" });
                                        break;
                                    case 0x60:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "<>I" });
                                        break;
                                    case 0x80:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "==I" });
                                        break;
                                    case 0xA0:
                                        retVal.Add(new S7FunctionBlockRow() { Command = ">=I" });
                                        break;
                                    case 0xC0:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "<=I" });
                                        break;
                                }
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x24:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSE[MN], Parameter = Mnemonic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x28:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opL[MN], Parameter = "B#16#" + (BD[pos + 1]).ToString("X") });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x29:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSLD[MN], Parameter = (BD[pos + 1]).ToString() });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x2C:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSS[MN], Parameter = Mnemonic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x30:
                            {
                                //Result = Result + Memnoic.opL[MN];
                                string par = "";
                                switch (BD[pos + 1])
                                {
                                    case 0x02:
                                        par = "2#" + Helper.GetWordBool(BD[pos + 2], BD[pos + 3]);
                                        break;
                                    case 0x03:
                                        par = Convert.ToString(libnodave.getS16from(BD, pos + 2)); //Helper.GetInt(BD[pos + 2], BD[pos + 3]));
                                        break;
                                    case 0x05:
                                        par = Helper.GetS7String(pos + 2, 2, BD);
                                        break;
                                    case 0x06:
                                        par = "B#(" + Convert.ToString(BD[pos + 2]) + ", " +
                                                 Convert.ToString(BD[pos + 3]) + ")";
                                        break;
                                    case 0x00:
                                    case 0x07:
                                        par = "W#16#" + libnodave.getU16from(BD, pos + 2).ToString("X"); // Helper.GetWord(BD[pos + 2], BD[pos + 3]).ToString("X");
                                        break;
                                    case 0x08:
                                        par = "C#" + libnodave.getU16from(BD, pos + 2).ToString("X"); // Helper.GetWord(BD[pos + 2], BD[pos + 3]).ToString("X");
                                        break;
                                    case 0x0A:
                                        par = Helper.GetDate(BD[pos + 2], BD[pos + 3]);
                                        break;
                                    case 0x0C:
                                        par = Helper.GetS5Time(BD[pos + 2], BD[pos + 3]);
                                        break;
                                }
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opL[MN], Parameter = par });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                pos += 4;
                            }
                            break;
                        case 0x31:
                            {
                                switch (BD[pos + 1])
                                {
                                    case 0x20:
                                        retVal.Add(new S7FunctionBlockRow() { Command = ">R" });
                                        break;
                                    case 0x40:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "<R" });
                                        break;
                                    case 0x60:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "<>R" });
                                        break;
                                    case 0x80:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "==R" });
                                        break;
                                    case 0xA0:
                                        retVal.Add(new S7FunctionBlockRow() { Command = ">=R" });
                                        break;
                                    case 0xC0:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "<=R" });
                                        break;
                                }
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x34:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSI[MN], Parameter = Mnemonic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x38:
                            {
                                string par = "";
                                switch (BD[pos + 1])
                                {
                                    case 0x01:
                                        par = libnodave.getFloatfrom(BD, pos + 2).ToString("0.000000e+000", numberFormat);
                                        break;
                                    case 0x02:
                                        par = "2#" +
                                                 Helper.GetDWordBool(BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5]);
                                        break;
                                    case 0x03:
                                        par = "L#" + Convert.ToString(libnodave.getS32from(BD, pos + 2));
                                        break;
                                    case 0x04:
                                        par = Helper.GetPointer(BD, pos + 2, MN);
                                        break;
                                    case 0x05:
                                        par = Helper.GetS7String(pos + 2, 4, BD);
                                        break;
                                    case 0x06:
                                        par = "B#(" + Convert.ToString(BD[pos + 2]) + ", " +
                                                 Convert.ToString(BD[pos + 3]) + ", " + Convert.ToString(BD[pos + 4]) +
                                                 ", " +
                                                 Convert.ToString(BD[pos + 5]) + ")";
                                        break;
                                    case 0x07:
                                        par = "DW#16#" + libnodave.getU32from(BD, pos + 2).ToString("X");
                                        break;
                                    case 0x09:
                                        par = Helper.GetDTime(BD, pos + 2);
                                        break;
                                    case 0x0b:
                                        par = Helper.GetTOD(BD, pos + 2);
                                        break;
                                }
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opL[MN], Parameter = par });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };
                                pos += 6;
                            }
                            break;
                        case 0x39:
                            {
                                switch (BD[pos + 1])
                                {
                                    case 0x20:
                                        retVal.Add(new S7FunctionBlockRow() { Command = ">D" });
                                        //Result = Result + ">D";
                                        break;
                                    case 0x40:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "<D" });
                                        //Result = Result + "<D";
                                        break;
                                    case 0x60:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "<>D" });
                                        //Result = Result + "<>D";
                                        break;
                                    case 0x80:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "==D" });
                                        //Result = Result + "==D";
                                        break;
                                    case 0xA0:
                                        retVal.Add(new S7FunctionBlockRow() { Command = ">=D" });
                                        //Result = Result + ">=D";
                                        break;
                                    case 0xC0:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "<=D" });
                                        //Result = Result + "<=D";
                                        break;
                                }
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x3C:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opR[MN], Parameter = Mnemonic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x3D:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opUC[MN], Parameter = Mnemonic.adFC[MN] + Convert.ToString(BD[pos + 1]) });

                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };

                                //Get Parameter Count                                 
                                int paranz = (BD[pos + 5] / 2) - 1;
                                List<string> tmplst = new List<string>();
                                for (int n = 1; n <= paranz; n++)
                                {
                                    tmplst.Add(Helper.GetFCPointer(BD[pos + 6], BD[pos + 7], BD[pos + 8], BD[pos + 9]));
                                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = Helper.CombineByteArray(((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7, new byte[] { BD[pos + 6], BD[pos + 7], BD[pos + 8], BD[pos + 9] });
                                    pos += 4;
                                }
                                byte[] backup = ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7;
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).ExtParameter = tmplst;
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = backup;

                                pos += 6;
                            }
                            break;
                        case 0x41:
                            {
                                switch (BD[pos + 1])
                                {
                                    case 0x00:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opUW[MN] });
                                            //Result = Result + Memnoic.opUW[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x10:
                                    case 0x11:
                                    case 0x12:
                                    case 0x13:
                                    case 0x14:
                                    case 0x15:
                                    case 0x16:
                                    case 0x17:
                                        retVal.Add(new S7FunctionBlockRow()
                                        {
                                            Command = Mnemonic.opZUW[MN],
                                            Parameter = Mnemonic.adE[MN] +
                                                Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "." +
                                                Convert.ToString(BD[pos + 1] - 0x10)
                                        });
                                        ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                        pos += 4;
                                        break;
                                    case 0x20:
                                    case 0x21:
                                    case 0x22:
                                    case 0x23:
                                    case 0x24:
                                    case 0x25:
                                    case 0x26:
                                    case 0x27:
                                        retVal.Add(new S7FunctionBlockRow()
                                        {
                                            Command = Mnemonic.opZUW[MN],
                                            Parameter = Mnemonic.adA[MN] +
                                                Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "." +
                                                Convert.ToString(BD[pos + 1] - 0x20)
                                        });
                                        ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                        pos += 4;
                                        break;
                                    case 0x30:
                                    case 0x31:
                                    case 0x32:
                                    case 0x33:
                                    case 0x34:
                                    case 0x35:
                                    case 0x36:
                                    case 0x37:
                                        retVal.Add(new S7FunctionBlockRow()
                                        {
                                            Command = Mnemonic.opZUW[MN],
                                            Parameter = Mnemonic.adM[MN] +
                                                Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "." +
                                                Convert.ToString(BD[pos + 1] - 0x30)
                                        });
                                        ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                        pos += 4;
                                        break;
                                    case 0x40:  //Zuweisung with DB
                                    case 0x41:
                                    case 0x42:
                                    case 0x43:
                                    case 0x44:
                                    case 0x45:
                                    case 0x46:
                                    case 0x47:
                                        {
                                            string par = "";
                                            byte[] DBByte = null;
                                            if (retVal[retVal.Count - 1].Command == Mnemonic.opAUF[MN] && !((S7FunctionBlockRow)retVal[retVal.Count - 1]).Parameter.Contains("["))// && CombineDBOpenAndCommand)
                                            {
                                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).CombineDBAccess = true;                                                
                                            }
                                            retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opZUW[MN], Parameter = par + Mnemonic.adDBX[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "." + Convert.ToString(BD[pos + 1] - 0x40) });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            if (DBByte != null)
                                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = Helper.CombineByteArray(DBByte, ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7);
                                            pos += 4;
                                            break;
                                        }
                                    case 0x50: //Zuweisung with DI
                                    case 0x51:
                                    case 0x52:
                                    case 0x53:
                                    case 0x54:
                                    case 0x55:
                                    case 0x56:
                                    case 0x57:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                                           {
                                                               Command = Mnemonic.opZUW[MN],
                                                               Parameter = Mnemonic.adDIX[MN] +
                                                                           Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                                           "." +
                                                                           Convert.ToString(BD[pos + 1] - 0x50)
                                                           });                                        
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                            break;
                                        }
                                    case 0x60://Zuweisung without DB
                                    case 0x61:
                                    case 0x62:
                                    case 0x63:
                                    case 0x64:
                                    case 0x65:
                                    case 0x66:
                                    case 0x67:
                                        retVal.Add(new S7FunctionBlockRow()
                                        {
                                            Command = Mnemonic.opZUW[MN],
                                            Parameter = Mnemonic.adL[MN] +
                                                Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "." +
                                                Convert.ToString(BD[pos + 1] - 0x60)
                                        });
                                        ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                        pos += 4;
                                        break;
                                    default:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "erra" });
                                        ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                        pos += 4;
                                        break;
                                }
                            }
                            break;
                        case 0x42:
                            {
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opL[MN],
                                    Parameter = Mnemonic.adZ[MN] + Convert.ToString(BD[pos + 1])
                                });
                                //Result = Result + Memnoic.opL[MN] + Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x44:
                            {
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opFR[MN],
                                    Parameter = Mnemonic.adZ[MN] + Convert.ToString(BD[pos + 1])
                                });
                                //Result = Result + Memnoic.opFR[MN] + Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x4A:
                            {
                                if (BD[pos + 1] < 0x80)
                                    retVal.Add(new S7FunctionBlockRow()
                                                   {
                                                       Command = Mnemonic.opL[MN],
                                                       Parameter = Mnemonic.adEB[MN] + Convert.ToString(BD[pos + 1])
                                                   });
                                else
                                    retVal.Add(new S7FunctionBlockRow()
                                    {
                                        Command = Mnemonic.opL[MN],
                                        Parameter = Mnemonic.adAB[MN] + Convert.ToString(BD[pos + 1] - 0x80)
                                    });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x4b:
                            {
                                if (BD[pos + 1] < 0x80)
                                    retVal.Add(new S7FunctionBlockRow()
                                                   {
                                                       Command = Mnemonic.opT[MN],
                                                       Parameter = Mnemonic.adEB[MN] + Convert.ToString(BD[pos + 1])
                                                   });
                                else
                                    retVal.Add(new S7FunctionBlockRow()
                                                   {
                                                       Command = Mnemonic.opT[MN],
                                                       Parameter = Mnemonic.adAB[MN] + Convert.ToString(BD[pos + 1] - 0x80)
                                                   });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x4C:
                            {
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opLC[MN],
                                    Parameter = Mnemonic.adZ[MN] + Convert.ToString(BD[pos + 1])
                                });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x51:
                        case 0x58:
                        case 0x59:
                            {

                                string cmd = "", par = "";

                                if (BD[pos + 1] == 0x00)
                                {
                                    switch (BD[pos])
                                    {
                                        case 0x51:
                                            cmd = Mnemonic.opXOW[MN];
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                            break;
                                        case 0x58:
                                            cmd = Mnemonic.opPLU[MN];
                                            retVal.Add(new S7FunctionBlockRow()
                                                           {
                                                               Command = cmd,
                                                               Parameter = libnodave.getS16from(BD, pos + 2).ToString()
                                                           });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[]
                                                                               {
                                                                                   BD[pos], BD[pos + 1], BD[pos + 2],
                                                                                   BD[pos + 3]
                                                                               };
                                            pos += 4;
                                            break;
                                        case 0x59:
                                            cmd = "-I";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                            break;
                                    }

                                }
                                else
                                {
                                    int LowByte = BD[pos + 1] & 0x0F; //Only Highbyte
                                    switch (BD[pos])
                                    {
                                        case 0x51:
                                            if (BD[pos + 1] < 0xb0)
                                                cmd = LowByte < 0x09 ? Mnemonic.opU[MN] : Mnemonic.opO[MN];
                                            else
                                                cmd = LowByte < 0x09 ? Mnemonic.opUN[MN] : Mnemonic.opON[MN];
                                            break;
                                        case 0x58:
                                            if (BD[pos + 1] < 0xb0)
                                                cmd = LowByte < 0x09 ? Mnemonic.opX[MN] : Mnemonic.opS[MN];
                                            else
                                                cmd = LowByte < 0x09 ? Mnemonic.opXN[MN] : Mnemonic.opR[MN];
                                            break;
                                        case 0x59:
                                            if (BD[pos + 1] < 0xb0)
                                                cmd = LowByte < 0x09 ? Mnemonic.opZUW[MN] : Mnemonic.opFP[MN];
                                            else
                                                cmd = LowByte < 0x09 ? "err1" : Mnemonic.opFN[MN];//Don't know the Low Byte command
                                            break;
                                    }
                                    switch (BD[pos + 1] & 0x0F)
                                    {
                                        case 0x01:
                                        case 0x09:
                                            par = Mnemonic.adE[MN];
                                            break;
                                        case 0x02:
                                        case 0x0A:
                                            par = Mnemonic.adA[MN];
                                            break;
                                        case 0x03:
                                        case 0x0B:
                                            par = Mnemonic.adM[MN];
                                            break;
                                        case 0x04:
                                        case 0x0C:
                                            par = Mnemonic.adDBX[MN];
                                            break;
                                        case 0x05:
                                        case 0x0D:
                                            par = Mnemonic.adDIX[MN];
                                            break;
                                        case 0x06:
                                        case 0x0E:
                                            par = Mnemonic.adL[MN];
                                            break;

                                    }

                                    switch (BD[pos + 1] & 0xF0)
                                    {

                                        case 0x30:
                                        case 0xb0:
                                            par += Mnemonic.adMD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x40:
                                        case 0xc0:
                                            par += Mnemonic.adDBD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x50:
                                        case 0xd0:
                                            par += Mnemonic.adDID[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x60:
                                        case 0xe0:
                                            par += Mnemonic.adLD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x70:
                                        case 0xf0:
                                            par += Mnemonic.adMD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                    }
                                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                    retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                    pos += 4;
                                }


                            }
                            break;
                        case 0x52:
                            {
                                if (BD[pos + 1] < 0x80)
                                    retVal.Add(new S7FunctionBlockRow()
                                                   {
                                                       Command = Mnemonic.opL[MN],
                                                       Parameter = Mnemonic.adEW[MN] + Convert.ToString(BD[pos + 1])
                                                   });
                                else
                                    retVal.Add(new S7FunctionBlockRow()
                                                   {
                                                       Command = Mnemonic.opL[MN],
                                                       Parameter = Mnemonic.adAW[MN] + Convert.ToString(BD[pos + 1] - 0x80)
                                                   });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x53:
                            {
                                if (BD[pos + 1] < 0x80)
                                    retVal.Add(new S7FunctionBlockRow()
                                                   {
                                                       Command = Mnemonic.opT[MN],
                                                       Parameter = Mnemonic.adEW[MN] + Convert.ToString(BD[pos + 1])
                                                   });
                                //Result = Result + Memnoic.opT[MN] + Memnoic.adEW[MN] + Convert.ToString(BD[pos + 1]);
                                else
                                    retVal.Add(new S7FunctionBlockRow()
                                    {
                                        Command = Mnemonic.opT[MN],
                                        Parameter = Mnemonic.adAW[MN] + Convert.ToString(BD[pos + 1] - 0x80)
                                    });
                                //Result = Result + Memnoic.opT[MN] + Memnoic.adAW[MN] + Convert.ToString(BD[pos + 1] - 0x80);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x54:
                            {
                                if (BD[pos + 1] < 0x80)
                                    retVal.Add(new S7FunctionBlockRow()
                                    {
                                        Command = Mnemonic.opZR[MN],
                                        Parameter = Mnemonic.adZ[MN] + Convert.ToString(BD[pos + 1])
                                    });
                                //Result = Result + Memnoic.opZR[MN] + Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x55:
                            {
                                if (BD[pos + 1] < 0x80)
                                    retVal.Add(new S7FunctionBlockRow()
                                    {
                                        Command = Mnemonic.opCC[MN],
                                        Parameter = Mnemonic.adFB[MN] + Convert.ToString(BD[pos + 1])
                                    });
                                //Result = Result + Memnoic.opCC[MN] + Memnoic.adFB[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x5A:
                            {
                                if (BD[pos + 1] < 0x80)
                                    retVal.Add(new S7FunctionBlockRow()
                                                   {
                                                       Command = Mnemonic.opL[MN],
                                                       Parameter = Mnemonic.adED[MN] + Convert.ToString(BD[pos + 1])
                                                   });
                                //Result = Result + Memnoic.opL[MN] + Memnoic.adED[MN] + Convert.ToString(BD[pos + 1]);
                                else
                                    retVal.Add(new S7FunctionBlockRow()
                                                   {
                                                       Command = Mnemonic.opL[MN],
                                                       Parameter = Mnemonic.adAD[MN] + Convert.ToString(BD[pos + 1] - 0x80)
                                                   });
                                //Result = Result + Memnoic.opL[MN] + Memnoic.adAD[MN] + Convert.ToString(BD[pos + 1] - 0x80);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x5b:
                            {
                                if (BD[pos + 1] < 0x80)
                                    retVal.Add(new S7FunctionBlockRow()
                                    {
                                        Command = Mnemonic.opT[MN],
                                        Parameter = Mnemonic.adED[MN] + Convert.ToString(BD[pos + 1])
                                    });
                                else
                                    retVal.Add(new S7FunctionBlockRow()
                                    {
                                        Command = Mnemonic.opT[MN],
                                        Parameter = Mnemonic.adAD[MN] + Convert.ToString(BD[pos + 1])
                                    });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x5C:
                            {
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opS[MN],
                                    Parameter = Mnemonic.adZ[MN] + Convert.ToString(BD[pos + 1])
                                });
                                //Result = Result + Memnoic.opS[MN] + Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x60:
                            {
                                switch (BD[pos + 1])
                                {
                                    case 0x00:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "/I" });
                                        break;
                                    case 0x01:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opMOD[MN] });
                                        break;
                                    case 0x02:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opABS[MN] });
                                        break;
                                    case 0x03:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "/R" });
                                        break;
                                    case 0x04:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "*I" });
                                        break;
                                    case 0x05:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                                           {
                                                               Command = Mnemonic.opPLU[MN],
                                                               Parameter = "L#" + Convert.ToString(libnodave.getS32from(BD, pos + 2))
                                                           });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };
                                            pos += 6;
                                            goto brk2;
                                        }
                                        break;
                                    case 0x06:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opNEGR[MN] });
                                        break;
                                    case 0x07:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "*R" });
                                        break;
                                    case 0x08:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opENT[MN] });
                                        break;
                                    case 0x09:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "-D" });
                                        break;
                                    case 0x0A:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "*D" });
                                        break;
                                    case 0x0b:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "-R" });
                                        break;
                                    case 0x0D:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "+D" });
                                        break;
                                    case 0x0E:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "/D" });
                                        break;
                                    case 0x0F:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "+R" });
                                        break;
                                    case 0x10:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSIN[MN] });
                                        break;
                                    case 0x11:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opCOS[MN] });
                                        break;
                                    case 0x12:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opTAN[MN] });
                                        break;
                                    case 0x13:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opLN[MN] });
                                        break;
                                    case 0x14:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSQRT[MN] });
                                        break;
                                    case 0x18:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opASIN[MN] });
                                        break;
                                    case 0x19:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opACOS[MN] });
                                        break;
                                    case 0x1A:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opATAN[MN] });
                                        break;
                                    case 0x1b:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opEXP[MN] });
                                        break;
                                    case 0x1C:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSQR[MN] });
                                        break;
                                }
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                        brk2:
                            break;
                        case 0x61:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSLW[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                //Result = Result + Memnoic.opSLW[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x64:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opRLD[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                //Result = Result + Memnoic.opRLD[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x65:
                            {
                                switch (BD[pos + 1])
                                {
                                    case 0x00:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opBE[MN] });
                                        //Result = Result + Memnoic.opBE[MN];
                                        break;
                                    case 0x01:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opBEA[MN] });
                                        //Result = Result + Memnoic.opBEA[MN];
                                        break;
                                }
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x68:
                            {
                                switch (BD[pos + 1])
                                {
                                    case 0x06:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opDTR[MN] });
                                        //Result = Result + Memnoic.opDTR[MN];
                                        break;
                                    case 0x07:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opNEGD[MN] });
                                        //Result = Result + Memnoic.opNEGD[MN];
                                        break;
                                    case 0x08:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opITB[MN] });
                                        //Result = Result + Memnoic.opITB[MN];
                                        break;
                                    case 0x0C:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opBTI[MN] });
                                        //Result = Result + Memnoic.opBTI[MN];
                                        break;
                                    case 0x0A:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opDTB[MN] });
                                        //Result = Result + Memnoic.opDTB[MN];
                                        break;
                                    case 0x0D:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opINVD[MN] });
                                        //Result = Result + Memnoic.opINVD[MN];
                                        break;
                                    case 0x0E:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opBTD[MN] });
                                        //Result = Result + Memnoic.opBTD[MN];
                                        break;
                                    case 0x12:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSLW[MN] });
                                        //Result = Result + Memnoic.opSLW[MN];
                                        break;
                                    case 0x13:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSLD[MN] });
                                        //Result = Result + Memnoic.opSLD[MN];
                                        break;
                                    case 0x17:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opRLD[MN] });
                                        //Result = Result + Memnoic.opRLD[MN];
                                        break;
                                    case 0x18:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opRLDA[MN] });
                                        //Result = Result + Memnoic.opRLDA[MN];
                                        break;
                                    case 0x1A:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opTAW[MN] });
                                        //Result = Result + Memnoic.opTAW[MN];
                                        break;
                                    case 0x1b:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opTAD[MN] });
                                        //Result = Result + Memnoic.opTAD[MN];
                                        break;
                                    case 0x1C:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opCLR[MN] });
                                        //Result = Result + Memnoic.opCLR[MN];
                                        break;
                                    case 0x1D:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSET[MN] });
                                        //Result = Result + Memnoic.opSET[MN];
                                        break;
                                    case 0x1E:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opITD[MN] });
                                        //Result = Result + Memnoic.opITD[MN];
                                        break;
                                    case 0x22:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSRW[MN] });
                                        //Result = Result + Memnoic.opSRW[MN];
                                        break;
                                    case 0x23:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSRD[MN] });
                                        //Result = Result + Memnoic.opSRD[MN];
                                        break;
                                    case 0x24:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSSI[MN] });
                                        //Result = Result + Memnoic.opSSI[MN];
                                        break;
                                    case 0x25:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSSD[MN] });
                                        //Result = Result + Memnoic.opSSD[MN];
                                        break;
                                    case 0x27:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opRRD[MN] });
                                        //Result = Result + Memnoic.opRRD[MN];
                                        break;
                                    case 0x28:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opRRDA[MN] });
                                        //Result = Result + Memnoic.opRRDA[MN];
                                        break;
                                    case 0x2C:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSAVE[MN] });
                                        //Result = Result + Memnoic.opSAVE[MN];
                                        break;
                                    case 0x2D:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opNOT[MN] });
                                        //Result = Result + Memnoic.opNOT[MN];
                                        break;
                                    case 0x2E:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opPUSH[MN] });
                                        //Result = Result + Memnoic.opPUSH[MN];
                                        break;
                                    case 0x37:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opUD[MN] });
                                        //Result = Result + Memnoic.opUD[MN];
                                        break;
                                    case 0x34:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                                           {
                                                               Command = Mnemonic.opUW[MN],
                                                               Parameter = "W#16#" +
                                                                           (libnodave.getU16from(BD, pos + 2)).ToString("X")
                                                           });
                                            //Result = Result + Memnoic.opUW[MN] + "W#16#" +
                                            //         (Helper.GetWord(BD[pos + 2], BD[pos + 3])).ToString("X");
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                            goto brk1;
                                        }
                                        break;
                                    case 0x36:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUD[MN],
                                                Parameter = "DW#16#" + (libnodave.getU32from(BD, pos + 2).ToString("X"))
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };
                                            pos += 6;
                                            goto brk1;
                                        }
                                        break;
                                    case 0x3A:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opMCRA[MN] });
                                        //Result = Result + Memnoic.opMCRA[MN];
                                        break;
                                    case 0x3b:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opMCRD[MN] });
                                        //Result = Result + Memnoic.opMCRD[MN];
                                        break;
                                    case 0x3C:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opMCRO[MN] });
                                        //Result = Result + Memnoic.opMCRO[MN];
                                        break;
                                    case 0x3D:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opMCRC[MN] });
                                        //Result = Result + Memnoic.opMCRC[MN];
                                        break;
                                    case 0x3E:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opPOP[MN] });
                                        //Result = Result + Memnoic.opPOP[MN];
                                        break;
                                    case 0x44:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opOW[MN],
                                                Parameter = "W#16#" +
                                                            (libnodave.getU16from(BD, pos + 2)).ToString("X")
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                            goto brk1;
                                        }
                                        break;
                                    case 0x46:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opOD[MN],
                                                Parameter = "DW#16#" + libnodave.getU32from(BD, pos + 2).ToString("X")
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };
                                            pos += 6;
                                            goto brk1;
                                        }
                                        break;
                                    case 0x47:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opOD[MN] });
                                        break;
                                    case 0x4E:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opLEAVE[MN] });
                                        break;
                                    case 0x54:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXOW[MN],
                                                Parameter = "W#16#" +
                                                     (libnodave.getU16from(BD, pos + 2)).ToString("X")
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                            goto brk1;
                                        }
                                        break;
                                    case 0x56:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXOD[MN],
                                                Parameter = "DW#16#" + libnodave.getU32from(BD, pos + 2).ToString("X")
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };
                                            pos += 6;
                                            goto brk1;
                                        }
                                        break;
                                    case 0x57:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opXOD[MN] });
                                        break;
                                    case 0x5C:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opRND[MN] });
                                        break;
                                    case 0x5D:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opRNDM[MN] });
                                        break;
                                    case 0x5E:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opRNDP[MN] });
                                        break;
                                    case 0x5F:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opTRUNC[MN] });
                                        break;
                                    default:
                                        {
                                            if ((BD[pos + 1] & 0x0F) == 0x01)
                                                retVal.Add(new S7FunctionBlockRow()
                                                               {
                                                                   Command = Mnemonic.opSSI[MN],
                                                                   Parameter = Convert.ToString((BD[pos + 1] >> 4) & 0x0F)
                                                               });
                                            else
                                                retVal.Add(new S7FunctionBlockRow() { Command = "err2" });
                                        }
                                        break;
                                }
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                        brk1:
                            break;
                        case 0x69:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSRW[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                //Result = Result + Memnoic.opSRW[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x6C:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opZV[MN], Parameter = Mnemonic.adZ[MN] + Convert.ToString(BD[pos + 1]) });
                                //Result = Result + Memnoic.opZV[MN] + Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x70:
                            {
                                switch (BD[pos + 1])
                                {
                                    case 0x02:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opTAK[MN] });
                                            //Result = Result + Memnoic.opTAK[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x06:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opL[MN], Parameter = Mnemonic.adSTW[MN] });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x07:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opT[MN], Parameter = Mnemonic.adSTW[MN] });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x08:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                                           {
                                                               Command = Mnemonic.opLOOP[MN],
                                                               JumpWidth = libnodave.getS16from(BD, pos + 2)
                                                           });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x09:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                                           {
                                                               Command = Mnemonic.opSPL[MN],
                                                               JumpWidth = libnodave.getS16from(BD, pos + 2)
                                                           });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x0b:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSPA[MN], JumpWidth = libnodave.getS16from(BD, pos + 2) });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    default:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "err3" });
                                        ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                        pos += 4;
                                        break;
                                }
                            }
                            break;
                        case 0x71:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opSSD[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                //Result = Result + Memnoic.opSSD[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x74:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opRRD[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x75:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opUC[MN], Parameter = Mnemonic.adFB[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x79:
                            {
                                string cmd = "", par = "";

                                if (BD[pos + 1] == 0x00)
                                {
                                    cmd = "+I";
                                    retVal.Add(new S7FunctionBlockRow() { Command = cmd });
                                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                    pos += 2;
                                }
                                else
                                {
                                    int LowByte = BD[pos + 1] & 0x0F;
                                    int HighByte = BD[pos + 1] & 0xF0;
                                    switch (LowByte)
                                    {
                                        case 0x00:
                                        case 0x08:
                                            cmd += (HighByte < 0x90) ? Mnemonic.opU[MN] : Mnemonic.opS[MN];
                                            break;
                                        case 0x01:
                                        case 0x09:
                                            cmd += (HighByte < 0x90) ? Mnemonic.opUN[MN] : Mnemonic.opR[MN];
                                            break;
                                        case 0x02:
                                        case 0x0A:
                                            cmd += (HighByte < 0x90) ? Mnemonic.opO[MN] : Mnemonic.opZUW[MN];
                                            break;
                                        case 0x03:
                                        case 0x0B:
                                            cmd += (HighByte < 0x90) ? Mnemonic.opON[MN] : "err5"; //Ther is no Value for this???
                                            break;
                                        case 0x04:
                                        case 0x0C:
                                            cmd += (HighByte < 0x90) ? Mnemonic.opX[MN] : Mnemonic.opFP[MN];
                                            break;
                                        case 0x05:
                                        case 0x0D:
                                            cmd += (HighByte < 0x90) ? Mnemonic.opXN[MN] : Mnemonic.opFN[MN];
                                            break;
                                    }

                                    switch (HighByte)
                                    {
                                        case 0x10:
                                        case 0x90:
                                            par += Mnemonic.adE[MN];
                                            break;
                                        case 0x20:
                                        case 0xa0:
                                            par += Mnemonic.adA[MN];
                                            break;
                                        case 0x30:
                                        case 0xb0:
                                            par += Mnemonic.adM[MN];
                                            break;
                                        case 0x40:
                                        case 0xc0:
                                            par += Mnemonic.adDBX[MN];
                                            break;
                                        case 0x50:
                                        case 0xd0:
                                            par += Mnemonic.adDIX[MN];
                                            break;
                                        case 0x60:
                                        case 0xe0:
                                            par += Mnemonic.adL[MN];
                                            break;

                                    }
                                    if (LowByte < 0x08)
                                        par += "[" + Mnemonic.adAR1[MN] + "," +
                                               Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                    else
                                        par += "[" + Mnemonic.adAR2[MN] + "," +
                                               Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                    retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                    pos += 4;
                                }


                            }
                            break;
                        case 0x7C:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opR[MN], Parameter = Mnemonic.adZ[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x7E:
                        case 0xBE:
                            {
                                string cmd = "", par = "";

                                if (BD[pos + 1] == 0x00)
                                {
                                    cmd = "err6";
                                    retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                    pos += 2;
                                }
                                else
                                {
                                    int LowByte = BD[pos + 1] & 0x0F;
                                    int HighByte = BD[pos + 1] & 0xF0;
                                    cmd = Mnemonic.opT[MN];
                                    //if (LowByte>= 0x05 && LowByte<=0x09 || LowByte >)
                                    //cmd = (LowByte <= 0x07) ? Memnoic.opL[MN] : Memnoic.opT[MN];
                                    if (LowByte <= 0x04 || ((LowByte >= 0x09) && LowByte <= 0x0c))
                                        cmd = Mnemonic.opL[MN];

                                    switch (HighByte)
                                    {
                                        case 0x00:
                                            switch (LowByte)
                                            {
                                                case 0x01:
                                                case 0x09:
                                                    par += Mnemonic.adPEB[MN];
                                                    break;
                                                case 0x02:
                                                case 0x0A:
                                                    par += Mnemonic.adPEW[MN];
                                                    break;
                                                case 0x03:
                                                case 0x0B:
                                                    par += Mnemonic.adPED[MN];
                                                    break;
                                                case 0x05:
                                                case 0x0D:
                                                    par += Mnemonic.adPAB[MN];
                                                    break;
                                                case 0x06:
                                                case 0x0E:
                                                    par += Mnemonic.adPAW[MN];
                                                    break;
                                                case 0x07:
                                                case 0x0F:
                                                    par += Mnemonic.adPAD[MN];
                                                    break;
                                            }
                                            break;
                                        case 0x10:
                                            switch (LowByte)
                                            {
                                                case 0x01:
                                                case 0x05:
                                                case 0x09:
                                                case 0x0D:
                                                    par += Mnemonic.adEB[MN];
                                                    break;
                                                case 0x02:
                                                case 0x06:
                                                case 0x0A:
                                                case 0x0E:
                                                    par += Mnemonic.adEW[MN];
                                                    break;
                                                case 0x03:
                                                case 0x07:
                                                case 0x0B:
                                                case 0x0F:
                                                    par += Mnemonic.adED[MN];
                                                    break;
                                            }
                                            break;
                                        case 0x20:
                                            switch (LowByte)
                                            {
                                                case 0x01:
                                                case 0x05:
                                                case 0x09:
                                                case 0x0D:
                                                    par += Mnemonic.adAB[MN];
                                                    break;
                                                case 0x02:
                                                case 0x06:
                                                case 0x0A:
                                                case 0x0E:
                                                    par += Mnemonic.adAW[MN];
                                                    break;
                                                case 0x03:
                                                case 0x07:
                                                case 0x0B:
                                                case 0x0F:
                                                    par += Mnemonic.adAD[MN];
                                                    break;
                                            }
                                            break;
                                        case 0x30:
                                            switch (LowByte)
                                            {
                                                case 0x01:
                                                case 0x05:
                                                case 0x09:
                                                case 0x0D:
                                                    par += Mnemonic.adMB[MN];
                                                    break;
                                                case 0x02:
                                                case 0x06:
                                                case 0x0A:
                                                case 0x0E:
                                                    par += Mnemonic.adMW[MN];
                                                    break;
                                                case 0x03:
                                                case 0x07:
                                                case 0x0B:
                                                case 0x0F:
                                                    par += Mnemonic.adMD[MN];
                                                    break;
                                            }
                                            break;
                                        case 0x40:
                                            switch (LowByte)
                                            {
                                                case 0x01:
                                                case 0x05:
                                                case 0x09:
                                                case 0x0D:
                                                    par += Mnemonic.adDBB[MN];
                                                    break;
                                                case 0x02:
                                                case 0x06:
                                                case 0x0A:
                                                case 0x0E:
                                                    par += Mnemonic.adDBW[MN];
                                                    break;
                                                case 0x03:
                                                case 0x07:
                                                case 0x0B:
                                                case 0x0F:
                                                    par += Mnemonic.adDBD[MN];
                                                    break;
                                            }
                                            break;
                                        case 0x50:
                                            switch (LowByte)
                                            {
                                                case 0x01:
                                                case 0x05:
                                                case 0x09:
                                                case 0x0D:
                                                    par += Mnemonic.adDIB[MN];
                                                    break;
                                                case 0x02:
                                                case 0x06:
                                                case 0x0A:
                                                case 0x0E:
                                                    par += Mnemonic.adDIW[MN];
                                                    break;
                                                case 0x03:
                                                case 0x07:
                                                case 0x0B:
                                                case 0x0F:
                                                    par += Mnemonic.adDID[MN];
                                                    break;
                                            }
                                            break;
                                        case 0x60:
                                            switch (LowByte)
                                            {
                                                case 0x01:
                                                case 0x05:
                                                case 0x09:
                                                case 0x0D:
                                                    par += Mnemonic.adLB[MN];
                                                    break;
                                                case 0x02:
                                                case 0x06:
                                                case 0x0A:
                                                case 0x0E:
                                                    par += Mnemonic.adLW[MN];
                                                    break;
                                                case 0x03:
                                                case 0x07:
                                                case 0x0B:
                                                case 0x0F:
                                                    par += Mnemonic.adLD[MN];
                                                    break;
                                            }
                                            break;
                                    }

                                    if (BD[pos] == 0x7E)
                                        par += Convert.ToString(libnodave.getU16from(BD, pos + 2));
                                    else if (BD[pos] == 0xBE)
                                        if (LowByte < 0x09)
                                            par += "[" + Mnemonic.adAR1[MN] + "," +
                                                         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                        else
                                            par += "[" + Mnemonic.adAR2[MN] + "," +
                                                     Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";


                                    //new
                                    byte[] DBByte = null;
                                    if (retVal[retVal.Count - 1].Command == Mnemonic.opAUF[MN] && par.Substring(0, 2) == "DB")// && CombineDBOpenAndCommand)
                                    {
                                        ((S7FunctionBlockRow)retVal[retVal.Count - 1]).CombineDBAccess = true;                                       
                                    }
                                    retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                    if (DBByte != null)
                                        ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = Helper.CombineByteArray(DBByte, ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7);
                                    pos += 4;
                                }

                            }
                            break;

                        case 0x80:
                        case 0x81:
                        case 0x82:
                        case 0x83:
                        case 0x84:
                        case 0x85:
                        case 0x86:
                        case 0x87:
                            retVal.Add(new S7FunctionBlockRow()
                                           {
                                               Command = Mnemonic.opU[MN],
                                               Parameter = Mnemonic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
                                                           Convert.ToString(BD[pos] - 0x80)
                                           });
                            //Result = Result + Memnoic.opU[MN] + Memnoic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0x80);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0x88:
                        case 0x89:
                        case 0x8A:
                        case 0x8B:
                        case 0x8C:
                        case 0x8D:
                        case 0x8E:
                        case 0x8F:
                            retVal.Add(new S7FunctionBlockRow()
                            {
                                Command = Mnemonic.opO[MN],
                                Parameter = Mnemonic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
                                            Convert.ToString(BD[pos] - 0x88)
                            });
                            //Result = Result + Memnoic.opO[MN] + Memnoic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0x88);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0x90:
                        case 0x91:
                        case 0x92:
                        case 0x93:
                        case 0x94:
                        case 0x95:
                        case 0x96:
                        case 0x97:
                            retVal.Add(new S7FunctionBlockRow()
                            {
                                Command = Mnemonic.opS[MN],
                                Parameter = Mnemonic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
                                            Convert.ToString(BD[pos] - 0x90)
                            });
                            //Result = Result + Memnoic.opS[MN] + Memnoic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0x90);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0x98:
                        case 0x99:
                        case 0x9A:
                        case 0x9B:
                        case 0x9C:
                        case 0x9D:
                        case 0x9E:
                        case 0x9F:
                            retVal.Add(new S7FunctionBlockRow()
                            {
                                Command = Mnemonic.opZUW[MN],
                                Parameter = Mnemonic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
                                            Convert.ToString(BD[pos] - 0x98)
                            });
                            //Result = Result + Memnoic.opZUW[MN] + Memnoic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0x98);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xA0:
                        case 0xA1:
                        case 0xA2:
                        case 0xA3:
                        case 0xA4:
                        case 0xA5:
                        case 0xA6:
                        case 0xA7:
                            retVal.Add(new S7FunctionBlockRow()
                            {
                                Command = Mnemonic.opUN[MN],
                                Parameter = Mnemonic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
                                            Convert.ToString(BD[pos] - 0xA0)
                            });
                            //Result = Result + Memnoic.opUN[MN] + Memnoic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xA0);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xA8:
                        case 0xA9:
                        case 0xAA:
                        case 0xAB:
                        case 0xAC:
                        case 0xAD:
                        case 0xAE:
                        case 0xAF:
                            retVal.Add(new S7FunctionBlockRow()
                            {
                                Command = Mnemonic.opON[MN],
                                Parameter = Mnemonic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
                                            Convert.ToString(BD[pos] - 0xA8)
                            });
                            //Result = Result + Memnoic.opON[MN] + Memnoic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xA8);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xB0:
                        case 0xB1:
                        case 0xB2:
                        case 0xB3:
                        case 0xB4:
                        case 0xB5:
                        case 0xB6:
                        case 0xB7:
                            retVal.Add(new S7FunctionBlockRow()
                            {
                                Command = Mnemonic.opR[MN],
                                Parameter = Mnemonic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
                                            Convert.ToString(BD[pos] - 0xB0)
                            });
                            //Result = Result + Memnoic.opR[MN] + Memnoic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xB0);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xB8:
                            {
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opU[MN],
                                    Parameter = Mnemonic.adZ[MN] + Convert.ToString(BD[pos + 1])
                                });
                                //Result = Result + Memnoic.opU[MN] + Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0xB9:
                            {
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opO[MN],
                                    Parameter = Mnemonic.adZ[MN] + Convert.ToString(BD[pos + 1])
                                });
                                //Result = Result + Memnoic.opO[MN] + Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0xBA:
                        case 0xBB:
                            {
                                int LowByte = BD[pos + 1] & 0x0F;
                                int HighByte = BD[pos + 1] & 0xF0;

                                string cmd = "", par = "";

                                if (BD[pos + 1] == 0x00)
                                {
                                    switch (BD[pos])
                                    {
                                        case 0xBA:
                                            cmd = cmd + Mnemonic.opUO[MN];
                                            break;
                                        case 0xBB:
                                            cmd = cmd + Mnemonic.opOO[MN];
                                            break;
                                    }
                                    retVal.Add(new S7FunctionBlockRow()
                                    {
                                        Command = cmd,
                                        Parameter = par
                                    });
                                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                    pos += 2;
                                }
                                else
                                {
                                    cmd += HighByte < 0x80 ? Mnemonic.opL[MN] : Mnemonic.opT[MN];

                                    switch (BD[pos])
                                    {
                                        case 0xBA:
                                            switch (LowByte)
                                            {
                                                case 0x00:
                                                    par += HighByte < 0x80 ? Mnemonic.adPEB[MN] : Mnemonic.adPAB[MN];
                                                    break;
                                                case 0x01:
                                                    par += Mnemonic.adEB[MN];
                                                    break;
                                                case 0x02:
                                                    par += Mnemonic.adAB[MN];
                                                    break;
                                                case 0x03:
                                                    par += Mnemonic.adMB[MN];
                                                    break;
                                                case 0x04:
                                                    par += Mnemonic.adDBB[MN];
                                                    break;
                                                case 0x05:
                                                    par += Mnemonic.adDIB[MN];
                                                    break;
                                                case 0x06:
                                                    par += Mnemonic.adLB[MN];
                                                    break;
                                            }
                                            break;
                                        case 0xBB:
                                            switch (LowByte)
                                            {
                                                case 0x00:
                                                    par += HighByte < 0x80 ? Mnemonic.adPEW[MN] : Mnemonic.adPAW[MN];
                                                    break;
                                                case 0x01:
                                                    par += Mnemonic.adEW[MN];
                                                    break;
                                                case 0x02:
                                                    par += Mnemonic.adAW[MN];
                                                    break;
                                                case 0x03:
                                                    par += Mnemonic.adMW[MN];
                                                    break;
                                                case 0x04:
                                                    par += Mnemonic.adDBW[MN];
                                                    break;
                                                case 0x05:
                                                    par += Mnemonic.adDIW[MN];
                                                    break;
                                                case 0x06:
                                                    par += Mnemonic.adLW[MN];
                                                    break;
                                                case 0x08:
                                                    par += HighByte < 0x80 ? Mnemonic.adPED[MN] : Mnemonic.adPAD[MN];
                                                    break;
                                                case 0x09:
                                                    par += Mnemonic.adED[MN];
                                                    break;
                                                case 0x0A:
                                                    par += Mnemonic.adAD[MN];
                                                    break;
                                                case 0x0B:
                                                    par += Mnemonic.adMD[MN];
                                                    break;
                                                case 0x0C:
                                                    par += Mnemonic.adDBD[MN];
                                                    break;
                                                case 0x0D:
                                                    par += Mnemonic.adDID[MN];
                                                    break;
                                                case 0x0E:
                                                    par += Mnemonic.adLD[MN];
                                                    break;
                                            }
                                            break;
                                    }

                                    switch (HighByte)
                                    {
                                        case 0x00: //Not from old programm, guessed Values!
                                        case 0x80:
                                            par += "[" + Mnemonic.adPED[MN] +
                                                   Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x10://Not from old programm, guessed Values!
                                        case 0x90:
                                            par += "[" + Mnemonic.adED[MN] +
                                                   Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x20://Not from old programm, guessed Values!
                                        case 0xa0:
                                            par += "[" + Mnemonic.adAD[MN] +
                                                   Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x30:
                                        case 0xb0:
                                            par += "[" + Mnemonic.adMD[MN] +
                                                   Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x40:
                                        case 0xc0:
                                            par += "[" + Mnemonic.adDBD[MN] +
                                                   Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x50:
                                        case 0xd0:
                                            par += "[" + Mnemonic.adDID[MN] +
                                                   Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x60:
                                        case 0xe0:
                                            par += "[" + Mnemonic.adLD[MN] +
                                                   Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                    }
                                    retVal.Add(new S7FunctionBlockRow()
                                    {
                                        Command = cmd,
                                        Parameter = par
                                    });
                                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                    pos += 4;
                                }

                            }
                            break;
                        case 0xBF:
                            {
                                string cmd = "", par = "";

                                if (BD[pos + 1] == 0x00)
                                {
                                    cmd = ")";
                                    retVal.Add(new S7FunctionBlockRow()
                                    {
                                        Command = cmd,
                                        Parameter = par
                                    });
                                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                    pos += 2;
                                }
                                else
                                {
                                    int LowByte = BD[pos + 1] & 0x0F;
                                    int HighByte = BD[pos + 1] & 0xF0;
                                    switch (LowByte)
                                    {
                                        case 0x00:
                                            cmd += Mnemonic.opU[MN];
                                            break;
                                        case 0x01:
                                            cmd += Mnemonic.opUN[MN];
                                            break;
                                        case 0x02:
                                            cmd += Mnemonic.opO[MN];
                                            break;
                                        case 0x03:
                                            cmd += Mnemonic.opON[MN];
                                            break;
                                        case 0x04:
                                            cmd += Mnemonic.opX[MN];
                                            break;
                                        case 0x05:
                                            cmd += Mnemonic.opXN[MN];
                                            break;
                                        case 0x06:
                                            cmd += Mnemonic.opL[MN];
                                            break;
                                        case 0x08:
                                            cmd += Mnemonic.opFR[MN];
                                            break;
                                        case 0x09:
                                            cmd += Mnemonic.opLC[MN];
                                            break;
                                        case 0x0A:
                                            cmd += HighByte < 0x80 ? Mnemonic.opSA[MN] : Mnemonic.opZR[MN];
                                            break;
                                        case 0x0B:
                                            cmd += HighByte < 0x80 ? Mnemonic.opSV[MN] : Mnemonic.opS[MN];
                                            break;
                                        case 0x0C:
                                            cmd += Mnemonic.opSE[MN];
                                            break;
                                        case 0x0D:
                                            cmd += HighByte < 0x80 ? Mnemonic.opSS[MN] : Mnemonic.opZV[MN];
                                            break;
                                        case 0x0E:
                                            cmd += Mnemonic.opSI[MN];
                                            break;
                                        case 0x0F:
                                            cmd += Mnemonic.opR[MN];
                                            break;
                                    }
                                    if (HighByte < 0x80)
                                        par += Mnemonic.adT[MN];
                                    else
                                        par += Mnemonic.adZ[MN];

                                    switch (HighByte)
                                    {
                                        case 0x00://Guessed Value, not from old PRG
                                        case 0x80:
                                            par += "[" + Mnemonic.adPEW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]";
                                            break;
                                        case 0x10://Guessed Value, not from old PRG
                                        case 0x90:
                                            par += "[" + Mnemonic.adEW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]";
                                            break;
                                        case 0x20://Guessed Value, not from old Prg
                                        case 0xa0:
                                            par += "[" + Mnemonic.adAW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]";
                                            break;
                                        case 0x30:
                                        case 0xb0:
                                            par += "[" + Mnemonic.adMW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]";
                                            break;
                                        case 0x40:
                                        case 0xc0:
                                            par += "[" + Mnemonic.adDBW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]";
                                            break;
                                        case 0x50:
                                        case 0xd0:
                                            par += "[" + Mnemonic.adDIW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]";
                                            break;
                                        case 0x60:
                                        case 0xe0:
                                            par += "[" + Mnemonic.adLW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]";
                                            break;
                                    }
                                    retVal.Add(new S7FunctionBlockRow()
                                    {
                                        Command = cmd,
                                        Parameter = par
                                    });
                                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                    pos += 4;
                                }

                            }
                            break;
                        case 0xC0:
                        case 0xC1:
                        case 0xC2:
                        case 0xC3:
                        case 0xC4:
                        case 0xC5:
                        case 0xC6:
                        case 0xC7:
                            if (BD[pos + 1] < 0x80)
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opU[MN],
                                    Parameter = Mnemonic.adE[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xC0)
                                });
                            //Result = Result + Memnoic.opU[MN] + Memnoic.adE[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xC0);
                            else
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opU[MN],
                                    Parameter = Mnemonic.adA[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xC0)
                                });
                            //Result = Result + Memnoic.opU[MN] + Memnoic.adA[MN] + Convert.ToString(BD[pos + 1] - 0x80) + "." +
                            //         Convert.ToString(BD[pos] - 0xC0);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;

                            break;
                        case 0xC8:
                        case 0xC9:
                        case 0xCA:
                        case 0xCB:
                        case 0xCC:
                        case 0xCD:
                        case 0xCE:
                        case 0xCF:
                            if (BD[pos + 1] < 0x80)
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opO[MN],
                                    Parameter = Mnemonic.adE[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xC8)
                                });
                            //Result = Result + Memnoic.opO[MN] + Memnoic.adE[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xC8);
                            else
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opO[MN],
                                    Parameter = Mnemonic.adA[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xC8)
                                });
                            //Result = Result + Memnoic.opO[MN] + Memnoic.adA[MN] + Convert.ToString(BD[pos + 1] - 0x80) + "." +
                            //         Convert.ToString(BD[pos] - 0xC8);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;

                            break;
                        case 0xD0:
                        case 0xD1:
                        case 0xD2:
                        case 0xD3:
                        case 0xD4:
                        case 0xD5:
                        case 0xD6:
                        case 0xD7:

                            if (BD[pos + 1] < 0x80)
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opS[MN],
                                    Parameter = Mnemonic.adE[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xD0)
                                });
                            //Result = Result + Memnoic.opS[MN] + Memnoic.adE[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xD0);
                            else
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opS[MN],
                                    Parameter = Mnemonic.adA[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xD0)
                                });
                            //Result = Result + Memnoic.opS[MN] + Memnoic.adA[MN] + Convert.ToString(BD[pos + 1] - 0x80) + "." +
                            //         Convert.ToString(BD[pos] - 0xD0);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xD8:
                        case 0xD9:
                        case 0xDA:
                        case 0xDB:
                        case 0xDC:
                        case 0xDD:
                        case 0xDE:
                        case 0xDF:
                            if (BD[pos + 1] < 0x80)
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opZUW[MN],
                                    Parameter = Mnemonic.adE[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xD8)
                                });
                            //Result = Result + Memnoic.opZUW[MN] + Memnoic.adE[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xD8);
                            else
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opZUW[MN],
                                    Parameter = Mnemonic.adA[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xD8)
                                });
                            //Result = Result + Memnoic.opZUW[MN] + Memnoic.adA[MN] + Convert.ToString(BD[pos + 1] - 0x80) + "." +
                            //         Convert.ToString(BD[pos] - 0xD8);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xE0:
                        case 0xE1:
                        case 0xE2:
                        case 0xE3:
                        case 0xE4:
                        case 0xE5:
                        case 0xE6:
                        case 0xE7:
                            if (BD[pos + 1] < 0x80)
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opUN[MN],
                                    Parameter = Mnemonic.adE[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xE0)
                                });
                            //Result = Result + Memnoic.opUN[MN] + Memnoic.adE[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xE0);
                            else
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opU[MN],
                                    Parameter = Mnemonic.adA[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xE0)
                                });
                            //Result = Result + Memnoic.opUN[MN] + Memnoic.adA[MN] + Convert.ToString(BD[pos + 1] - 0x80) + "." +
                            //         Convert.ToString(BD[pos] - 0xE0);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xE8:
                        case 0xE9:
                        case 0xEA:
                        case 0xEB:
                        case 0xEC:
                        case 0xED:
                        case 0xEE:
                        case 0xEF:
                            if (BD[pos + 1] < 0x80)
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opON[MN],
                                    Parameter = Mnemonic.adE[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xE8)
                                });
                            //Result = Result + Memnoic.opON[MN] + Memnoic.adE[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xE8);
                            else
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opON[MN],
                                    Parameter = Mnemonic.adA[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xE8)
                                });
                            //Result = Result + Memnoic.opON[MN] + Memnoic.adA[MN] + Convert.ToString(BD[pos + 1] - 0x80) + "." +
                            //         Convert.ToString(BD[pos] - 0xE8);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xF0:
                        case 0xF1:
                        case 0xF2:
                        case 0xF3:
                        case 0xF4:
                        case 0xF5:
                        case 0xF6:
                        case 0xF7:
                            if (BD[pos + 1] < 0x80)
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opR[MN],
                                    Parameter = Mnemonic.adE[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xF0)
                                });
                            //Result = Result + Memnoic.opR[MN] + Memnoic.adE[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xF0);
                            else
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Mnemonic.opR[MN],
                                    Parameter = Mnemonic.adA[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xF0)
                                });
                            //Result = Result + Memnoic.opR[MN] + Memnoic.adA[MN] + Convert.ToString(BD[pos + 1] - 0x80) + "." +
                            //         Convert.ToString(BD[pos] - 0xF0);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xF8:
                            retVal.Add(new S7FunctionBlockRow()
                            {
                                Command = Mnemonic.opU[MN],
                                Parameter = Mnemonic.adT[MN] + Convert.ToString(BD[pos + 1])
                            });
                            //Result = Result + Memnoic.opU[MN] + Memnoic.adT[MN] + Convert.ToString(BD[pos + 1]);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xF9:
                            retVal.Add(new S7FunctionBlockRow()
                            {
                                Command = Mnemonic.opO[MN],
                                Parameter = Mnemonic.adT[MN] + Convert.ToString(BD[pos + 1])
                            });
                            //Result = Result + Memnoic.opO[MN] + Memnoic.adT[MN] + Convert.ToString(BD[pos + 1]);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xFB:
                            {
                                switch (BD[pos + 1])
                                {
                                    case 0x00:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opO[MN] });
                                            //Result = Result + Memnoic.opO[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x01:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opL[MN],
                                                Parameter = Mnemonic.adB[MN] + "[" + Mnemonic.adAR1[MN] + "," +
                                                    Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x02:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opL[MN],
                                                Parameter = Mnemonic.adW[MN] + "[" + Mnemonic.adAR1[MN] + "," +
                                                    Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });                                            
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x03:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opL[MN],
                                                Parameter = Mnemonic.adD[MN] + "[" + Mnemonic.adAR1[MN] + "," +
                                                    Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opL[MN] + Memnoic.adD[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x05:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opT[MN],
                                                Parameter = Mnemonic.adB[MN] + "[" + Mnemonic.adAR1[MN] + "," +
                                                    Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opT[MN] + Memnoic.adB[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x06:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opT[MN],
                                                Parameter = Mnemonic.adW[MN] + "[" + Mnemonic.adAR1[MN] + "," +
                                                    Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opT[MN] + Memnoic.adW[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x07:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opT[MN],
                                                Parameter = Mnemonic.adD[MN] + "[" + Mnemonic.adAR1[MN] + "," +
                                                    Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opT[MN] + Memnoic.adD[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x09:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opL[MN],
                                                Parameter = Mnemonic.adB[MN] + "[" + Mnemonic.adAR2[MN] + "," +
                                                    Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opL[MN] + Memnoic.adB[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x0A:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opL[MN],
                                                Parameter = Mnemonic.adW[MN] + "[" + Mnemonic.adAR2[MN] + "," +
                                                    Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opL[MN] + Memnoic.adW[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x0b:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opL[MN],
                                                Parameter = Mnemonic.adD[MN] + "[" + Mnemonic.adAR2[MN] + "," +
                                                    Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opL[MN] + Memnoic.adD[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x0D:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opT[MN],
                                                Parameter = Mnemonic.adB[MN] + "[" + Mnemonic.adAR2[MN] + "," +
                                                    Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opT[MN] + Memnoic.adB[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x0E:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opT[MN],
                                                Parameter = Mnemonic.adW[MN] + "[" + Mnemonic.adAR2[MN] + "," +
                                                    Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opT[MN] + Memnoic.adW[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x0F:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opT[MN],
                                                Parameter = Mnemonic.adD[MN] + "[" + Mnemonic.adAR2[MN] + "," +
                                                    Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opT[MN] + Memnoic.adD[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x10:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opU[MN],
                                                Parameter = "[" + Mnemonic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opU[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x11:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUN[MN],
                                                Parameter = "[" + Mnemonic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opUN[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x12:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opO[MN],
                                                Parameter = "[" + Mnemonic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opO[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x13:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opON[MN],
                                                Parameter = "[" + Mnemonic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opON[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x14:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opX[MN],
                                                Parameter = "[" + Mnemonic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opX[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x15:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXN[MN],
                                                Parameter = "[" + Mnemonic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opXN[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x18:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opU[MN],
                                                Parameter = "[" + Mnemonic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opU[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x19:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUN[MN],
                                                Parameter = "[" + Mnemonic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opUN[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x1A:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opO[MN],
                                                Parameter = "[" + Mnemonic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opO[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x1b:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opON[MN],
                                                Parameter = "[" + Mnemonic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opON[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x1C:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opX[MN],
                                                Parameter = "[" + Mnemonic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opX[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x1D:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXN[MN],
                                                Parameter = "[" + Mnemonic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opXN[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x20:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opS[MN],
                                                Parameter = "[" + Mnemonic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opS[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x21:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opR[MN],
                                                Parameter = "[" + Mnemonic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opR[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x22:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opZUW[MN],
                                                Parameter = "[" + Mnemonic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opZUW[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x24:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opFP[MN],
                                                Parameter = "[" + Mnemonic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opFP[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x25:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opFN[MN],
                                                Parameter = "[" + Mnemonic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opFN[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x28:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opS[MN],
                                                Parameter = "[" + Mnemonic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opS[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x29:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opR[MN],
                                                Parameter = "[" + Mnemonic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opR[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x2A:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opZUW[MN],
                                                Parameter = "[" + Mnemonic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opZUW[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x2C:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opFP[MN],
                                                Parameter = "[" + Mnemonic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opFP[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x2D:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opFN[MN],
                                                Parameter = "[" + Mnemonic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opFN[MN] + "[" + Memnoic.adAR2[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x38:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opAUF[MN],
                                                Parameter = Mnemonic.adDB[MN] + "[" + Mnemonic.adMW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]"
                                            });
                                            //Result = Result + Memnoic.opAUF[MN] + Memnoic.adDB[MN] + "[" + Memnoic.adMW[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3])) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x39:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opAUF[MN],
                                                Parameter = Mnemonic.adDI[MN] + "[" + Mnemonic.adMW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]"
                                            });
                                            //Result = Result + Memnoic.opAUF[MN] + Memnoic.adDI[MN] + "[" + Memnoic.adMW[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3])) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x3C:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opL[MN],
                                                Parameter = Mnemonic.adDBLG[MN]
                                            });
                                            //Result = Result + Memnoic.opL[MN] + Memnoic.adDBLG[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x3D:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opL[MN],
                                                Parameter = Mnemonic.adDILG[MN]
                                            });
                                            //Result = Result + Memnoic.opL[MN] + Memnoic.adDILG[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x48:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opAUF[MN],
                                                Parameter = Mnemonic.adDB[MN] + "[" + Mnemonic.adDBW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]"
                                            });
                                            //Result = Result + Memnoic.opAUF[MN] + Memnoic.adDB[MN] + "[" + Memnoic.adDBW[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3])) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x49:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opAUF[MN],
                                                Parameter = Mnemonic.adDI[MN] + "[" + Mnemonic.adDBW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]"
                                            });
                                            //Result = Result + Memnoic.opAUF[MN] + Memnoic.adDI[MN] + "[" + Memnoic.adDBW[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3])) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x4C:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opL[MN],
                                                Parameter = Mnemonic.adDBNO[MN]
                                            });
                                            //Result = Result + Memnoic.opL[MN] + Memnoic.adDBNO[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x4D:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opL[MN],
                                                Parameter = Mnemonic.adDINO[MN]
                                            });
                                            //Result = Result + Memnoic.opL[MN] + Memnoic.adDINO[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x58:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opAUF[MN],
                                                Parameter = Mnemonic.adDB[MN] + "[" + Mnemonic.adDIW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]"
                                            });
                                            //Result = Result + Memnoic.opAUF[MN] + Memnoic.adDB[MN] + "[" + Memnoic.adDIW[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3])) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x59:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opAUF[MN],
                                                Parameter = Mnemonic.adDI[MN] + "[" + Mnemonic.adDIW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]"
                                            });
                                            //Result = Result + Memnoic.opAUF[MN] + Memnoic.adDI[MN] + "[" + Memnoic.adDIW[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3])) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x68:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opAUF[MN],
                                                Parameter = Mnemonic.adDB[MN] + "[" + Mnemonic.adLW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]"
                                            });
                                            //Result = Result + Memnoic.opAUF[MN] + Memnoic.adDB[MN] + "[" + Memnoic.adLW[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3])) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x69:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opAUF[MN],
                                                Parameter = Mnemonic.adDI[MN] + "[" + Mnemonic.adLW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]"
                                            });
                                            //Result = Result + Memnoic.opAUF[MN] + Memnoic.adDI[MN] + "[" + Memnoic.adLW[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3])) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x70:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opUC[MN], Parameter = Mnemonic.adFC[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) });

                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5], BD[pos + 6], BD[pos + 7] };

                                            //Get Parameter Count 
                                            int paranz = (BD[pos + 7] / 2) - 1;
                                            List<string> tmplst = new List<string>();
                                            for (int n = 1; n <= paranz; n++)
                                            {
                                                tmplst.Add(Helper.GetFCPointer(BD[pos + 8], BD[pos + 9], BD[pos + 10], BD[pos + 11]));
                                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = Helper.CombineByteArray(((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7, new byte[] { BD[pos + 8], BD[pos + 9], BD[pos + 10], BD[pos + 11] });
                                                pos += 4;
                                            }

                                            byte[] backup = ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7;
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).ExtParameter = tmplst;
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = backup;

                                            pos += 8;
                                        }
                                        break;
                                    case 0x30:
                                    case 0x40:
                                    case 0x50:
                                    case 0x60:
                                        {
                                            string par = "";
                                            switch (BD[pos + 1] & 0xf0)
                                            {
                                                case 0x30:
                                                    par = Mnemonic.adMW[MN];
                                                    break;
                                                case 0x40:
                                                    par = Mnemonic.adDBW[MN];
                                                    break;
                                                case 0x50:
                                                    par = Mnemonic.adDIW[MN];
                                                    break;
                                                case 0x60:
                                                    par = Mnemonic.adLW[MN];
                                                    break;
                                            }
                                            retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opUC[MN], Parameter = Mnemonic.adFC[MN] + "[" + par + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]" });

                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5], BD[pos + 6], BD[pos + 7] };

                                            //Get Parameter Count 
                                            int paranz = (BD[pos + 7] / 2) - 1;
                                            List<string> tmplst = new List<string>();
                                            for (int n = 1; n <= paranz; n++)
                                            {
                                                tmplst.Add(Helper.GetFCPointer(BD[pos + 8], BD[pos + 9], BD[pos + 10], BD[pos + 11]));
                                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = Helper.CombineByteArray(((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7, new byte[] { BD[pos + 8], BD[pos + 9], BD[pos + 10], BD[pos + 11] });
                                                pos += 4;
                                            }

                                            byte[] backup = ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7;
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).ExtParameter = tmplst;
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = backup;

                                            pos += 8;
                                        }
                                        break;
                                    case 0x32:
                                    case 0x42:
                                    case 0x52:
                                    case 0x62:
                                        {
                                            string par = "";
                                            switch (BD[pos + 1] & 0xf0)
                                            {
                                                case 0x30:
                                                    par = Mnemonic.adMW[MN];
                                                    break;
                                                case 0x40:
                                                    par = Mnemonic.adDBW[MN];
                                                    break;
                                                case 0x60:
                                                    par = Mnemonic.adLW[MN];
                                                    break;
                                                case 0x50:
                                                    par = Mnemonic.adDIW[MN];
                                                    break;
                                            }
                                            retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opUC[MN], Parameter = Mnemonic.adFB[MN] + "[" + par + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]" });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;

                                    case 0x41:
                                    case 0x51:
                                    case 0x61:
                                        {
                                            string par = "";
                                            switch (BD[pos + 1])
                                            {
                                                case 0x61:
                                                    par = Mnemonic.adLW[MN];
                                                    break;
                                                case 0x41:
                                                    par = Mnemonic.adDBW[MN];
                                                    break;
                                                case 0x51:
                                                    par = Mnemonic.adDIW[MN];
                                                    break;
                                            }
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opCC[MN],
                                                Parameter = Mnemonic.adFC[MN] + "[" + par +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]"
                                            });

                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5], BD[pos + 6], BD[pos + 7] };
                                            pos += 8;
                                        }
                                        break;

                                    case 0x43:
                                    case 0x53:
                                    case 0x63:
                                        {
                                            string par = "";
                                            switch (BD[pos + 1] & 0xf0)
                                            {
                                                case 0x60:
                                                    par = Mnemonic.adLW[MN];
                                                    break;
                                                case 0x40:
                                                    par = Mnemonic.adDBW[MN];
                                                    break;
                                                case 0x50:
                                                    par = Mnemonic.adDIW[MN];
                                                    break;
                                            }
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opCC[MN],
                                                Parameter = Mnemonic.adFB[MN] + "[" + par +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]"
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x71:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opCC[MN],
                                                Parameter = Mnemonic.adFC[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5], BD[pos + 6], BD[pos + 7] };
                                            pos += 8;
                                        }
                                        break;
                                    case 0x72:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opUC[MN], Parameter = Mnemonic.adFB[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x73:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opCC[MN],
                                                Parameter = Mnemonic.adFB[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });

                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x74:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opUC[MN], Parameter = Mnemonic.adSFC[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) });

                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5], BD[pos + 6], BD[pos + 7] };

                                            //Get Parameter Count 
                                            int paranz = (BD[pos + 7] / 2) - 1;
                                            List<string> tmplst = new List<string>();
                                            for (int n = 1; n <= paranz; n++)
                                            {
                                                tmplst.Add(Helper.GetFCPointer(BD[pos + 8], BD[pos + 9], BD[pos + 10], BD[pos + 11]));
                                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = Helper.CombineByteArray(((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7, new byte[] { BD[pos + 8], BD[pos + 9], BD[pos + 10], BD[pos + 11] });
                                                pos += 4;
                                            }

                                            byte[] backup = ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7;
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).ExtParameter = tmplst;
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = backup;

                                            pos += 8;
                                        }
                                        break;
                                    case 0x76:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opUC[MN], Parameter = Mnemonic.adSFB[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x78:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opAUF[MN],
                                                Parameter = Mnemonic.adDB[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opAUF[MN] + Memnoic.adDB[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x79:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opAUF[MN],
                                                Parameter = Mnemonic.adDI[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opAUF[MN] + Memnoic.adDI[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x7C:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Mnemonic.opTDB[MN] });
                                            //Result = Result + Memnoic.opTDB[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x80:
                                    case 0xA0:
                                    case 0xB0:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opU[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x81:
                                    case 0xA1:
                                    case 0xB1:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opUN[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x82:
                                    case 0xA2:
                                    case 0xB2:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opO[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x83:
                                    case 0xA3:
                                    case 0xB3:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opON[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x84:
                                    case 0xA4:
                                    case 0xB4:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opX[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x85:
                                    case 0xA5:
                                    case 0xB5:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opXN[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x90:
                                    case 0xBB:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opS[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x91:
                                    case 0xAF:
                                    case 0xBF:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opR[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x92:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opZUW[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x94:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opFP[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x95:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opFN[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xA6:
                                    case 0xb6:
                                    case 0xc1:
                                    case 0xc2:
                                    case 0xc3:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opL[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                            {
                                                if (BD[pos + 1] == 0xc2)
                                                {
                                                    par = "IN" + ParaList.Count;
                                                    if (blockInterface != null)
                                                    {
                                                        var inInterface = blockInterface.Children[0];
                                                        ((S7DataRow)inInterface).Add(new S7DataRow("IN" + ParaList.Count, S7DataRowType.INT, inInterface.PlcBlock));
                                                    }
                                                    ParaList.Add(par);
                                                    par = "#" + par;
                                                }
                                                else
                                                    par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            }
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xA8:
                                    case 0xb8:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opFR[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xA9:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opLC[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;

                                        }
                                        break;
                                    case 0xAA:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opSA[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xAB:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opSV[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xAC:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opSE[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xAD:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opSS[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xAE:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opSI[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xBA:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opZR[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xBD:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opZV[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xC5:
                                    case 0xC6:
                                    case 0xC7:
                                        //case 0xCB:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opT[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xCA:
                                    case 0xCB:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opL[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "P##" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xD0:
                                    case 0xD2:  //UC unconditional Call without parameters as in literal UC FBxxx
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opUC[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });

                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5], BD[pos + 6], BD[pos + 7] };

                                            //This OpCode does not have parameter asignments
                                            //Get Parameter Count 
                                            //int paranz = (BD[pos + 7] / 2) - 1;
                                            //List<string> tmplst = new List<string>();
                                            //for (int n = 1; n <= paranz; n++)
                                            //{
                                            //    tmplst.Add(Helper.GetFCPointer(BD[pos + 8], BD[pos + 9], BD[pos + 10], BD[pos + 11]));
                                            //    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = Helper.CombineByteArray(((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7, new byte[] { BD[pos + 8], BD[pos + 9], BD[pos + 10], BD[pos + 11] });
                                            //    pos += 4;
                                            //}

                                            //byte[] backup = ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7;
                                            //((S7FunctionBlockRow)retVal[retVal.Count - 1]).ExtParameter = tmplst;
                                            //((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = backup;

                                            pos += 8;
                                        }
                                        break;
                                    case 0xD8:
                                    case 0xD9:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Mnemonic.opAUF[MN];
                                            if (ParaList.Count > (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;                                    
                                    case 0xE0:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opU[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opU[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xE1:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUN[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opUN[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xE2:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opO[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opO[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xE3:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opON[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opON[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xE4:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opX[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opX[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xE5:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXN[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opXN[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xE6:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opL[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opL[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xE8:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opFR[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opFR[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xE9:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opLC[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opLC[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xEA:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSA[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opSA[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xEB:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSV[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opSV[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xEC:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSE[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opSE[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xED:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSS[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opSS[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xEE:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSI[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opSI[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xEF:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opR[MN],
                                                Parameter = Mnemonic.adT[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opR[MN] + Memnoic.adT[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xF0:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opU[MN],
                                                Parameter = Mnemonic.adZ[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opU[MN] + Memnoic.adZ[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xF1:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUN[MN],
                                                Parameter = Mnemonic.adZ[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opUN[MN] + Memnoic.adZ[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xF2:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opO[MN],
                                                Parameter = Mnemonic.adZ[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opO[MN] + Memnoic.adZ[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xF3:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opON[MN],
                                                Parameter = Mnemonic.adZ[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opON[MN] + Memnoic.adZ[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xF4:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opX[MN],
                                                Parameter = Mnemonic.adZ[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opX[MN] + Memnoic.adZ[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xF5:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXN[MN],
                                                Parameter = Mnemonic.adZ[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opXN[MN] + Memnoic.adZ[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xF8:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opFR[MN],
                                                Parameter = Mnemonic.adZ[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opFR[MN] + Memnoic.adZ[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xFA:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opZR[MN],
                                                Parameter = Mnemonic.adZ[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opZR[MN] + Memnoic.adZ[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xFB:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opS[MN],
                                                Parameter = Mnemonic.adZ[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opS[MN] + Memnoic.adZ[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xfd:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opZV[MN],
                                                Parameter = Mnemonic.adZ[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opZV[MN] + Memnoic.adZ[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xFF:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opR[MN],
                                                Parameter = Mnemonic.adZ[MN] +
                                                    Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opR[MN] + Memnoic.adZ[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    default:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "errd" });
                                        ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                        pos += 4;
                                        break;
                                }
                            }
                            break;
                        case 0xFC:
                            retVal.Add(new S7FunctionBlockRow()
                            {
                                Command = Mnemonic.opUN[MN],
                                Parameter = Mnemonic.adT[MN] + Convert.ToString(BD[pos + 1])
                            });
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xFD:
                            retVal.Add(new S7FunctionBlockRow()
                            {
                                Command = Mnemonic.opON[MN],
                                Parameter = Mnemonic.adT[MN] + Convert.ToString(BD[pos + 1])
                            });
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xFE:
                            {
                                switch (BD[pos + 1])
                                {
                                    case 0x01:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opLAR1[MN],
                                                Parameter = Mnemonic.adAR2[MN]
                                            });
                                            //Result = Result + Memnoic.opLAR1[MN] + Memnoic.adAR2[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x02:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opPAR1[MN],
                                                Parameter = Helper.GetShortPointer(BD[pos + 2], BD[pos + 3])
                                            });
                                            //Result = Result + Memnoic.opPAR1[MN] +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]);
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x03:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                                           {
                                                               Command = Mnemonic.opLAR1[MN],
                                                               Parameter = Helper.GetPointer(BD, pos + 2, MN)
                                                           });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };
                                            pos += 6;
                                        }
                                        break;
                                    case 0x04:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opLAR1[MN]
                                            });
                                            //Result = Result + Memnoic.opLAR1[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x05:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opTAR1[MN]
                                            });
                                            //Result = Result + Memnoic.opTAR1[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x06:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opPAR1[MN]
                                            });
                                            //Result = Result + Memnoic.opPAR1[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x08:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opTAR[MN]
                                            });
                                            //Result = Result + Memnoic.opTAR[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x09:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opTAR1[MN],
                                                Parameter = Mnemonic.adAR2[MN]
                                            });
                                            //Result = Result + Memnoic.opTAR1[MN] + Memnoic.adAR2[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x0A:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opPAR2[MN],
                                                Parameter = Helper.GetShortPointer(BD[pos + 2], BD[pos + 3])
                                            });
                                            //Result = Result + Memnoic.opPAR2[MN] +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]);
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x0b:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opLAR2[MN],
                                                Parameter = Helper.GetPointer(BD, pos + 2, MN)
                                            });
                                            //Result = Result + Memnoic.opLAR2[MN] +
                                            //         Helper.GetPointer(BD[pos + 2], BD[pos + 3], BD[pos + 4],
                                            //                           BD[pos + 5]);
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };
                                            pos += 6;
                                        }
                                        break;
                                    case 0x0C:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opLAR2[MN]
                                            });
                                            //Result = Result + Memnoic.opLAR2[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x0D:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opTAR2[MN]
                                            });
                                            //Result = Result + Memnoic.opTAR2[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x0E:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opPAR2[MN]
                                            });
                                            //Result = Result + Memnoic.opPAR2[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x33:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opLAR1[MN],
                                                Parameter = Mnemonic.adMD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x37:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opTAR1[MN],
                                                Parameter = Mnemonic.adMD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opTAR1[MN] + Memnoic.adMD[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x3b:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opLAR2[MN],
                                                Parameter = Mnemonic.adMD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opLAR2[MN] + Memnoic.adMD[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x3F:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opTAR2[MN],
                                                Parameter = Mnemonic.adMD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opTAR2[MN] + Memnoic.adMD[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x43:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opLAR1[MN],
                                                Parameter = Mnemonic.adDBD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opLAR1[MN] + Memnoic.adDBD[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x47:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opTAR1[MN],
                                                Parameter = Mnemonic.adDBD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opTAR1[MN] + Memnoic.adDBD[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x4b:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opLAR2[MN],
                                                Parameter = Mnemonic.adDBD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opLAR2[MN] + Memnoic.adDBD[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x4F:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opTAR2[MN],
                                                Parameter = Mnemonic.adDBD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opTAR2[MN] + Memnoic.adDBD[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x53:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opLAR1[MN],
                                                Parameter = Mnemonic.adDID[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opLAR1[MN] + Memnoic.adDID[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x57:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opTAR1[MN],
                                                Parameter = Mnemonic.adDID[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opTAR1[MN] + Memnoic.adDID[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x5b:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opLAR2[MN],
                                                Parameter = Mnemonic.adDID[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opLAR2[MN] + Memnoic.adDID[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x5F:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opTAR2[MN],
                                                Parameter = Mnemonic.adDID[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opTAR2[MN] + Memnoic.adDID[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x63:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opLAR1[MN],
                                                Parameter = Mnemonic.adLD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opLAR1[MN] + Memnoic.adLD[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x67:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opTAR1[MN],
                                                Parameter = Mnemonic.adLD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opTAR1[MN] + Memnoic.adLD[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x6b:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opLAR2[MN],
                                                Parameter = Mnemonic.adLD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opLAR2[MN] + Memnoic.adLD[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x6F:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opTAR2[MN],
                                                Parameter = Mnemonic.adLD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            //Result = Result + Memnoic.opTAR2[MN] + Memnoic.adLD[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3]));
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xC0:
                                    case 0xD0:
                                    case 0xE0:
                                        retVal.Add(new S7FunctionBlockRow()
                                                    {
                                                        Command = Mnemonic.opSRD[MN],
                                                        Parameter = Convert.ToString(BD[pos + 1] - 0xC0)
                                                    });
                                        //Result = Result + Memnoic.opSRD[MN] + Convert.ToString(BD[pos + 1] - 0xC0);
                                        ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                        pos += 2;
                                        break;
                                    default:
                                        switch (BD[pos + 1] & 0xf0)
                                        {
                                            case 0xC0:
                                            case 0xD0:
                                            case 0xE0:
                                                retVal.Add(new S7FunctionBlockRow()
                                                               {
                                                                   Command = Mnemonic.opSRD[MN],
                                                                   Parameter = Convert.ToString(BD[pos + 1] - 0xC0)
                                                               });
                                                //Result = Result + Memnoic.opSRD[MN] + Convert.ToString(BD[pos + 1] - 0xC0);
                                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                                pos += 2;
                                                break;
                                            default:
                                                retVal.Add(new S7FunctionBlockRow() { Command = "err 99" });
                                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                                pos += 2;
                                                break;
                                        }
                                        break;
                                }
                            }
                            break;
                        case 0xFF:
                            {
                                switch (BD[pos + 1])
                                {
                                    case 0x00:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opU[MN],
                                                Parameter = Mnemonic.adOS[MN]
                                            });
                                            //Result = Result + Memnoic.opU[MN] + Memnoic.adOS[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x01:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUN[MN],
                                                Parameter = Mnemonic.adOS[MN]
                                            });
                                            //Result = Result + Memnoic.opUN[MN] + Memnoic.adOS[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x02:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opO[MN],
                                                Parameter = Mnemonic.adOS[MN]
                                            });
                                            //Result = Result + Memnoic.opO[MN] + Memnoic.adOS[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x03:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opON[MN],
                                                Parameter = Mnemonic.adOS[MN]
                                            });
                                            //Result = Result + Memnoic.opON[MN] + Memnoic.adOS[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x04:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opX[MN],
                                                Parameter = Mnemonic.adOS[MN]
                                            });
                                            //Result = Result + Memnoic.opX[MN] + Memnoic.adOS[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x05:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXN[MN],
                                                Parameter = Mnemonic.adOS[MN]
                                            });
                                            //Result = Result + Memnoic.opXN[MN] + Memnoic.adOS[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x08:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                                           {
                                                               Command = Mnemonic.opSPS[MN],
                                                               JumpWidth = libnodave.getS16from(BD, pos + 2)
                                                           });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x10:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opU[MN],
                                                Parameter = Mnemonic.adOV[MN]
                                            });
                                            //Result = Result + Memnoic.opU[MN] + Memnoic.adOV[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x11:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUN[MN],
                                                Parameter = Mnemonic.adOV[MN]
                                            });
                                            //Result = Result + Memnoic.opUN[MN] + Memnoic.adOV[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x12:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opO[MN],
                                                Parameter = Mnemonic.adOV[MN]
                                            });
                                            //Result = Result + Memnoic.opO[MN] + Memnoic.adOV[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x13:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opON[MN],
                                                Parameter = Mnemonic.adOV[MN]
                                            });
                                            //Result = Result + Memnoic.opON[MN] + Memnoic.adOV[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x14:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opX[MN],
                                                Parameter = Mnemonic.adOV[MN]
                                            });
                                            //Result = Result + Memnoic.opX[MN] + Memnoic.adOV[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x15:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXN[MN],
                                                Parameter = Mnemonic.adOV[MN]
                                            });
                                            //Result = Result + Memnoic.opXN[MN] + Memnoic.adOV[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x18:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSPO[MN],
                                                JumpWidth = libnodave.getS16from(BD, pos + 2)
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x20:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opU[MN],
                                                Parameter = ">0"
                                            });
                                            //Result = Result + Memnoic.opU[MN] + ">0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x21:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUN[MN],
                                                Parameter = ">0"
                                            });
                                            //Result = Result + Memnoic.opUN[MN] + ">0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x22:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opO[MN],
                                                Parameter = ">0"
                                            });
                                            //Result = Result + Memnoic.opO[MN] + ">0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x23:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opON[MN],
                                                Parameter = ">0"
                                            });
                                            //Result = Result + Memnoic.opON[MN] + ">0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x24:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opX[MN],
                                                Parameter = ">0"
                                            });
                                            //Result = Result + Memnoic.opX[MN] + ">0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x25:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXN[MN],
                                                Parameter = ">0"
                                            });
                                            //Result = Result + Memnoic.opXN[MN] + ">0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x28:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSPP[MN],
                                                JumpWidth = libnodave.getS16from(BD, pos + 2)
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x40:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opU[MN],
                                                Parameter = "<0"
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x41:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUN[MN],
                                                Parameter = "<0"
                                            });
                                            //Result = Result + Memnoic.opUN[MN] + "<0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x42:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opO[MN],
                                                Parameter = "<0"
                                            });
                                            //Result = Result + Memnoic.opO[MN] + "<0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x43:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opON[MN],
                                                Parameter = "<0"
                                            });
                                            //Result = Result + Memnoic.opON[MN] + "<0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x44:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opX[MN],
                                                Parameter = "<0"
                                            });
                                            //Result = Result + Memnoic.opX[MN] + "<0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x45:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXN[MN],
                                                Parameter = "<0"
                                            });
                                            //Result = Result + Memnoic.opXN[MN] + "<0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x48:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSPM[MN],
                                                JumpWidth = libnodave.getS16from(BD, pos + 2)
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x50:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opU[MN],
                                                Parameter = Mnemonic.adUO[MN]
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x51:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUN[MN],
                                                Parameter = Mnemonic.adUO[MN]
                                            });
                                            //Result = Result + Memnoic.opUN[MN] + Memnoic.adUO[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x52:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opO[MN],
                                                Parameter = Mnemonic.adUO[MN]
                                            });
                                            //Result = Result + Memnoic.opO[MN] + Memnoic.adUO[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x53:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opON[MN],
                                                Parameter = Mnemonic.adUO[MN]
                                            });
                                            //Result = Result + Memnoic.opON[MN] + Memnoic.adUO[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x54:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opX[MN],
                                                Parameter = Mnemonic.adUO[MN]
                                            });
                                            //Result = Result + Memnoic.opX[MN] + Memnoic.adUO[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x55:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXN[MN],
                                                Parameter = Mnemonic.adUO[MN]
                                            });
                                            //Result = Result + Memnoic.opXN[MN] + Memnoic.adUO[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x58:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSPU[MN],
                                                JumpWidth = libnodave.getS16from(BD, pos + 2)
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x60:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opU[MN],
                                                Parameter = "<>0"
                                            });
                                            //Result = Result + Memnoic.opU[MN] + "<>0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x61:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUN[MN],
                                                Parameter = "<>0"
                                            });
                                            //Result = Result + Memnoic.opUN[MN] + "<>0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x62:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opO[MN],
                                                Parameter = "<>0"
                                            });
                                            //Result = Result + Memnoic.opO[MN] + "<>0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x63:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opON[MN],
                                                Parameter = "<>0"
                                            });
                                            //Result = Result + Memnoic.opON[MN] + "<>0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x64:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opX[MN],
                                                Parameter = "<>0"
                                            });
                                            //Result = Result + Memnoic.opX[MN] + "<>0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x65:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXN[MN],
                                                Parameter = "<>0"
                                            });
                                            //Result = Result + Memnoic.opXN[MN] + "<>0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x68:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSPN[MN],
                                                JumpWidth = libnodave.getS16from(BD, pos + 2)
                                            });
                                            //Result = Result + Memnoic.opSPN[MN] + "(Springe " +
                                            //         Helper.JumpStr(Helper.GetInt(BD[pos + 2], BD[pos + 3])) + " W)";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x78:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSPBIN[MN],
                                                JumpWidth = libnodave.getS16from(BD, pos + 2)
                                            });
                                            //Result = Result + Memnoic.opSPBIN[MN] + "(Springe " +
                                            //         Helper.JumpStr(Helper.GetInt(BD[pos + 2], BD[pos + 3])) + " W)";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x80:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opU[MN],
                                                Parameter = "==0"
                                            });
                                            //Result = Result + Memnoic.opU[MN] + "==0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x81:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUN[MN],
                                                Parameter = "==0"
                                            });
                                            //Result = Result + Memnoic.opUN[MN] + "==0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x82:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opO[MN],
                                                Parameter = "==0"
                                            });
                                            //Result = Result + Memnoic.opO[MN] + "==0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x83:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opON[MN],
                                                Parameter = "==0"
                                            });
                                            //Result = Result + Memnoic.opON[MN] + "==0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x84:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opX[MN],
                                                Parameter = "==0"
                                            });
                                            //Result = Result + Memnoic.opX[MN] + "==0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x85:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXN[MN],
                                                Parameter = "==0"
                                            });
                                            //Result = Result + Memnoic.opXN[MN] + "==0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x88:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSPZ[MN],
                                                JumpWidth = libnodave.getS16from(BD, pos + 2)
                                            });
                                            //Result = Result + Memnoic.opSPZ[MN] + "(Springe " +
                                            //         Helper.JumpStr(Helper.GetInt(BD[pos + 2], BD[pos + 3])) + " W)";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x98:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSPBNB[MN],
                                                JumpWidth = libnodave.getS16from(BD, pos + 2)
                                            });
                                            //Result = Result + Memnoic.opSPBNB[MN] + "(Springe " +
                                            //         Helper.JumpStr(Helper.GetInt(BD[pos + 2], BD[pos + 3])) + " W)";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xA0:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opU[MN],
                                                Parameter = ">=0"
                                            });
                                            //Result = Result + Memnoic.opU[MN] + ">=0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xA1:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUN[MN],
                                                Parameter = ">=0"
                                            });
                                            //Result = Result + Memnoic.opUN[MN] + ">=0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xA2:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opO[MN],
                                                Parameter = ">=0"
                                            });
                                            //Result = Result + Memnoic.opO[MN] + ">=0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xA3:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opON[MN],
                                                Parameter = ">=0"
                                            });
                                            //Result = Result + Memnoic.opON[MN] + ">=0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xA4:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opX[MN],
                                                Parameter = ">=0"
                                            });
                                            //Result = Result + Memnoic.opX[MN] + ">=0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xA5:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXN[MN],
                                                Parameter = ">=0"
                                            });
                                            //Result = Result + Memnoic.opXN[MN] + ">=0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xA8:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSPPZ[MN],
                                                JumpWidth = libnodave.getS16from(BD, pos + 2)
                                            });
                                            //Result = Result + Memnoic.opSPPZ[MN] + "(Springe " +
                                            //         Helper.JumpStr(Helper.GetInt(BD[pos + 2], BD[pos + 3])) + " W)";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xB8:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSPBN[MN],
                                                JumpWidth = libnodave.getS16from(BD, pos + 2)
                                            });
                                            //Result = Result + Memnoic.opSPBN[MN] + "(Springe " +
                                            //         Helper.JumpStr(Helper.GetInt(BD[pos + 2], BD[pos + 3])) + " W)";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xC0:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opU[MN],
                                                Parameter = "<=0"
                                            });
                                            //Result = Result + Memnoic.opU[MN] + "<=0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xC1:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUN[MN],
                                                Parameter = "<=0"
                                            });
                                            //Result = Result + Memnoic.opUN[MN] + "<=0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xC2:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opO[MN],
                                                Parameter = "<=0"
                                            });
                                            //Result = Result + Memnoic.opO[MN] + "<=0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xC3:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opON[MN],
                                                Parameter = "<=0"
                                            });
                                            //Result = Result + Memnoic.opON[MN] + "<=0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xC4:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opX[MN],
                                                Parameter = "<=0"
                                            });
                                            //Result = Result + Memnoic.opX[MN] + "<=0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xC5:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXN[MN],
                                                Parameter = "<=0"
                                            });
                                            //Result = Result + Memnoic.opXN[MN] + "<=0";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xC8:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSPMZ[MN],
                                                JumpWidth = libnodave.getS16from(BD, pos + 2)
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xD8:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSPBB[MN],
                                                JumpWidth = libnodave.getS16from(BD, pos + 2)
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xE0:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opU[MN],
                                                Parameter = Mnemonic.adBIE[MN]
                                            });
                                            //Result = Result + Memnoic.opU[MN] + Memnoic.adBIE[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xE1:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUN[MN],
                                                Parameter = Mnemonic.adBIE[MN]
                                            });
                                            //Result = Result + Memnoic.opUN[MN] + Memnoic.adBIE[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xE2:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opO[MN],
                                                Parameter = Mnemonic.adBIE[MN]
                                            });
                                            //Result = Result + Memnoic.opO[MN] + Memnoic.adBIE[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xE3:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opON[MN],
                                                Parameter = Mnemonic.adBIE[MN]
                                            });
                                            //Result = Result + Memnoic.opON[MN] + Memnoic.adBIE[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xE4:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opX[MN],
                                                Parameter = Mnemonic.adBIE[MN]
                                            });
                                            //Result = Result + Memnoic.opX[MN] + Memnoic.adBIE[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xE5:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXN[MN],
                                                Parameter = Mnemonic.adBIE[MN]
                                            });
                                            //Result = Result + Memnoic.opXN[MN] + Memnoic.adBIE[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xE8:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSPBI[MN],
                                                JumpWidth = libnodave.getS16from(BD, pos + 2)
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xF8:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opSPB[MN],
                                                JumpWidth = libnodave.getS16from(BD, pos + 2)
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xF1:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opUNO[MN],
                                            });
                                            //Result = Result + Memnoic.opUNO[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xF3:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opONO[MN],
                                            });
                                            //Result = Result + Memnoic.opONO[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xF4:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXO[MN],
                                            });
                                            //Result = Result + Memnoic.opXO[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xF5:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opXNO[MN],
                                            });
                                            //Result = Result + Memnoic.opXNO[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0xFF:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Mnemonic.opNOP[MN],
                                                Parameter = "1"
                                            });
                                            //Result = Result + Memnoic.opNOP[MN] + "1";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    default:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "errf" });
                                        ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                        pos += 4;
                                        break;
                                }
                            }
                            break;
                        default:
                            retVal.Add(new S7FunctionBlockRow() { Command = "err7" });
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                    }
                //Result = Result + "\r\n";
            }


            if (Networks != null)
                NetWork.NetworkCheck(ref Networks, ref retVal, ref counter, oldpos, pos, ref NWNr);


            //Go throug retval and add Symbols...
            /*if (prjBlkFld != null)
            {
                if (prjBlkFld.SymbolTable != null)
                {
                    foreach (S7FunctionBlockRow awlRow in retVal)
                    {
                        string para = awlRow.Parameter.Replace(" ", "");

                        awlRow.SymbolTableEntry = prjBlkFld.SymbolTable.GetEntryFromOperand(para);
                    }
                }
            }*/

            //Go throug retval and add Symbols...
            {
                foreach (S7FunctionBlockRow awlRow in retVal)
                {
                    awlRow.Parent = block;
                }
            }

            /*
            //Use the Jump-Marks from the Step7 project....
            string[] jumpNames = null;
            int cntJ = 0;
            if (JumpMarks!=null)
            {
                string aa = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(JumpMarks);
                int anzJ = JumpMarks[4] + JumpMarks[5]*0x100;
                jumpNames = new string[anzJ];

                for (int n = 6; n < JumpMarks.Length; n++)
                {
                    if (JumpMarks[n] != 0 && cntJ < anzJ)
                    {
                        if (jumpNames[cntJ] == null)
                            jumpNames[cntJ] = "";
                        jumpNames[cntJ] = jumpNames[cntJ] + (char) JumpMarks[n];
                    }
                    else
                        cntJ++;
                }
                cntJ = 0;                
            }
            */

            //Build the Jumps: Create a List with the Addresses, Look for Jumps, Add Jump Marks
            int JumpCount = 0;
            int akBytePos = 0;
            Dictionary<int, S7FunctionBlockRow> ByteAdressNumerPLCFunctionBlocks = new Dictionary<int, S7FunctionBlockRow>();
            foreach (S7FunctionBlockRow tmp in retVal)
            {
                if (tmp.ByteSize > 0)
                {
                    ByteAdressNumerPLCFunctionBlocks.Add(akBytePos, tmp);
                    akBytePos += tmp.ByteSize;
                }
            }


            akBytePos = 0;
            foreach (S7FunctionBlockRow tmp in retVal)
            {
                if (Helper.IsJump(tmp, MN))
                {
                    int jmpBytePos = 0;

                    if (tmp.Command == Mnemonic.opSPL[MN])
                        jmpBytePos = akBytePos + ((tmp.JumpWidth + 1) * 4);
                    else
                        jmpBytePos = akBytePos + (tmp.JumpWidth * 2);

                    if (ByteAdressNumerPLCFunctionBlocks.ContainsKey(jmpBytePos))
                    {
                        var target = ByteAdressNumerPLCFunctionBlocks[jmpBytePos];
                        if (target.Label == "")
                        {
                            target.Label = "M" + JumpCount.ToString().PadLeft(3, '0');
                            JumpCount++;
                        }

                        //Backup the MC7 Code, because the MC7 Code is always deleted when the command or parameter changes!
                        byte[] backup = tmp.MC7;
                        tmp.Parameter = target.Label;
                        tmp.MC7 = backup;
                    }
                    else
                    {
                        byte[] backup = tmp.MC7;
                        tmp.Parameter = "Error! JumpWidth :" + tmp.JumpWidth;
                        tmp.MC7 = backup;
                    }


                }
                akBytePos += tmp.ByteSize;
            }
            //End Building of the Jumps.*/

            return retVal;
        }
    }
}
