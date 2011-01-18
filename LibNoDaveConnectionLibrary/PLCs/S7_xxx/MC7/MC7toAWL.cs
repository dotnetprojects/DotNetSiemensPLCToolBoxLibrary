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
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;


//Todo: Finish AWL to MC7
namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    internal static class MC7toAWL
    {
        internal static List<FunctionBlockRow> GetAWL(int Start, int Count, int MN, byte[] BD, int[] Networks, List<string> ParaList, S7ProgrammFolder prjBlkFld)
        {
            var retVal = new List<FunctionBlockRow>();

            bool CombineDBOpenAndCommand = true; // false; //If DB Open and Acess should be One AWL Line (like in Step 7). This should be a Parameter.

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
                                       Command = Memnoic.opNOP[MN],
                                       Parameter = "0"
                                   });

                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                    pos += 2;
                }
                else if (BD[pos] == 0x01 && BD[pos + 1] == 0x00)
                {
                    retVal.Add(new S7FunctionBlockRow() {Command = Memnoic.opINVI[MN]});
                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] {BD[pos], BD[pos + 1]};
                    pos += 2;
                }
                else if (BD[pos] == 0x05 && BD[pos + 1] == 0x00)
                {
                    retVal.Add(new S7FunctionBlockRow() {Command = Memnoic.opBEB[MN]});
                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] {BD[pos], BD[pos + 1]};
                    pos += 2;
                }
                else if (BD[pos] == 0x09 && BD[pos + 1] == 0x00)
                {
                    retVal.Add(new S7FunctionBlockRow() {Command = Memnoic.opNEGI[MN]});
                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] {BD[pos], BD[pos + 1]};
                    pos += 2;
                }
                else if (BD[pos] == 0x49 && BD[pos + 1] == 0x00)
                {
                    retVal.Add(new S7FunctionBlockRow() {Command = Memnoic.opOW[MN]});
                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] {BD[pos], BD[pos + 1]};
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
                            curr_op = HighByte < 0x90 ? Memnoic.opU[MN] : Memnoic.opUN[MN];
                            break;
                        case (0x01):
                            curr_op = HighByte < 0x90 ? Memnoic.opO[MN] : Memnoic.opON[MN];
                            break;
                        case (0x05):
                            curr_op = HighByte < 0x90 ? Memnoic.opX[MN] : Memnoic.opXN[MN];
                            break;
                        case (0x09):
                            curr_op = HighByte < 0x90 ? Memnoic.opS[MN] : Memnoic.opR[MN];
                            break;
                        case (0x49):
                            curr_op = HighByte < 0x90 ? Memnoic.opFP[MN] : Memnoic.opFN[MN];
                            break;
                    }

                    string par = "";
                    byte[] DBByte = null;

                    switch (HighByte)
                    {
                        case (0x10):
                        case (0x90):
                            curr_ad = Memnoic.adE[MN];
                            break;
                        case (0x20):
                        case (0xA0):
                            curr_ad = Memnoic.adA[MN];
                            break;
                        case (0x30):
                        case (0xB0):
                            curr_ad = Memnoic.adM[MN];
                            break;
                        case (0x40):
                        case (0xC0):
                            if (retVal[retVal.Count - 1].Command == Memnoic.opAUF[MN] && !((S7FunctionBlockRow)retVal[retVal.Count - 1]).Parameter.Contains("[") && CombineDBOpenAndCommand)
                            {
                                DBByte = ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7;
                                par = ((S7FunctionBlockRow)retVal[retVal.Count - 1]).Parameter + ".";
                                retVal.RemoveAt(retVal.Count - 1);
                            }
                            curr_ad = Memnoic.adDBX[MN];
                            break;
                        case (0x50):
                        case (0xD0):
                            curr_ad = Memnoic.adDIX[MN];
                            break;
                        case (0x60):
                        case (0xE0):
                            curr_ad = Memnoic.adL[MN];
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
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opNOP[MN], Parameter = "0" });
                                        //Result = Result + Memnoic.opNOP[MN] + "0";
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
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opL[MN], Parameter = Memnoic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x04:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opFR[MN], Parameter = Memnoic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x0A:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opL[MN], Parameter = Memnoic.adMB[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x0B:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opT[MN], Parameter = Memnoic.adMB[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x0C:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opLC[MN], Parameter = Memnoic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x10:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opBLD[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x11:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opINC[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x12:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opL[MN], Parameter = Memnoic.adMW[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x13:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opT[MN], Parameter = Memnoic.adMW[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x14:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSA[MN], Parameter = Memnoic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x19:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opDEC[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x1A:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opL[MN], Parameter = Memnoic.adMD[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x1b:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opT[MN], Parameter = Memnoic.adMD[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x1C:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSV[MN], Parameter = Memnoic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x1D:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opCC[MN], Parameter = Memnoic.adFC[MN] + Convert.ToString(BD[pos + 1]) });
                                //Result = Result + Memnoic.opCC[MN] + Memnoic.adFC[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };
                                pos += 6;
                            }
                            break;
                        case 0x20:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opAUF[MN], Parameter = Memnoic.adDB[MN] + Convert.ToString(BD[pos + 1]) });
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
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSE[MN], Parameter = Memnoic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                //Result = Result + Memnoic.opSE[MN] + Memnoic.adT[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x28:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opL[MN], Parameter = "B#16#" + (BD[pos + 1]).ToString("X") });
                                //Result = Result + Memnoic.opL[MN] + "B#16#" + (BD[pos + 1]).ToString("X");
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x29:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSLD[MN], Parameter = (BD[pos + 1]).ToString() });
                                //Result = Result + Memnoic.opSLD[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x2C:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSS[MN], Parameter = Memnoic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                //Result = Result + Memnoic.opSS[MN] + Memnoic.adT[MN] + Convert.ToString(BD[pos + 1]);
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
                                        //Result = Result + "2#" + Helper.GetWordBool(BD[pos + 2], BD[pos + 3]);
                                        par = "2#" + Helper.GetWordBool(BD[pos + 2], BD[pos + 3]);
                                        break;
                                    case 0x03:
                                        par = Convert.ToString(libnodave.getS16from(BD, pos + 2)); //Helper.GetInt(BD[pos + 2], BD[pos + 3]));
                                        //Result = Result + Convert.ToString(Helper.GetInt(BD[pos + 2], BD[pos + 3]));
                                        break;
                                    case 0x05:
                                        par = Helper.GetS7String(pos + 2, 2, BD);
                                        //Result = Result + Helper.GetS7String(pos + 2, 2, BD);
                                        break;
                                    case 0x06:
                                        par = "B#(" + Convert.ToString(BD[pos + 2]) + ", " +
                                                 Convert.ToString(BD[pos + 3]) + ")";
                                        break;
                                    case 0x00:
                                    case 0x07:
                                        par = "W#16#" + libnodave.getU16from(BD, pos + 2).ToString("X"); // Helper.GetWord(BD[pos + 2], BD[pos + 3]).ToString("X");
                                        //Result = Result + "W#16#" +
                                        //         Helper.GetWord(BD[pos + 2], BD[pos + 3]).ToString("X");
                                        break;
                                    case 0x08:
                                        par = "C#" + libnodave.getU16from(BD, pos + 2).ToString("X"); // Helper.GetWord(BD[pos + 2], BD[pos + 3]).ToString("X");
                                        //Result = Result + "C#" + Helper.GetWord(BD[pos + 2], BD[pos + 3]).ToString("X");
                                        break;
                                    case 0x0A:
                                        par = Helper.GetDate(BD[pos + 2], BD[pos + 3]);
                                        //Result = Result + Helper.GetDate(BD[pos + 2], BD[pos + 3]);
                                        break;
                                    case 0x0C:
                                        par = Helper.GetS5Time(BD[pos + 2], BD[pos + 3]);
                                        //Result = Result + Helper.GetS5Time(BD[pos + 2], BD[pos + 3]);
                                        break;
                                }
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opL[MN], Parameter = par });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                pos += 4;
                            }
                            break;
                        case 0x31:
                            {
                                switch (BD[pos + 1])
                                {
                                    case 0x20:
                                        //Result = Result + ">R";
                                        retVal.Add(new S7FunctionBlockRow() { Command = ">R" });
                                        break;
                                    case 0x40:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "<R" });
                                        //Result = Result + "<R";
                                        break;
                                    case 0x60:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "<>R" });
                                        //Result = Result + "<>R";
                                        break;
                                    case 0x80:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "==R" });
                                        //Result = Result + "==R";
                                        break;
                                    case 0xA0:
                                        retVal.Add(new S7FunctionBlockRow() { Command = ">=R" });
                                        //Result = Result + ">=R";
                                        break;
                                    case 0xC0:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "<=R" });
                                        //Result = Result + "<=R";
                                        break;
                                }
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x34:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSI[MN], Parameter = Memnoic.adT[MN] + Convert.ToString(BD[pos + 1]) });
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
                                        par = libnodave.getFloatfrom(BD, pos + 2).ToString("0.000000e+000");
                                        break;
                                    case 0x02:
                                        par = "2#" +
                                                 Helper.GetDWordBool(BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5]);
                                        break;
                                    case 0x03:
                                        par = "L#" + Convert.ToString(libnodave.getS32from(BD, pos + 2));
                                        //par = "L#" + Convert.ToString(Helper.GetDInt(BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5]));
                                        break;
                                    case 0x04:
                                        par = Helper.GetPointer(BD,pos+2);
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
                                        //par = "DW#16#" + Helper.GetDWord(BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5]).ToString("X");
                                        par = "DW#16#" + libnodave.getU32from(BD, pos + 2).ToString("X");
                                        break;
                                    case 0x09:
                                        par = Helper.GetDTime(BD, pos + 2);
                                        //par = Helper.GetDTime(BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5]);
                                        break;
                                    case 0x0b:
                                        //par = Helper.GetTOD(BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5]);
                                        par = Helper.GetTOD(BD, pos + 2);
                                        break;
                                }
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opL[MN], Parameter = par });
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
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opR[MN], Parameter = Memnoic.adT[MN] + Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x3D:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opUC[MN], Parameter = Memnoic.adFC[MN] + Convert.ToString(BD[pos + 1]) });

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
                                            retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opUW[MN] });
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
                                            Command = Memnoic.opZUW[MN],
                                            Parameter = Memnoic.adE[MN] +
                                                Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "." +
                                                Convert.ToString(BD[pos + 1] - 0x10)
                                        });
                                        //Result = Result + Memnoic.opZUW[MN] + Memnoic.adE[MN] +
                                        //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3])) + "." +
                                        //         Convert.ToString(BD[pos + 1] - 0x10);
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
                                            Command = Memnoic.opZUW[MN],
                                            Parameter = Memnoic.adA[MN] +
                                                Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "." +
                                                Convert.ToString(BD[pos + 1] - 0x20)
                                        });
                                        //Result = Result + Memnoic.opZUW[MN] + Memnoic.adA[MN] +
                                        //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3])) + "." +
                                        //         Convert.ToString(BD[pos + 1] - 0x20);
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
                                            Command = Memnoic.opZUW[MN],
                                            Parameter = Memnoic.adM[MN] +
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
                                            if (retVal[retVal.Count - 1].Command == Memnoic.opAUF[MN] && !((S7FunctionBlockRow)retVal[retVal.Count - 1]).Parameter.Contains("[") && CombineDBOpenAndCommand)
                                            {
                                                DBByte = ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7;
                                                par = ((S7FunctionBlockRow)retVal[retVal.Count - 1]).Parameter + ".";
                                                retVal.RemoveAt(retVal.Count - 1);
                                            }
                                            retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opZUW[MN], Parameter = par + Memnoic.adDBX[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "." + Convert.ToString(BD[pos + 1] - 0x40) });
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
                                                               Command = Memnoic.opZUW[MN],
                                                               Parameter = Memnoic.adDIX[MN] +
                                                                           Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                                           "." +
                                                                           Convert.ToString(BD[pos + 1] - 0x50)
                                                           });
                                            //Result = Result + Memnoic.opZUW[MN] + Memnoic.adDIX[MN] +
                                            //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3])) + "." +
                                            //         Convert.ToString(BD[pos + 1] - 0x50);
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
                                            Command = Memnoic.opZUW[MN],
                                            Parameter = Memnoic.adL[MN] +
                                                Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "." +
                                                Convert.ToString(BD[pos + 1] - 0x60)
                                        });
                                        //Result = Result + Memnoic.opZUW[MN] + Memnoic.adL[MN] +
                                        //         Convert.ToString(Helper.GetWord(BD[pos + 2], BD[pos + 3])) + "." +
                                        //         Convert.ToString(BD[pos + 1] - 0x60);
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
                                    Command = Memnoic.opL[MN],
                                    Parameter = Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1])
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
                                    Command = Memnoic.opFR[MN],
                                    Parameter = Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1])
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
                                                       Command = Memnoic.opL[MN],
                                                       Parameter = Memnoic.adEB[MN] + Convert.ToString(BD[pos + 1])
                                                   });
                                //Result = Result + Memnoic.opL[MN] + Memnoic.adEB[MN] + Convert.ToString(BD[pos + 1]);
                                else
                                    retVal.Add(new S7FunctionBlockRow()
                                    {
                                        Command = Memnoic.opL[MN],
                                        Parameter = Memnoic.adAB[MN] + Convert.ToString(BD[pos + 1] - 0x80)
                                    });
                                //Result = Result + Memnoic.opL[MN] + Memnoic.adAB[MN] + Convert.ToString(BD[pos + 1] - 0x80);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x4b:
                            {
                                if (BD[pos + 1] < 0x80)
                                    retVal.Add(new S7FunctionBlockRow()
                                                   {
                                                       Command = Memnoic.opT[MN],
                                                       Parameter = Memnoic.adEB[MN] + Convert.ToString(BD[pos + 1])
                                                   });
                                //Result = Result + Memnoic.opT[MN] + Memnoic.adEB[MN] + Convert.ToString(BD[pos + 1]);
                                else
                                    retVal.Add(new S7FunctionBlockRow()
                                                   {
                                                       Command = Memnoic.opT[MN],
                                                       Parameter = Memnoic.adAB[MN] + Convert.ToString(BD[pos + 1] - 0x80)
                                                   });
                                //Result = Result + Memnoic.opT[MN] + Memnoic.adAB[MN] + Convert.ToString(BD[pos + 1] - 0x80);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x4C:
                            {
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Memnoic.opLC[MN],
                                    Parameter = Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1])
                                });
                                //Result = Result + Memnoic.opLC[MN] + Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1]);
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
                                            cmd = Memnoic.opXOW[MN];
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd});
                                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                    pos += 2;
                                            break;
                                        case 0x58:
                                            cmd = Memnoic.opPLU[MN];
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
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd});
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
                                                cmd = LowByte < 0x09 ? Memnoic.opU[MN] : Memnoic.opO[MN];
                                            else
                                                cmd = LowByte < 0x09 ? Memnoic.opUN[MN] : Memnoic.opON[MN];
                                            break;
                                        case 0x58:
                                            if (BD[pos + 1] < 0xb0)
                                                cmd = LowByte < 0x09 ? Memnoic.opX[MN] : Memnoic.opS[MN];
                                            else
                                                cmd = LowByte < 0x09 ? Memnoic.opXN[MN] : Memnoic.opR[MN];
                                            break;
                                        case 0x59:
                                            if (BD[pos + 1] < 0xb0)
                                                cmd = LowByte < 0x09 ? Memnoic.opZUW[MN] : Memnoic.opFP[MN];
                                            else
                                                cmd = LowByte < 0x09 ? "err1" : Memnoic.opFN[MN];//Don't know the Low Byte command
                                            break;
                                    }
                                    switch (BD[pos + 1] & 0x0F)
                                    {
                                        case 0x01:
                                        case 0x09:
                                            par = Memnoic.adE[MN];
                                            break;
                                        case 0x02:
                                        case 0x0A:
                                            par = Memnoic.adA[MN];
                                            break;
                                        case 0x03:
                                        case 0x0B:
                                            par = Memnoic.adM[MN];
                                            break;
                                        case 0x04:
                                        case 0x0C:
                                            par = Memnoic.adDBX[MN];
                                            break;
                                        case 0x05:
                                        case 0x0D:
                                            par = Memnoic.adDIX[MN];
                                            break;
                                        case 0x06:
                                        case 0x0E:
                                            par = Memnoic.adL[MN];
                                            break;

                                    }

                                    switch (BD[pos + 1] & 0xF0)
                                    {

                                        case 0x30:
                                        case 0xb0:
                                            par += Memnoic.adMD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x40:
                                        case 0xc0:
                                            par += Memnoic.adDBD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x50:
                                        case 0xd0:
                                            par += Memnoic.adDID[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x60:
                                        case 0xe0:
                                            par += Memnoic.adLD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x70:
                                        case 0xf0:
                                            par += Memnoic.adMD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
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
                                                       Command = Memnoic.opL[MN],
                                                       Parameter = Memnoic.adEW[MN] + Convert.ToString(BD[pos + 1])
                                                   });
                                else
                                    retVal.Add(new S7FunctionBlockRow()
                                                   {
                                                       Command = Memnoic.opL[MN],
                                                       Parameter = Memnoic.adAW[MN] + Convert.ToString(BD[pos + 1] - 0x80)
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
                                                           Command = Memnoic.opT[MN],
                                                           Parameter = Memnoic.adEW[MN] + Convert.ToString(BD[pos + 1])
                                                       });
                                    //Result = Result + Memnoic.opT[MN] + Memnoic.adEW[MN] + Convert.ToString(BD[pos + 1]);
                                    else                                        
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Memnoic.opT[MN],
                                                Parameter = Memnoic.adAW[MN] + Convert.ToString(BD[pos + 1] - 0x80)
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
                                        Command = Memnoic.opZR[MN],
                                        Parameter = Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1])
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
                                        Command = Memnoic.opCC[MN],
                                        Parameter = Memnoic.adFB[MN] + Convert.ToString(BD[pos + 1])
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
                                                       Command = Memnoic.opL[MN],
                                                       Parameter = Memnoic.adED[MN] + Convert.ToString(BD[pos + 1])
                                                   });
                                //Result = Result + Memnoic.opL[MN] + Memnoic.adED[MN] + Convert.ToString(BD[pos + 1]);
                                else
                                    retVal.Add(new S7FunctionBlockRow()
                                                   {
                                                       Command = Memnoic.opL[MN],
                                                       Parameter = Memnoic.adAD[MN] + Convert.ToString(BD[pos + 1] - 0x80)
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
                                        Command = Memnoic.opT[MN],
                                        Parameter = Memnoic.adED[MN] + Convert.ToString(BD[pos + 1])
                                    });
                                else
                                    retVal.Add(new S7FunctionBlockRow()
                                    {
                                        Command = Memnoic.opT[MN],
                                        Parameter = Memnoic.adAD[MN] + Convert.ToString(BD[pos + 1])
                                    });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x5C:
                            {
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Memnoic.opS[MN],
                                    Parameter = Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1])
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
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opMOD[MN] });
                                        break;
                                    case 0x02:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opABS[MN] });
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
                                                               Command = Memnoic.opPLU[MN],
                                                               Parameter = "L#" + Convert.ToString(libnodave.getS32from(BD, pos + 2))                                                               
                                                           });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };
                                            pos += 6;
                                            goto brk2;
                                        }
                                        break;
                                    case 0x06:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opNEGR[MN] });
                                        break;
                                    case 0x07:
                                        retVal.Add(new S7FunctionBlockRow() { Command = "*R" });
                                        break;
                                    case 0x08:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opENT[MN] });
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
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSIN[MN] });
                                        break;
                                    case 0x11:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opCOS[MN] });
                                        break;
                                    case 0x12:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opTAN[MN] });
                                        break;
                                    case 0x13:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opLN[MN] });
                                        break;
                                    case 0x14:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSQRT[MN] });
                                        break;
                                    case 0x18:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opASIN[MN] });
                                        break;
                                    case 0x19:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opACOS[MN] });
                                        break;
                                    case 0x1A:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opATAN[MN] });
                                        break;
                                    case 0x1b:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opEXP[MN] });
                                        break;
                                    case 0x1C:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSQR[MN] });
                                        break;
                                }
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                    brk2:
                            break;
                        case 0x61:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSLW[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                //Result = Result + Memnoic.opSLW[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x64:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opRLD[MN], Parameter = Convert.ToString(BD[pos + 1]) });
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
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opBE[MN] });
                                        //Result = Result + Memnoic.opBE[MN];
                                        break;
                                    case 0x01:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opBEA[MN] });
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
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opDTR[MN] });
                                        //Result = Result + Memnoic.opDTR[MN];
                                        break;
                                    case 0x07:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opNEGD[MN] });
                                        //Result = Result + Memnoic.opNEGD[MN];
                                        break;
                                    case 0x08:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opITB[MN] });
                                        //Result = Result + Memnoic.opITB[MN];
                                        break;
                                    case 0x0C:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opBTI[MN] });
                                        //Result = Result + Memnoic.opBTI[MN];
                                        break;
                                    case 0x0A:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opDTB[MN] });
                                        //Result = Result + Memnoic.opDTB[MN];
                                        break;
                                    case 0x0D:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opINVD[MN] });
                                        //Result = Result + Memnoic.opINVD[MN];
                                        break;
                                    case 0x0E:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opBTD[MN] });
                                        //Result = Result + Memnoic.opBTD[MN];
                                        break;
                                    case 0x12:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSLW[MN] });
                                        //Result = Result + Memnoic.opSLW[MN];
                                        break;
                                    case 0x13:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSLD[MN] });
                                        //Result = Result + Memnoic.opSLD[MN];
                                        break;
                                    case 0x17:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opRLD[MN] });
                                        //Result = Result + Memnoic.opRLD[MN];
                                        break;
                                    case 0x18:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opRLDA[MN] });
                                        //Result = Result + Memnoic.opRLDA[MN];
                                        break;
                                    case 0x1A:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opTAW[MN] });
                                        //Result = Result + Memnoic.opTAW[MN];
                                        break;
                                    case 0x1b:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opTAD[MN] });
                                        //Result = Result + Memnoic.opTAD[MN];
                                        break;
                                    case 0x1C:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opCLR[MN] });
                                        //Result = Result + Memnoic.opCLR[MN];
                                        break;
                                    case 0x1D:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSET[MN] });
                                        //Result = Result + Memnoic.opSET[MN];
                                        break;
                                    case 0x1E:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opITD[MN] });
                                        //Result = Result + Memnoic.opITD[MN];
                                        break;
                                    case 0x22:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSRW[MN] });
                                        //Result = Result + Memnoic.opSRW[MN];
                                        break;
                                    case 0x23:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSRD[MN] });
                                        //Result = Result + Memnoic.opSRD[MN];
                                        break;
                                    case 0x24:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSSI[MN] });
                                        //Result = Result + Memnoic.opSSI[MN];
                                        break;
                                    case 0x25:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSSD[MN] });
                                        //Result = Result + Memnoic.opSSD[MN];
                                        break;
                                    case 0x27:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opRRD[MN] });
                                        //Result = Result + Memnoic.opRRD[MN];
                                        break;
                                    case 0x28:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opRRDA[MN] });
                                        //Result = Result + Memnoic.opRRDA[MN];
                                        break;
                                    case 0x2C:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSAVE[MN] });
                                        //Result = Result + Memnoic.opSAVE[MN];
                                        break;
                                    case 0x2D:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opNOT[MN] });
                                        //Result = Result + Memnoic.opNOT[MN];
                                        break;
                                    case 0x2E:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opPUSH[MN] });
                                        //Result = Result + Memnoic.opPUSH[MN];
                                        break;
                                    case 0x37:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opUD[MN] });
                                        //Result = Result + Memnoic.opUD[MN];
                                        break;
                                    case 0x34:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                                           {
                                                               Command = Memnoic.opUW[MN],
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
                                                Command = Memnoic.opUD[MN],
                                                Parameter = "DW#16#" + (libnodave.getU32from(BD,pos + 2).ToString("X"))
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };
                                            pos += 6;
                                            goto brk1;
                                        }
                                        break;
                                    case 0x3A:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opMCRA[MN] });
                                        //Result = Result + Memnoic.opMCRA[MN];
                                        break;
                                    case 0x3b:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opMCRD[MN] });
                                        //Result = Result + Memnoic.opMCRD[MN];
                                        break;
                                    case 0x3C:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opMCRO[MN] });
                                        //Result = Result + Memnoic.opMCRO[MN];
                                        break;
                                    case 0x3D:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opMCRC[MN] });
                                        //Result = Result + Memnoic.opMCRC[MN];
                                        break;
                                    case 0x3E:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opPOP[MN] });
                                        //Result = Result + Memnoic.opPOP[MN];
                                        break;
                                    case 0x44:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Memnoic.opOW[MN],
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
                                                Command = Memnoic.opOD[MN],
                                                Parameter = "DW#16#" + libnodave.getU32from(BD,pos + 2).ToString("X")                                               
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };
                                            pos += 6;
                                            goto brk1;
                                        }
                                        break;
                                    case 0x47:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opOD[MN] });
                                        break;
                                    case 0x4E:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opLEAVE[MN] });
                                        break;
                                    case 0x54:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Memnoic.opXOW[MN],
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
                                                Command = Memnoic.opXOD[MN],
                                                Parameter = "DW#16#" + libnodave.getU32from(BD, pos + 2).ToString("X")
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };
                                            pos += 6;
                                            goto brk1;
                                        }
                                        break;
                                    case 0x57:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opXOD[MN] });
                                        break;
                                    case 0x5C:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opRND[MN] });
                                        break;
                                    case 0x5D:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opRNDM[MN] });
                                        break;
                                    case 0x5E:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opRNDP[MN] });
                                        break;
                                    case 0x5F:
                                        retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opTRUNC[MN] });
                                        break;
                                    default:
                                        {
                                            if ((BD[pos + 1] & 0x0F) == 0x01)
                                                retVal.Add(new S7FunctionBlockRow()
                                                               {
                                                                   Command = Memnoic.opSSI[MN],
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
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSRW[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                //Result = Result + Memnoic.opSRW[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x6C:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opZV[MN], Parameter = Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1]) });
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
                                            retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opTAK[MN] });
                                            //Result = Result + Memnoic.opTAK[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x06:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opL[MN], Parameter = Memnoic.adSTW[MN] });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x07:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opT[MN], Parameter = Memnoic.adSTW[MN] });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x08:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                                           {
                                                               Command = Memnoic.opLOOP[MN],
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
                                                               Command = Memnoic.opSPL[MN],
                                                               JumpWidth = libnodave.getS16from(BD, pos + 2)
                                                           });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x0b:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSPA[MN], JumpWidth = libnodave.getS16from(BD,pos+2) });
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
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opSSD[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                //Result = Result + Memnoic.opSSD[MN] + Convert.ToString(BD[pos + 1]);
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x74:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opRRD[MN], Parameter = Convert.ToString(BD[pos + 1]) });
                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                pos += 2;
                            }
                            break;
                        case 0x75:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opUC[MN], Parameter = Memnoic.adFB[MN] + Convert.ToString(BD[pos + 1]) });
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
                                            cmd += (HighByte < 0x90) ? Memnoic.opU[MN] : Memnoic.opS[MN];
                                            break;
                                        case 0x01:
                                        case 0x09:
                                            cmd += (HighByte < 0x90) ? Memnoic.opUN[MN] : Memnoic.opR[MN];
                                            break;
                                        case 0x02:
                                        case 0x0A:
                                            cmd += (HighByte < 0x90) ? Memnoic.opO[MN] : Memnoic.opZUW[MN];
                                            break;
                                        case 0x03:
                                        case 0x0B:
                                            cmd += (HighByte < 0x90) ? Memnoic.opON[MN] : "err5"; //Ther is no Value for this???
                                            break;
                                        case 0x04:
                                        case 0x0C:
                                            cmd += (HighByte < 0x90) ? Memnoic.opX[MN] : Memnoic.opFP[MN];
                                            break;
                                        case 0x05:
                                        case 0x0D:
                                            cmd += (HighByte < 0x90) ? Memnoic.opXN[MN] : Memnoic.opFN[MN];
                                            break;
                                    }

                                    switch (HighByte)
                                    {
                                        case 0x10:
                                            par += Memnoic.adE[MN];
                                            break;
                                        case 0x20:
                                            par += Memnoic.adA[MN];
                                            break;
                                        case 0x30:
                                            par += Memnoic.adM[MN];
                                            break;
                                        case 0x40:
                                            par += Memnoic.adDBX[MN];
                                            break;
                                        case 0x50:
                                            par += Memnoic.adDIX[MN];
                                            break;
                                        case 0x60:
                                            par += Memnoic.adL[MN];
                                            break;

                                    }
                                    if (LowByte < 0x08)
                                        par += "[" + Memnoic.adAR1[MN] + "," +
                                               Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                    else
                                        par += "[" + Memnoic.adAR2[MN] + "," +
                                               Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                    retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                    pos += 4;
                                }


                            }
                            break;
                        case 0x7C:
                            {
                                retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opR[MN], Parameter = Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1]) });
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
                                    cmd = Memnoic.opT[MN];
                                    //if (LowByte>= 0x05 && LowByte<=0x09 || LowByte >)
                                    //cmd = (LowByte <= 0x07) ? Memnoic.opL[MN] : Memnoic.opT[MN];
                                    if (LowByte <= 0x04|| ((LowByte >= 0x09) && LowByte <= 0x0c))
                                        cmd = Memnoic.opL[MN];

                                    switch (HighByte)
                                    {
                                        case 0x00:
                                            switch (LowByte)
                                            {
                                                case 0x01:
                                                case 0x09:
                                                    par += Memnoic.adPEB[MN];
                                                    break;
                                                case 0x02:
                                                case 0x0A:
                                                    par += Memnoic.adPEW[MN];
                                                    break;
                                                case 0x03:
                                                case 0x0B:
                                                    par += Memnoic.adPED[MN];
                                                    break;
                                                case 0x05:
                                                case 0x0D:
                                                    par += Memnoic.adPAB[MN];
                                                    break;
                                                case 0x06:
                                                case 0x0E:
                                                    par += Memnoic.adPAW[MN];
                                                    break;
                                                case 0x07:
                                                case 0x0F:
                                                    par += Memnoic.adPAD[MN];
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
                                                    par += Memnoic.adEB[MN];
                                                    break;
                                                case 0x02:
                                                case 0x06:
                                                case 0x0A:
                                                case 0x0E:
                                                    par += Memnoic.adEW[MN];
                                                    break;
                                                case 0x03:
                                                case 0x07:
                                                case 0x0B:
                                                case 0x0F:
                                                    par += Memnoic.adED[MN];
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
                                                    par += Memnoic.adAB[MN];
                                                    break;
                                                case 0x02:
                                                case 0x06:
                                                case 0x0A:
                                                case 0x0E:
                                                    par += Memnoic.adAW[MN];
                                                    break;
                                                case 0x03:
                                                case 0x07:
                                                case 0x0B:
                                                case 0x0F:
                                                    par += Memnoic.adAD[MN];
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
                                                    par += Memnoic.adMB[MN];
                                                    break;
                                                case 0x02:
                                                case 0x06:
                                                case 0x0A:
                                                case 0x0E:
                                                    par += Memnoic.adMW[MN];
                                                    break;
                                                case 0x03:
                                                case 0x07:
                                                case 0x0B:
                                                case 0x0F:
                                                    par += Memnoic.adMD[MN];
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
                                                    par += Memnoic.adDBB[MN];
                                                    break;
                                                case 0x02:
                                                case 0x06:
                                                case 0x0A:
                                                case 0x0E:
                                                    par += Memnoic.adDBW[MN];
                                                    break;
                                                case 0x03:
                                                case 0x07:
                                                case 0x0B:
                                                case 0x0F:
                                                    par += Memnoic.adDBD[MN];
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
                                                    par += Memnoic.adDIB[MN];
                                                    break;
                                                case 0x02:
                                                case 0x06:
                                                case 0x0A:
                                                case 0x0E:
                                                    par += Memnoic.adDIW[MN];
                                                    break;
                                                case 0x03:
                                                case 0x07:
                                                case 0x0B:
                                                case 0x0F:
                                                    par += Memnoic.adDID[MN];
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
                                                    par += Memnoic.adLB[MN];
                                                    break;
                                                case 0x02:
                                                case 0x06:
                                                case 0x0A:
                                                case 0x0E:
                                                    par += Memnoic.adLW[MN];
                                                    break;
                                                case 0x03:
                                                case 0x07:
                                                case 0x0B:
                                                case 0x0F:
                                                    par += Memnoic.adLD[MN];
                                                    break;
                                            }
                                            break;
                                    }
                                    if (BD[pos] == 0x7E)
                                        par += Convert.ToString(libnodave.getU16from(BD, pos + 2));
                                    else if (BD[pos] == 0xBE)
                                        if (LowByte < 0x09)
                                            par += "[" + Memnoic.adAR1[MN] + "," +
                                                         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                        else
                                            par += "[" + Memnoic.adAR2[MN] + "," +
                                                     Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                    retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                    ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
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
                                               Command = Memnoic.opU[MN],
                                               Parameter = Memnoic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
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
                                Command = Memnoic.opO[MN],
                                Parameter = Memnoic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
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
                                Command = Memnoic.opS[MN],
                                Parameter = Memnoic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
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
                                Command = Memnoic.opZUW[MN],
                                Parameter = Memnoic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
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
                                Command = Memnoic.opUN[MN],
                                Parameter = Memnoic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
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
                                Command = Memnoic.opON[MN],
                                Parameter = Memnoic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
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
                                Command = Memnoic.opR[MN],
                                Parameter = Memnoic.adM[MN] + Convert.ToString(BD[pos + 1]) + "." +
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
                                    Command = Memnoic.opU[MN],
                                    Parameter = Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1])
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
                                    Command = Memnoic.opO[MN],
                                    Parameter = Memnoic.adZ[MN] + Convert.ToString(BD[pos + 1])
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
                                            cmd = cmd + Memnoic.opUO[MN];
                                            break;
                                        case 0xBB:
                                            cmd = cmd + Memnoic.opOO[MN];
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
                                    cmd += HighByte < 0x80 ? Memnoic.opL[MN] : Memnoic.opT[MN];

                                    switch (BD[pos])
                                    {
                                        case 0xBA:
                                            switch (LowByte)
                                            {
                                                case 0x00:
                                                    par += HighByte < 0x80 ? Memnoic.adPEB[MN] : Memnoic.adPAB[MN];
                                                    break;
                                                case 0x01:
                                                    par += Memnoic.adEB[MN];
                                                    break;
                                                case 0x02:
                                                    par += Memnoic.adAB[MN];
                                                    break;
                                                case 0x03:
                                                    par += Memnoic.adMB[MN];
                                                    break;
                                                case 0x04:
                                                    par += Memnoic.adDBB[MN];
                                                    break;
                                                case 0x05:
                                                    par += Memnoic.adDIB[MN];
                                                    break;
                                                case 0x06:
                                                    par += Memnoic.adLB[MN];
                                                    break;
                                            }
                                            break;
                                        case 0xBB:
                                            switch (LowByte)
                                            {
                                                case 0x00:
                                                    par += HighByte < 0x80 ? Memnoic.adPEW[MN] : Memnoic.adPAW[MN];
                                                    break;
                                                case 0x01:
                                                    par += Memnoic.adEW[MN];
                                                    break;
                                                case 0x02:
                                                    par += Memnoic.adAW[MN];
                                                    break;
                                                case 0x03:
                                                    par += Memnoic.adMW[MN];
                                                    break;
                                                case 0x04:
                                                    par += Memnoic.adDBW[MN];
                                                    break;
                                                case 0x05:
                                                    par += Memnoic.adDIW[MN];
                                                    break;
                                                case 0x06:
                                                    par += Memnoic.adLW[MN];
                                                    break;
                                                case 0x08:
                                                    par += HighByte < 0x80 ? Memnoic.adPED[MN] : Memnoic.adPAD[MN];
                                                    break;
                                                case 0x09:
                                                    par += Memnoic.adED[MN];
                                                    break;
                                                case 0x0A:
                                                    par += Memnoic.adAD[MN];
                                                    break;
                                                case 0x0B:
                                                    par += Memnoic.adMD[MN];
                                                    break;
                                                case 0x0C:
                                                    par += Memnoic.adDBD[MN];
                                                    break;
                                                case 0x0D:
                                                    par += Memnoic.adDID[MN];
                                                    break;
                                                case 0x0E:
                                                    par += Memnoic.adLD[MN];
                                                    break;
                                            }
                                            break;
                                    }

                                    switch (HighByte)
                                    {
                                        case 0x00: //Not from old programm, guessed Values!
                                        case 0x80:
                                            par += "[" + Memnoic.adPED[MN] +
                                                   Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x10://Not from old programm, guessed Values!
                                        case 0x90:
                                            par += "[" + Memnoic.adED[MN] +
                                                   Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x20://Not from old programm, guessed Values!
                                        case 0xa0:
                                            par += "[" + Memnoic.adAD[MN] +
                                                   Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x30:
                                        case 0xb0:
                                            par += "[" + Memnoic.adMD[MN] +
                                                   Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x40:
                                        case 0xc0:
                                            par += "[" + Memnoic.adDBD[MN] +
                                                   Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x50:
                                        case 0xd0:
                                            par += "[" + Memnoic.adDID[MN] +
                                                   Convert.ToString(libnodave.getU16from(BD, pos + 2)) +
                                                   "]";
                                            break;
                                        case 0x60:
                                        case 0xe0:
                                            par += "[" + Memnoic.adLD[MN] +
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
                                            cmd += Memnoic.opU[MN];
                                            break;
                                        case 0x01:
                                            cmd += Memnoic.opUN[MN];
                                            break;
                                        case 0x02:
                                            cmd += Memnoic.opO[MN];
                                            break;
                                        case 0x03:
                                            cmd += Memnoic.opON[MN];
                                            break;
                                        case 0x04:
                                            cmd += Memnoic.opX[MN];
                                            break;
                                        case 0x05:
                                            cmd += Memnoic.opXN[MN];
                                            break;
                                        case 0x06:
                                            cmd += Memnoic.opL[MN];
                                            break;
                                        case 0x08:
                                            cmd += Memnoic.opFR[MN];
                                            break;
                                        case 0x09:
                                            cmd += Memnoic.opLC[MN];
                                            break;
                                        case 0x0A:
                                            cmd += HighByte < 0x80 ? Memnoic.opSA[MN] : Memnoic.opZR[MN];
                                            break;
                                        case 0x0B:
                                            cmd += HighByte < 0x80 ? Memnoic.opSV[MN] : Memnoic.opS[MN];
                                            break;
                                        case 0x0C:
                                            cmd += Memnoic.opSE[MN];
                                            break;
                                        case 0x0D:
                                            cmd += HighByte < 0x80 ? Memnoic.opSS[MN] : Memnoic.opZV[MN];
                                            break;
                                        case 0x0E:
                                            cmd += Memnoic.opSI[MN];
                                            break;
                                        case 0x0F:
                                            cmd += Memnoic.opR[MN];
                                            break;
                                    }
                                    if (HighByte < 0x80)
                                        par += Memnoic.adT[MN];
                                    else
                                        par += Memnoic.adZ[MN];

                                    switch (HighByte)
                                    {
                                        case 0x00://Guessed Value, not from old PRG
                                        case 0x80:
                                            par += "[" + Memnoic.adPEW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]";
                                            break;
                                        case 0x10://Guessed Value, not from old PRG
                                        case 0x90:
                                            par += "[" + Memnoic.adEW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]";
                                            break;
                                        case 0x20://Guessed Value, not from old Prg
                                        case 0xa0:
                                            par += "[" + Memnoic.adAW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]";
                                            break;
                                        case 0x30:
                                        case 0xb0:
                                            par += "[" + Memnoic.adMW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]";
                                            break;
                                        case 0x40:
                                        case 0xc0:
                                            par += "[" + Memnoic.adDBW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]";
                                            break;
                                        case 0x50:
                                        case 0xd0:
                                            par += "[" + Memnoic.adDIW[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]";
                                            break;
                                        case 0x60:
                                        case 0xe0:
                                            par += "[" + Memnoic.adLW[MN] +
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
                                    Command = Memnoic.opU[MN],
                                    Parameter = Memnoic.adE[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xC0)
                                });
                            //Result = Result + Memnoic.opU[MN] + Memnoic.adE[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xC0);
                            else
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Memnoic.opU[MN],
                                    Parameter = Memnoic.adA[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
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
                                    Command = Memnoic.opO[MN],
                                    Parameter = Memnoic.adE[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xC8)
                                });
                            //Result = Result + Memnoic.opO[MN] + Memnoic.adE[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xC8);
                            else
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Memnoic.opO[MN],
                                    Parameter = Memnoic.adA[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
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
                                    Command = Memnoic.opS[MN],
                                    Parameter = Memnoic.adE[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xD0)
                                });
                            //Result = Result + Memnoic.opS[MN] + Memnoic.adE[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xD0);
                            else
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Memnoic.opS[MN],
                                    Parameter = Memnoic.adA[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
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
                                    Command = Memnoic.opZUW[MN],
                                    Parameter = Memnoic.adE[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xD8)
                                });
                            //Result = Result + Memnoic.opZUW[MN] + Memnoic.adE[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xD8);
                            else
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Memnoic.opZUW[MN],
                                    Parameter = Memnoic.adA[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
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
                                    Command = Memnoic.opUN[MN],
                                    Parameter = Memnoic.adE[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xE0)
                                });
                            //Result = Result + Memnoic.opUN[MN] + Memnoic.adE[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xE0);
                            else
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Memnoic.opU[MN],
                                    Parameter = Memnoic.adA[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
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
                                    Command = Memnoic.opON[MN],
                                    Parameter = Memnoic.adE[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xE8)
                                });
                            //Result = Result + Memnoic.opON[MN] + Memnoic.adE[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xE8);
                            else
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Memnoic.opON[MN],
                                    Parameter = Memnoic.adA[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
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
                                    Command = Memnoic.opR[MN],
                                    Parameter = Memnoic.adE[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
                                         Convert.ToString(BD[pos] - 0xF0)
                                });
                            //Result = Result + Memnoic.opR[MN] + Memnoic.adE[MN] + Convert.ToString(BD[pos + 1]) + "." +
                            //         Convert.ToString(BD[pos] - 0xF0);
                            else
                                retVal.Add(new S7FunctionBlockRow()
                                {
                                    Command = Memnoic.opR[MN],
                                    Parameter = Memnoic.adA[MN] + Convert.ToString(BD[pos + 1] & 0x7F) + "." +
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
                                Command = Memnoic.opU[MN],
                                Parameter = Memnoic.adT[MN] + Convert.ToString(BD[pos + 1])
                            });
                            //Result = Result + Memnoic.opU[MN] + Memnoic.adT[MN] + Convert.ToString(BD[pos + 1]);
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xF9:
                            retVal.Add(new S7FunctionBlockRow()
                            {
                                Command = Memnoic.opO[MN],
                                Parameter = Memnoic.adT[MN] + Convert.ToString(BD[pos + 1])
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
                                            retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opO[MN] });
                                            //Result = Result + Memnoic.opO[MN];
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x01:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Memnoic.opL[MN],
                                                Parameter = Memnoic.adB[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                                    Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opL[MN] + Memnoic.adB[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x02:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Memnoic.opL[MN],
                                                Parameter = Memnoic.adW[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                                    Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
                                            });
                                            //Result = Result + Memnoic.opL[MN] + Memnoic.adW[MN] + "[" + Memnoic.adAR1[MN] + "," +
                                            //         Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]";
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x03:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Memnoic.opL[MN],
                                                Parameter = Memnoic.adD[MN] + "[" + Memnoic.adAR1[MN] + "," +
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
                                                Command = Memnoic.opT[MN],
                                                Parameter = Memnoic.adB[MN] + "[" + Memnoic.adAR1[MN] + "," +
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
                                                Command = Memnoic.opT[MN],
                                                Parameter = Memnoic.adW[MN] + "[" + Memnoic.adAR1[MN] + "," +
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
                                                Command = Memnoic.opT[MN],
                                                Parameter = Memnoic.adD[MN] + "[" + Memnoic.adAR1[MN] + "," +
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
                                                Command = Memnoic.opL[MN],
                                                Parameter = Memnoic.adB[MN] + "[" + Memnoic.adAR2[MN] + "," +
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
                                                Command = Memnoic.opL[MN],
                                                Parameter = Memnoic.adW[MN] + "[" + Memnoic.adAR2[MN] + "," +
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
                                                Command = Memnoic.opL[MN],
                                                Parameter = Memnoic.adD[MN] + "[" + Memnoic.adAR2[MN] + "," +
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
                                                Command = Memnoic.opT[MN],
                                                Parameter = Memnoic.adB[MN] + "[" + Memnoic.adAR2[MN] + "," +
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
                                                Command = Memnoic.opT[MN],
                                                Parameter = Memnoic.adW[MN] + "[" + Memnoic.adAR2[MN] + "," +
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
                                                Command = Memnoic.opT[MN],
                                                Parameter = Memnoic.adD[MN] + "[" + Memnoic.adAR2[MN] + "," +
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
                                                Command = Memnoic.opU[MN],
                                                Parameter = "[" + Memnoic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opUN[MN],
                                                Parameter = "[" + Memnoic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opO[MN],
                                                Parameter = "[" + Memnoic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opON[MN],
                                                Parameter = "[" + Memnoic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opX[MN],
                                                Parameter = "[" + Memnoic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opXN[MN],
                                                Parameter = "[" + Memnoic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opU[MN],
                                                Parameter = "[" + Memnoic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opUN[MN],
                                                Parameter = "[" + Memnoic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opO[MN],
                                                Parameter = "[" + Memnoic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opON[MN],
                                                Parameter = "[" + Memnoic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opX[MN],
                                                Parameter = "[" + Memnoic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opXN[MN],
                                                Parameter = "[" + Memnoic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opS[MN],
                                                Parameter = "[" + Memnoic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opR[MN],
                                                Parameter = "[" + Memnoic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opZUW[MN],
                                                Parameter = "[" + Memnoic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opFP[MN],
                                                Parameter = "[" + Memnoic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opFN[MN],
                                                Parameter = "[" + Memnoic.adAR1[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opS[MN],
                                                Parameter = "[" + Memnoic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opR[MN],
                                                Parameter = "[" + Memnoic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opZUW[MN],
                                                Parameter = "[" + Memnoic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opFP[MN],
                                                Parameter = "[" + Memnoic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opFN[MN],
                                                Parameter = "[" + Memnoic.adAR2[MN] + "," + Helper.GetShortPointer(BD[pos + 2], BD[pos + 3]) + "]"
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
                                                Command = Memnoic.opAUF[MN],
                                                Parameter = Memnoic.adDB[MN] + "[" + Memnoic.adMW[MN] +
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
                                                Command = Memnoic.opAUF[MN],
                                                Parameter = Memnoic.adDI[MN] + "[" + Memnoic.adMW[MN] +
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
                                                Command = Memnoic.opL[MN],
                                                Parameter = Memnoic.adDBLG[MN]
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
                                                Command = Memnoic.opL[MN],
                                                Parameter = Memnoic.adDILG[MN]
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
                                                Command = Memnoic.opAUF[MN],
                                                Parameter = Memnoic.adDB[MN] + "[" + Memnoic.adDBW[MN] +
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
                                                Command = Memnoic.opAUF[MN],
                                                Parameter = Memnoic.adDI[MN] + "[" + Memnoic.adDBW[MN] +
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
                                                Command = Memnoic.opL[MN],
                                                Parameter = Memnoic.adDBNO[MN]
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
                                                Command = Memnoic.opL[MN],
                                                Parameter = Memnoic.adDINO[MN]
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
                                                Command = Memnoic.opAUF[MN],
                                                Parameter = Memnoic.adDB[MN] + "[" + Memnoic.adDIW[MN] +
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
                                                Command = Memnoic.opAUF[MN],
                                                Parameter = Memnoic.adDI[MN] + "[" + Memnoic.adDIW[MN] +
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
                                                Command = Memnoic.opAUF[MN],
                                                Parameter = Memnoic.adDB[MN] + "[" + Memnoic.adLW[MN] +
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
                                                Command = Memnoic.opAUF[MN],
                                                Parameter = Memnoic.adDI[MN] + "[" + Memnoic.adLW[MN] +
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
                                            retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opUC[MN], Parameter = Memnoic.adFC[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) });

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
                                                    par = Memnoic.adMW[MN];
                                                    break;
                                                case 0x40:
                                                    par = Memnoic.adDBW[MN];
                                                    break;
                                                case 0x50:
                                                    par = Memnoic.adDIW[MN];
                                                    break;
                                                case 0x60:
                                                    par = Memnoic.adLW[MN];
                                                    break;
                                            }
                                            retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opUC[MN], Parameter = Memnoic.adFC[MN] + "[" + par + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]" });

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
                                                    par = Memnoic.adMW[MN];
                                                    break;
                                                case 0x40:
                                                    par = Memnoic.adDBW[MN];
                                                    break;
                                                case 0x60:
                                                    par = Memnoic.adLW[MN];
                                                    break;
                                                case 0x50:
                                                    par = Memnoic.adDIW[MN];
                                                    break;
                                            }
                                            retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opUC[MN], Parameter = Memnoic.adFB[MN] + "[" + par + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + "]" });
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
                                                    par = Memnoic.adLW[MN];
                                                    break;
                                                case 0x41:
                                                    par = Memnoic.adDBW[MN];
                                                    break;
                                                case 0x51:
                                                    par = Memnoic.adDIW[MN];
                                                    break;
                                            }
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Memnoic.opCC[MN],
                                                Parameter = Memnoic.adFC[MN] + "[" + par +
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
                                                    par = Memnoic.adLW[MN];
                                                    break;
                                                case 0x40:
                                                    par = Memnoic.adDBW[MN];
                                                    break;
                                                case 0x50:
                                                    par = Memnoic.adDIW[MN];
                                                    break;
                                            }
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Memnoic.opCC[MN],
                                                Parameter = Memnoic.adFB[MN] + "[" + par +
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
                                                Command = Memnoic.opCC[MN],
                                                Parameter = Memnoic.adFC[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5], BD[pos + 6], BD[pos + 7] };
                                            pos += 8;
                                        }
                                        break;
                                    case 0x72:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opUC[MN], Parameter = Memnoic.adFB[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x73:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Memnoic.opCC[MN],
                                                Parameter = Memnoic.adFB[MN] +
                                                     Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });

                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x74:
                                        {
                                            retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opUC[MN], Parameter = Memnoic.adSFC[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) });

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
                                            retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opUC[MN], Parameter = Memnoic.adSFB[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2)) });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x78:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Memnoic.opAUF[MN],
                                                Parameter = Memnoic.adDB[MN] +
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
                                                Command = Memnoic.opAUF[MN],
                                                Parameter = Memnoic.adDI[MN] +
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
                                            retVal.Add(new S7FunctionBlockRow() { Command = Memnoic.opTDB[MN] });
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
                                            cmd = Memnoic.opU[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opUN[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opO[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opON[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opX[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opXN[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opS[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opR[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opZUW[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opFP[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opFN[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opL[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xA8:
                                    case 0xb8:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Memnoic.opFR[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opLC[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opSA[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opSV[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opSE[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opSS[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opSI[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opZR[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opZV[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                            cmd = Memnoic.opT[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xCB:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Memnoic.opL[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "P##" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0xD0:
                                    case 0xD2:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Memnoic.opUC[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
                                                par = "#" + ParaList[(libnodave.getU16from(BD, pos + 2) / 2) - 1];
                                            else
                                                par = "unkown parameter (" + Convert.ToString(libnodave.getU16from(BD, pos + 2)) + ")";
                                            retVal.Add(new S7FunctionBlockRow() { Command = cmd, Parameter = par });

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
                                    case 0xD8:
                                        {
                                            string cmd = "", par = "";
                                            cmd = Memnoic.opAUF[MN];
                                            if (ParaList.Count >= (libnodave.getU16from(BD, pos + 2) / 2) - 1)
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
                                                Command = Memnoic.opU[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opUN[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opO[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opON[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opX[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opXN[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opL[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opFR[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opLC[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opSA[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opSV[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opSE[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opSS[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opSI[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opR[MN],
                                                Parameter = Memnoic.adT[MN] +
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
                                                Command = Memnoic.opU[MN],
                                                Parameter = Memnoic.adZ[MN] +
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
                                                Command = Memnoic.opUN[MN],
                                                Parameter = Memnoic.adZ[MN] +
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
                                                Command = Memnoic.opO[MN],
                                                Parameter = Memnoic.adZ[MN] +
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
                                                Command = Memnoic.opON[MN],
                                                Parameter = Memnoic.adZ[MN] +
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
                                                Command = Memnoic.opX[MN],
                                                Parameter = Memnoic.adZ[MN] +
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
                                                Command = Memnoic.opXN[MN],
                                                Parameter = Memnoic.adZ[MN] +
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
                                                Command = Memnoic.opFR[MN],
                                                Parameter = Memnoic.adZ[MN] +
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
                                                Command = Memnoic.opZR[MN],
                                                Parameter = Memnoic.adZ[MN] +
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
                                                Command = Memnoic.opS[MN],
                                                Parameter = Memnoic.adZ[MN] +
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
                                                Command = Memnoic.opZV[MN],
                                                Parameter = Memnoic.adZ[MN] +
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
                                                Command = Memnoic.opR[MN],
                                                Parameter = Memnoic.adZ[MN] +
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
                                Command = Memnoic.opUN[MN],
                                Parameter = Memnoic.adT[MN] + Convert.ToString(BD[pos + 1])
                            });
                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                            pos += 2;
                            break;
                        case 0xFD:
                            retVal.Add(new S7FunctionBlockRow()
                            {
                                Command = Memnoic.opON[MN],
                                Parameter = Memnoic.adT[MN] + Convert.ToString(BD[pos + 1])
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
                                                Command = Memnoic.opLAR1[MN],
                                                Parameter = Memnoic.adAR2[MN]
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
                                                Command = Memnoic.opPAR1[MN],
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
                                                               Command = Memnoic.opLAR1[MN],
                                                               Parameter = Helper.GetPointer(BD, pos + 2)
                                                           });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3], BD[pos + 4], BD[pos + 5] };
                                            pos += 6;
                                        }
                                        break;
                                    case 0x04:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Memnoic.opLAR1[MN]
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
                                                Command = Memnoic.opTAR1[MN]
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
                                                Command = Memnoic.opPAR1[MN]
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
                                                Command = Memnoic.opTAR[MN]
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
                                                Command = Memnoic.opTAR1[MN],
                                                Parameter = Memnoic.adAR2[MN]
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
                                                Command = Memnoic.opPAR2[MN],
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
                                                Command = Memnoic.opLAR2[MN],
                                                Parameter = Helper.GetPointer(BD,pos+2)
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
                                                Command = Memnoic.opLAR2[MN]
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
                                                Command = Memnoic.opTAR2[MN]
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
                                                Command = Memnoic.opPAR2[MN]
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
                                                Command = Memnoic.opLAR1[MN],
                                                Parameter = Memnoic.adMD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1], BD[pos + 2], BD[pos + 3] };
                                            pos += 4;
                                        }
                                        break;
                                    case 0x37:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Memnoic.opTAR1[MN],
                                                Parameter = Memnoic.adMD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                Command = Memnoic.opLAR2[MN],
                                                Parameter = Memnoic.adMD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                Command = Memnoic.opTAR2[MN],
                                                Parameter = Memnoic.adMD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                Command = Memnoic.opLAR1[MN],
                                                Parameter = Memnoic.adDBD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                Command = Memnoic.opTAR1[MN],
                                                Parameter = Memnoic.adDBD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                Command = Memnoic.opLAR2[MN],
                                                Parameter = Memnoic.adDBD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                Command = Memnoic.opTAR2[MN],
                                                Parameter = Memnoic.adDBD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                Command = Memnoic.opLAR1[MN],
                                                Parameter = Memnoic.adDID[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                Command = Memnoic.opTAR1[MN],
                                                Parameter = Memnoic.adDID[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                Command = Memnoic.opLAR2[MN],
                                                Parameter = Memnoic.adDID[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                Command = Memnoic.opTAR2[MN],
                                                Parameter = Memnoic.adDID[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                Command = Memnoic.opLAR1[MN],
                                                Parameter = Memnoic.adLD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                Command = Memnoic.opTAR1[MN],
                                                Parameter = Memnoic.adLD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                Command = Memnoic.opLAR2[MN],
                                                Parameter = Memnoic.adLD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                Command = Memnoic.opTAR2[MN],
                                                Parameter = Memnoic.adLD[MN] + Convert.ToString(libnodave.getU16from(BD, pos + 2))
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
                                                        Command = Memnoic.opSRD[MN],
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
                                                                   Command = Memnoic.opSRD[MN],
                                                                   Parameter = Convert.ToString(BD[pos + 1] - 0xC0)
                                                               });
                                                //Result = Result + Memnoic.opSRD[MN] + Convert.ToString(BD[pos + 1] - 0xC0);
                                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] {BD[pos], BD[pos + 1]};
                                                pos += 2;
                                                break;
                                            default:
                                                retVal.Add(new S7FunctionBlockRow() {Command = "err 99"});
                                                ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] {BD[pos], BD[pos + 1]};
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
                                                Command = Memnoic.opU[MN],
                                                Parameter = Memnoic.adOS[MN]
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
                                                Command = Memnoic.opUN[MN],
                                                Parameter = Memnoic.adOS[MN]
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
                                                Command = Memnoic.opO[MN],
                                                Parameter = Memnoic.adOS[MN]
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
                                                Command = Memnoic.opON[MN],
                                                Parameter = Memnoic.adOS[MN]
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
                                                Command = Memnoic.opX[MN],
                                                Parameter = Memnoic.adOS[MN]
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
                                                Command = Memnoic.opXN[MN],
                                                Parameter = Memnoic.adOS[MN]
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
                                                               Command = Memnoic.opSPS[MN],
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
                                                Command = Memnoic.opU[MN],
                                                Parameter = Memnoic.adOV[MN]
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
                                                Command = Memnoic.opUN[MN],
                                                Parameter = Memnoic.adOV[MN]
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
                                                Command = Memnoic.opO[MN],
                                                Parameter = Memnoic.adOV[MN]
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
                                                Command = Memnoic.opON[MN],
                                                Parameter = Memnoic.adOV[MN]
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
                                                Command = Memnoic.opX[MN],
                                                Parameter = Memnoic.adOV[MN]
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
                                                Command = Memnoic.opXN[MN],
                                                Parameter = Memnoic.adOV[MN]
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
                                                Command = Memnoic.opSPO[MN],
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
                                                Command = Memnoic.opU[MN],
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
                                                Command = Memnoic.opUN[MN],
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
                                                Command = Memnoic.opO[MN],
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
                                                Command = Memnoic.opON[MN],
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
                                                Command = Memnoic.opX[MN],
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
                                                Command = Memnoic.opXN[MN],
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
                                                Command = Memnoic.opSPP[MN],
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
                                                Command = Memnoic.opU[MN],
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
                                                Command = Memnoic.opUN[MN],
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
                                                Command = Memnoic.opO[MN],
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
                                                Command = Memnoic.opON[MN],
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
                                                Command = Memnoic.opX[MN],
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
                                                Command = Memnoic.opXN[MN],
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
                                                Command = Memnoic.opSPM[MN],
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
                                                Command = Memnoic.opU[MN],
                                                Parameter = Memnoic.adUO[MN]
                                            });
                                            ((S7FunctionBlockRow)retVal[retVal.Count - 1]).MC7 = new byte[] { BD[pos], BD[pos + 1] };
                                            pos += 2;
                                        }
                                        break;
                                    case 0x51:
                                        {
                                            retVal.Add(new S7FunctionBlockRow()
                                            {
                                                Command = Memnoic.opUN[MN],
                                                Parameter = Memnoic.adUO[MN]
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
                                                Command = Memnoic.opO[MN],
                                                Parameter = Memnoic.adUO[MN]
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
                                                Command = Memnoic.opON[MN],
                                                Parameter = Memnoic.adUO[MN]
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
                                                Command = Memnoic.opX[MN],
                                                Parameter = Memnoic.adUO[MN]
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
                                                Command = Memnoic.opXN[MN],
                                                Parameter = Memnoic.adUO[MN]
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
                                                Command = Memnoic.opSPU[MN],
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
                                                Command = Memnoic.opU[MN],
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
                                                Command = Memnoic.opUN[MN],
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
                                                Command = Memnoic.opO[MN],
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
                                                Command = Memnoic.opON[MN],
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
                                                Command = Memnoic.opX[MN],
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
                                                Command = Memnoic.opXN[MN],
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
                                                Command = Memnoic.opSPN[MN],
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
                                                Command = Memnoic.opSPBIN[MN],
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
                                                Command = Memnoic.opU[MN],
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
                                                Command = Memnoic.opUN[MN],
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
                                                Command = Memnoic.opO[MN],
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
                                                Command = Memnoic.opON[MN],
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
                                                Command = Memnoic.opX[MN],
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
                                                Command = Memnoic.opXN[MN],
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
                                                Command = Memnoic.opSPZ[MN],
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
                                                Command = Memnoic.opSPBNB[MN],
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
                                                Command = Memnoic.opU[MN],
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
                                                Command = Memnoic.opUN[MN],
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
                                                Command = Memnoic.opO[MN],
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
                                                Command = Memnoic.opON[MN],
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
                                                Command = Memnoic.opX[MN],
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
                                                Command = Memnoic.opXN[MN],
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
                                                Command = Memnoic.opSPPZ[MN],
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
                                                Command = Memnoic.opSPBN[MN],
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
                                                Command = Memnoic.opU[MN],
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
                                                Command = Memnoic.opUN[MN],
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
                                                Command = Memnoic.opO[MN],
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
                                                Command = Memnoic.opON[MN],
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
                                                Command = Memnoic.opX[MN],
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
                                                Command = Memnoic.opXN[MN],
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
                                                Command = Memnoic.opSPMZ[MN],
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
                                                Command = Memnoic.opSPBB[MN],
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
                                                Command = Memnoic.opU[MN],
                                                Parameter = Memnoic.adBIE[MN]
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
                                                Command = Memnoic.opUN[MN],
                                                Parameter = Memnoic.adBIE[MN]
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
                                                Command = Memnoic.opO[MN],
                                                Parameter = Memnoic.adBIE[MN]
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
                                                Command = Memnoic.opON[MN],
                                                Parameter = Memnoic.adBIE[MN]
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
                                                Command = Memnoic.opX[MN],
                                                Parameter = Memnoic.adBIE[MN]
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
                                                Command = Memnoic.opXN[MN],
                                                Parameter = Memnoic.adBIE[MN]
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
                                                Command = Memnoic.opSPBI[MN],
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
                                                Command = Memnoic.opSPB[MN],
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
                                                Command = Memnoic.opUNO[MN],
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
                                                Command = Memnoic.opONO[MN],
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
                                                Command = Memnoic.opXO[MN],
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
                                                Command = Memnoic.opXNO[MN],
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
                                                Command = Memnoic.opNOP[MN],
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
            if (prjBlkFld!=null)
            {
                if (prjBlkFld.SymbolTable!=null)
                {
                    foreach (S7FunctionBlockRow awlRow in retVal)
                    {
                        string para = awlRow.Parameter.Replace(" ", "");

                        awlRow.SymbolTableEntry = prjBlkFld.SymbolTable.GetEntryFromOperand(para);
                    }
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
                if (Helper.IsJump(tmp,MN))
                {
                    int jmpBytePos = 0;
                    
                    if (tmp.Command==Memnoic.opSPL[MN])
                        jmpBytePos = akBytePos + ((tmp.JumpWidth+1)*4);
                    else
                        jmpBytePos = akBytePos + (tmp.JumpWidth*2);

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
