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
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    internal static class Parameter
    {

        //PLCDataRow
        //Diconary ByteAddres, PLCDataRow

        //Create Extra Functions for GetValues and GetActualValues
        //This functions get the PLCDataRow Structure.


        internal static PLCDataRow GetFunctionParameterFromNumber(PLCDataRow parameters, int index)
        {
            if (parameters==null || parameters.Children==null)
                return null;
            int n = 0;
            int akIdx = index;
            while (n < parameters.Children.Count)
            {
                PLCDataRow tmp = parameters.Children[n];
                if (akIdx >= tmp.Children.Count)
                {
                    akIdx -= tmp.Children.Count;
                    n++;
                }
                else
                {
                    return tmp.Children[akIdx];
                }
            }
            return null;
        }

        internal static PLCDataRow GetInterfaceOrDBFromStep7ProjectString(string txt, ref List<String> ParaList, PLCBlockType blkTP, bool isInstanceDB, BlocksOfflineFolder myFld, PLCBlock myBlk)
        {
            PLCDataRow parameterRoot = new PLCDataRow("ROOTNODE", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterIN = new PLCDataRow("IN", PLCDataRowType.STRUCT,myBlk);
            PLCDataRow parameterOUT = new PLCDataRow("OUT", PLCDataRowType.STRUCT,myBlk);
            PLCDataRow parameterINOUT = new PLCDataRow("IN_OUT", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterSTAT = new PLCDataRow("STATIC", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterTEMP = new PLCDataRow("TEMP", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterRETVAL = new PLCDataRow("RET_VAL", PLCDataRowType.STRUCT, myBlk);

            PLCDataRow akDataRow = parameterRoot;

            parameterTEMP.isRootBlock = true;

            bool tempAdded = false;

            if (txt == null)
            {
                if (blkTP != PLCBlockType.DB)
                    parameterRoot.Add(parameterTEMP);
                return parameterRoot;
            }

            //Todo: read the complete DB from mc5 code first.
            //Read the containing UDTs
            //compare the UDTs with the Structs
            //if the UDTs and Structs are not Equal, marke the PLCDataRow as TimeStampConflict

            string[] rows = txt.Split(new string[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
            for (int n=0;n<rows.Length;n++) // (string row in rows)
            {
                string rowTr = rows[n].Replace("\0", "").Trim();
                switch (rowTr)
                {
                    
                    case "VAR_INPUT":
                        akDataRow = parameterIN;
                        parameterRoot.Add(parameterIN);
                        break;
                    case "VAR_OUTPUT":
                        akDataRow = parameterOUT;
                        parameterRoot.Add(parameterOUT);
                        break;
                    case "VAR_IN_OUT":
                        akDataRow = parameterINOUT;
                        parameterRoot.Add(parameterINOUT);
                        break;
                    case "VAR_TEMP":
                        akDataRow = parameterTEMP;
                        if (blkTP != PLCBlockType.DB)
                        {
                            tempAdded = true;
                            parameterRoot.Add(parameterTEMP);
                        }
                        break;
                    case "VAR": //Static Data on a FB
                        akDataRow = parameterSTAT;
                        parameterRoot.Add(parameterSTAT);
                        break;
                    case "END_STRUCT ;":
                        akDataRow = akDataRow.Parent;
                        break;
                    case "STRUCT":                    
                    case "END_VAR":   
                    case "":
                        break;
                    default:                        
                        
                        char oldChar = ' ';

                        List<Step7Attribute> Step7Attributes = new List<Step7Attribute>();

                        string tmpName = "";
                        string tmpAttributeName = "";
                        string tmpAttributeValue = "";
                        string tmpType = "";
                        string tmpComment = "";
                        string tmpValue = "";

                        int parseZustand = 0; //0=ParName, 1=AttributeName, 6=AfterAttributeName, 2=AttributeValue, 3=Type, 4=Value, 7=InnerValue (without '), 5=Comment


                        if (!rows[n].Contains("\t"))
                        {
                            if (rows.Length>n+1)
                            {
                                if (rowTr.Contains("//"))
                                {
                                    int pos = rowTr.IndexOf("//");
                                    rowTr = rowTr.Substring(0, pos) + " " + rows[n + 1].Trim() + rowTr.Substring(pos);
                                }
                                else
                                {
                                    rowTr += " " + rows[n + 1].Trim();
                                }
                                n++;
                            }
                        }

                        for (int j=0;j<rowTr.Length;j++)
                        {
                            char tmpChar = rowTr[j];

                            if (parseZustand == 0 && tmpChar == '{')
                                parseZustand = 1;
                            else if (parseZustand == 0 && tmpChar != ' ' && tmpChar != ':')
                                tmpName += tmpChar;
                            else if (parseZustand == 6 && tmpChar == '\'')
                                parseZustand = 2;
                            else if (parseZustand == 1 && tmpChar == ':' && rowTr[j + 1] == '=')
                            {
                                parseZustand = 6;
                                j++;
                            }
                            else if (parseZustand == 1 && tmpChar != ' ' && tmpChar != ':' && tmpChar != '=' && tmpChar != '}' && tmpChar != '\'' && tmpChar != ';')
                                tmpAttributeName += tmpChar;
                            else if (parseZustand == 1 && tmpChar == '}')
                                parseZustand = 0;
                            else if (parseZustand == 0 && tmpChar == ':')
                                parseZustand = 3;
                            else if (parseZustand == 2 && tmpChar == '$')
                            {
                                tmpAttributeValue += rowTr[j + 1];
                                j++;
                            }
                            else if (parseZustand == 2 && tmpChar == '\'')
                            {
                                parseZustand = 1;
                                Step7Attributes.Add(new Step7Attribute(tmpAttributeName, tmpAttributeValue));
                                tmpAttributeName = "";
                                tmpAttributeValue = "";
                            }
                            else if (parseZustand == 2)
                                tmpAttributeValue += tmpChar;
                            //else if (parseZustand == 3 && tmpChar == ':')
                            //    parseZustand = 2;
                            else if (parseZustand == 3 && tmpChar == ':' && rowTr[j + 1] == '=')
                            {
                                parseZustand = 4;
                                j++;
                            }
                            else if ((parseZustand == 3 || parseZustand == 4) && tmpChar == '/' && rowTr[j + 1] == '/')
                            {
                                parseZustand = 5;
                                j++;
                            }
                            else if (parseZustand == 4 && tmpChar == '\'')
                            {
                                tmpValue += tmpChar;
                                parseZustand = 7;
                            }
                            else if (parseZustand == 7 && tmpChar == '$')
                            {
                                tmpValue += rowTr[j + 1];
                                j++;
                            }
                            else if (parseZustand == 7 && tmpChar == '\'')
                            {
                                tmpValue += tmpChar;
                                parseZustand = 4;
                            }
                            else if (parseZustand == 3 && tmpChar != ';')
                                tmpType += tmpChar;
                            else if (parseZustand == 4 && tmpChar != ';' && tmpChar != ' ')
                                tmpValue += tmpChar;
                            else if (parseZustand == 7)
                                tmpValue += tmpChar;
                            else if (parseZustand == 5)
                                tmpComment += tmpChar;
                        }

                        tmpType = tmpType.Trim();




                        PLCDataRow addRW = new PLCDataRow(tmpName, PLCDataRowType.UNKNOWN, myBlk);
                   
                        if (tmpType.Contains("ARRAY  [") || tmpType.Contains("ARRAY [")  )
                        {                           
                            List<int> arrayStart = new List<int>();
                            List<int> arrayStop = new List<int>();

                            int pos1 = tmpType.IndexOf("[");
                            int pos2 = tmpType.IndexOf("]", pos1);
                            string[] arrays = tmpType.Substring(pos1 + 1, pos2 - pos1 - 2).Split(',');

                            foreach (string array in arrays)
                            {
                                string[] akar = array.Split(new string[] {".."}, StringSplitOptions.RemoveEmptyEntries);
                                arrayStart.Add(Convert.ToInt32(akar[0].Trim()));
                                arrayStop.Add(Convert.ToInt32(akar[1].Trim()));
                            }
                            
                            addRW.ArrayStart = arrayStart;
                            addRW.ArrayStop = arrayStop;
                            addRW.IsArray = true;
                            tmpType = tmpType.Substring(pos2 + 5);
                        }

                        if (tmpValue != "")
                            addRW.StartValue = tmpValue;

                        addRW.Comment = tmpComment.Replace("$'", "'").Replace("$$", "$");

                        if (Step7Attributes.Count > 0)
                            addRW.Attributes = Step7Attributes;

                        int akRowTypeNumber = 0;
                        if (tmpType.Contains("SFB"))
                        {
                            addRW.DataType = PLCDataRowType.SFB;
                            akRowTypeNumber = Convert.ToInt32(tmpType.Substring(4));

                            PLCFunctionBlock tmpBlk = ((PLCFunctionBlock)myFld.GetBlock("SFB" + akRowTypeNumber.ToString()));
                            if (tmpBlk != null && tmpBlk.Parameter != null && tmpBlk.Parameter.Children != null)
                                addRW.AddRange(tmpBlk.Parameter.Children);
                        }
                        else if (tmpType.Contains("UDT"))
                        {
                            addRW.DataType = PLCDataRowType.UDT;
                            akRowTypeNumber = Convert.ToInt32(tmpType.Substring(4));

                            PLCDataBlock tmpBlk = ((PLCDataBlock) myFld.GetBlock("UDT" + akRowTypeNumber.ToString()));
                            if (tmpBlk != null && tmpBlk.Structure != null && tmpBlk.Structure.Children != null)
                                addRW.AddRange(tmpBlk.Structure.Children);
                            
                        }
                        else if (tmpType.Contains("BLOCK_FB"))
                        {
                            addRW.DataType = PLCDataRowType.BLOCK_FB;
                            //akRowTypeNumber = Convert.ToInt32(tmpType.Substring(3));

                            //PLCFunctionBlock tmpBlk = ((PLCFunctionBlock)myFld.GetBlock("FB" + akRowTypeNumber.ToString()));
                            //if (tmpBlk != null && tmpBlk.Parameter != null && tmpBlk.Parameter.Children != null)
                            //    addRW.AddRange(tmpBlk.Parameter.Children);
                        }
                        else if (tmpType.Contains("FB"))
                        {
                            addRW.DataType = PLCDataRowType.FB;
                            akRowTypeNumber = Convert.ToInt32(tmpType.Substring(3));

                            PLCFunctionBlock tmpBlk = ((PLCFunctionBlock)myFld.GetBlock("FB" + akRowTypeNumber.ToString()));
                            if (tmpBlk != null && tmpBlk.Parameter != null && tmpBlk.Parameter.Children != null)
                                addRW.AddRange(tmpBlk.Parameter.Children);
                        }
                        else if (tmpType.Contains("STRING"))
                        {
                            addRW.DataType = PLCDataRowType.STRING;
                            int pos1 = tmpType.IndexOf("[");
                            int pos2 = tmpType.IndexOf("]", pos1);
                            addRW.StringSize = Convert.ToInt32(tmpType.Substring(pos1 + 1, pos2 - pos1 - 2));
                        }
                        else
                            addRW.DataType = (PLCDataRowType)Enum.Parse(typeof (PLCDataRowType), tmpType.ToUpper());
                       
                        addRW.DataTypeBlockNumber = akRowTypeNumber;
                       
                       
                        akDataRow.Add(addRW);
                        ParaList.Add(tmpName);

                        if (addRW.DataType==PLCDataRowType.STRUCT)
                            akDataRow = addRW;

                        break;
                        /* 
                         * Attributname kann nicht ' und } enthalten!
                         * In Attribt Values
                         * $$ zum ausmaskieren von $
                         * $' zum ausmaskieren von '
                         * 
                         * Beispielzeilen....
                            // "ID { S7_co := 'agag'; S7_server1 := 'connection' }: INT ;\t// Connection ID 1..16"
                            // "LADDR { S7_co := 'agag'; S7_server1 := 'connection' }: WORD ;\t// Module address in hardware configuration"
                            // "RECV : ANY ;\t// Buffer for received data"
                            // "NDR : BOOL ;\t// Indicates whether new data were received"
                            // "aa { ff := '//ghghf}' }: BOOL ;"
                            // "bb : INT  := 8888;"
                            // "P_1001 : ARRAY  [0 .. 3 ] OF CHAR  := '1', '0', '0', '1';"
                            // "aa : STRING  [20 ] := 'deewedw';"
                            // "aa { dsfs := '' }: BOOL ;"

                            //"res : ARRAY  [1 .. 121 ] OF STRUCT"
                            //      "P_AKL1 : ARRAY  [0 .. 3 ] OF CHAR  := 'A', 'K', 'L', '1';"
                        */                       
                }
            }
            if (blkTP != PLCBlockType.DB && tempAdded == false)
            {               
                parameterRoot.Add(parameterTEMP);
            }
            return parameterRoot;
        }


        //
        internal static PLCDataRow GetInterface(int Start, int Count, int ValStart, byte[] BD, ref List<String> ParaList, DataTypes.PLCBlockType blkTP, int DB_Actual_Values_Start, bool isInstanceDB, PLCBlock myBlk)        
        {
            PLCDataRow parameterRoot = new PLCDataRow("ROOTNODE", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterIN = new PLCDataRow("IN", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterOUT = new PLCDataRow("OUT", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterINOUT = new PLCDataRow("IN_OUT", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterSTAT = new PLCDataRow("STATIC", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterTEMP = new PLCDataRow("TEMP", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterRETVAL = new PLCDataRow("RET_VAL", PLCDataRowType.STRUCT, myBlk);


            parameterRoot.Add(parameterIN);
            parameterRoot.Add(parameterOUT);
            parameterRoot.Add(parameterINOUT);
            if (blkTP == DataTypes.PLCBlockType.FB || (blkTP == DataTypes.PLCBlockType.DB && isInstanceDB))
                parameterRoot.Add(parameterSTAT);
            if (blkTP != DataTypes.PLCBlockType.DB)
                parameterRoot.Add(parameterTEMP);
            parameterRoot.Add(parameterRETVAL);
            parameterRoot.ReadOnly = true;

            if (blkTP == DataTypes.PLCBlockType.DB && !isInstanceDB)
                parameterRoot = parameterSTAT;

            int INp = 0;
            int OUTp = 0;
            int IN_OUTp = 0;
            int STATp = 0;
            int TEMPp = 0;
            int StackNr = 1;


            int pos = Start + 4;
            int Valpos = ValStart;

            PLCDataRow akParameter = parameterRoot;

            ParaList.Clear();
           
            while (pos <= (Start + Count))
            {
                string startVal;
                if (Helper.IsWithStartVal(BD[pos + 1]))
                {
                    if (BD[pos] != 0x10) //Datentyp == Array...
                        startVal = GetVarTypeVal(BD[pos], BD, ref Valpos);
                    else
                    {
                        Valpos = Valpos + 6;
                        startVal = GetVarTypeVal(BD[pos + 3 + (BD[pos + 2] * 4)], BD, ref Valpos);
                    }
                }
                else
                    startVal = "";
                switch (BD[pos + 1])
                {
                    case 0x01:
                    case 0x09:
                        {
                            GetVarTypeEN(parameterIN, startVal, BD[pos], false, false, "IN" + Convert.ToString(INp), BD, ref pos, ref ParaList, ref StackNr, "IN", ref INp, ref Valpos,myBlk);
                        }
                        break;
                    case 0x02:
                    case 0x0A:
                        {
                            GetVarTypeEN(parameterOUT, startVal, BD[pos], false, false, "OUT" + Convert.ToString(OUTp), BD, ref pos, ref ParaList, ref StackNr, "OUT", ref OUTp, ref Valpos,myBlk);
                        }
                        break;
                    case 0x03:
                    case 0x0b:
                        {
                            GetVarTypeEN(parameterINOUT, startVal, BD[pos], false, false, "IN_OUT" + Convert.ToString(IN_OUTp), BD, ref pos, ref ParaList, ref StackNr, "IN_OUT", ref IN_OUTp, ref Valpos, myBlk);
                        }
                        break;
                    case 0x04:
                    case 0x0C:
                        {
                            GetVarTypeEN(parameterSTAT, startVal, BD[pos], false, false, "STAT" + Convert.ToString(STATp), BD, ref pos, ref ParaList, ref StackNr, "STAT", ref STATp, ref Valpos, myBlk);
                        }
                        break;
                    case 0x05:
                        {
                            GetVarTypeEN(parameterTEMP, startVal, BD[pos], false, false, "TEMP" + Convert.ToString(TEMPp), BD, ref pos, ref ParaList, ref StackNr, "TEMP", ref TEMPp, ref Valpos, myBlk);
                        }
                        break;
                    case 0x06:
                        {
                            int tmp = 0;
                            GetVarTypeEN(parameterRETVAL, startVal, BD[pos], false, false, "RET_VAL", BD, ref pos, ref ParaList, ref StackNr, "RET_VAL", ref tmp, ref Valpos, myBlk);
                        }
                        break;
                    /*default:
                RETURNIntf = RETURNIntf + Convert.ToString(pos) + " UNKNOWN: " +
                             Convert.ToString(BD[pos + 1]) + " " + Convert.ToString(BD[pos]) + startVal +
                             "\r\n";
                break;*/
                }
                pos += 2;
            }
            return parameterRoot;
        }

        internal static PLCDataRow GetInterface(int Start, int Count, byte[] BD, DataTypes.PLCBlockType blkTP, bool isInstanceDB, PLCBlock myBlk)        
        {
            PLCDataRow parameterRoot = new PLCDataRow("ROOTNODE", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterIN = new PLCDataRow("IN", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterOUT = new PLCDataRow("OUT", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterINOUT = new PLCDataRow("IN_OUT", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterSTAT = new PLCDataRow("STATIC", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterTEMP = new PLCDataRow("TEMP", PLCDataRowType.STRUCT, myBlk);
            PLCDataRow parameterRETVAL = new PLCDataRow("RET_VAL", PLCDataRowType.STRUCT, myBlk);


            parameterRoot.Add(parameterIN);
            parameterRoot.Add(parameterOUT);
            parameterRoot.Add(parameterINOUT);
            if (blkTP == DataTypes.PLCBlockType.FB || (blkTP == DataTypes.PLCBlockType.DB && isInstanceDB))
                parameterRoot.Add(parameterSTAT);
            if (blkTP != DataTypes.PLCBlockType.DB)
                parameterRoot.Add(parameterTEMP);
            parameterRoot.Add(parameterRETVAL);
            parameterRoot.ReadOnly = true;

            if (blkTP == DataTypes.PLCBlockType.DB && !isInstanceDB)
                parameterRoot = parameterSTAT;

            //PLCDataRowsInterface retVal = new PLCDataRowsInterface();
            int INcnt = 0;
            int OUTcnt = 0;
            int IN_OUTcnt = 0;
            int STATcnt = 0;
            int TEMPcnt = 0;
            int StackNr = 1;
           

            int pos = Start + 4;
            string parNm = "";

            PLCDataRow akParameter = parameterRoot;


            while (pos <= (Start + Count))
            {               
                switch (BD[pos + 1])
                {
                    case 0x01:
                    case 0x09: //with start val
                        akParameter = parameterIN;
                        parNm = "IN";
                        break;
                    case 0x02:
                    case 0x0A: //with start val
                        akParameter = parameterOUT;
                        parNm = "OUT";
                        break;
                    case 0x03:
                    case 0x0b: //with start val
                        akParameter = parameterINOUT;
                        parNm = "IN_OUT";
                        break;
                    case 0x04:
                    case 0x0C: //with start val
                        akParameter = parameterSTAT;
                        parNm = "STAT";
                        break;
                    case 0x05:
                        akParameter = parameterTEMP;
                        parNm = "TEMP";
                        break;
                    case 0x06:
                        akParameter = parameterRETVAL;
                        parNm = "RET_VAL";
                        break;
                }


                pos += 2;
            }
            return parameterRoot;
        }

        //internal PLCDataRow GetInterfaceSubrows(PLCDataRow currRow)


        internal static void GetVarTypeEN(PLCDataRow currPar, string startVal, byte b, bool Struct, bool Arry, string VarName, byte[] BD, ref int pos, ref List<string> ParaList, ref int StackNr, string VarNamePrefix, ref int VarCounter, ref int Valpos, PLCBlock myBlk)
        {
            int i, max, dim;

            PLCDataRowType Result = PLCDataRowType.BOOL;

            switch (b)
            {
                case 0x01:
                    Result = PLCDataRowType.BOOL;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x02:
                    Result = PLCDataRowType.BYTE;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x03:
                    Result = PLCDataRowType.CHAR;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x04:
                    Result = PLCDataRowType.WORD;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x05:
                    Result = PLCDataRowType.INT;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x06:
                    Result = PLCDataRowType.DWORD;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x07:
                    Result = PLCDataRowType.DINT;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x08:
                    Result = PLCDataRowType.REAL;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x09:
                    Result = PLCDataRowType.DATE;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x0A:
                    Result = PLCDataRowType.TIME_OF_DAY;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x0b:
                    Result = PLCDataRowType.TIME;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x0C:
                    Result = PLCDataRowType.S5TIME;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x0E:
                    Result = PLCDataRowType.DATE_AND_TIME;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;

                case 0x10: //Array...
                    {
                        dim = BD[pos + 2];
                        List<int> arrStart = new List<int>();
                        List<int> arrStop = new List<int>();

                        for (i = 0; i <= dim - 1; i++)
                        {
                            arrStart.Add(BitConverter.ToInt16(BD, pos + 3 + (i * 4)));
                            arrStop.Add(BitConverter.ToInt16(BD, pos + 5 + (i * 4)));                         
                        }
                        GetVarTypeEN(currPar, "", BD[pos + 3 + (dim * 4)], true, true, VarName, BD, ref pos, ref ParaList,
                                     ref StackNr, VarNamePrefix, ref VarCounter, ref Valpos, myBlk);
                        currPar.Children[currPar.Children.Count - 1].ArrayStart = arrStart;
                        currPar.Children[currPar.Children.Count - 1].ArrayStop = arrStop;
                        currPar.Children[currPar.Children.Count - 1].IsArray = true;
                        pos += 3 + (dim * 4);

                    } break;
                case 0x11: //Struct
                    {
                        if (Arry) pos += 7;
                        Result = PLCDataRowType.STRUCT;
                        var akPar = new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal };
                        currPar.Add(akPar);
                        VarCounter++;
                        max = BD[pos + 2] - 1;
                        for (i = 0; i <= max; i++)
                        {



                            if ((BD[pos + 3] == 0x11) || (BD[pos + 3] == 0x10))
                            {
                                pos += 3;


                                if (Helper.IsWithStartVal(BD[pos + 1]))
                                {
                                    if (BD[pos] != 0x10) //Datentyp == Array...
                                        startVal = GetVarTypeVal(BD[pos], BD, ref Valpos);
                                    else
                                    {
                                        Valpos = Valpos + 6;
                                        startVal = GetVarTypeVal(BD[pos + 3 + (BD[pos + 2] * 4)], BD, ref Valpos);
                                    }
                                }
                                else
                                    startVal = "";


                                GetVarTypeEN(akPar, startVal, BD[pos], true, false,
                                             VarName + "." + VarNamePrefix + VarCounter.ToString(), BD, ref pos,
                                             ref ParaList, ref StackNr, VarNamePrefix, ref VarCounter, ref Valpos, myBlk);
                                pos -= 3;
                            }
                            else
                            {
                                if (Helper.IsWithStartVal(BD[pos + 4]))
                                {
                                    if (BD[pos] != 0x10) //Datentyp == Array...
                                        startVal = GetVarTypeVal(BD[pos+3], BD, ref Valpos);
                                    else
                                    {
                                        Valpos = Valpos + 6;
                                        startVal = GetVarTypeVal(BD[pos + 6 + (BD[pos + 2]*4)], BD, ref Valpos);
                                    }
                                }
                                else
                                    startVal = "";

                                GetVarTypeEN(akPar, startVal, BD[pos + 3], true, false, VarName + "." + VarNamePrefix + VarCounter.ToString(), BD, ref pos, ref ParaList, ref StackNr, VarNamePrefix, ref VarCounter, ref Valpos, myBlk);
                            }
                            pos += 2;
                        }
                        if (Arry) pos -= 7; pos += 1;
                    } break;

                case 0x13:
                    {
                        Result = PLCDataRowType.STRING;
                        currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                        if (Arry)
                            currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal, StringSize = BD[pos + 9] });
                        else
                            currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal, StringSize = BD[pos + 2] });
                        pos += 1;
                        VarCounter++;
                    }
                    break;
                case 0x14:
                    Result = PLCDataRowType.POINTER;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x16:
                    Result = PLCDataRowType.ANY;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x17:
                    Result = PLCDataRowType.BLOCK_FB;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x18:
                    Result = PLCDataRowType.BLOCK_FC;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x19:
                    Result = PLCDataRowType.BLOCK_DB;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x1A:
                    Result = PLCDataRowType.BLOCK_SDB;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x1C:
                    Result = PLCDataRowType.COUNTER;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                case 0x1D:
                    Result = PLCDataRowType.TIMER;
                    currPar.Add(new PLCDataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal });
                    VarCounter++;
                    break;
                //default: Result = "UNKNOWN (" + Convert.ToString(b) + ")"; break;
            }
            //if (!Struct || Arry)
            {
                ParaList.Add(VarName);
                //Result = Result + "(" + Convert.ToString(StackNr * 2) + ")";
                StackNr = StackNr + 1;
            }

            //return Result;
        }

        internal static string GetVarTypeVal(byte b, byte[] BD, ref int Valpos)
        {

            string Result;
            switch (b)
            {
                case 0x01:
                    { // 'BOOL';
                        if (BD[Valpos] == 0)
                            Result = "FALSE";
                        else
                            Result = "TRUE";
                        Valpos = Valpos + 1;
                    } break;
                case 0x02:
                    { // 'BYTE';
                        Result = "B#16#" + BD[Valpos].ToString("X");
                        Valpos = Valpos + 1;
                    } break;
                case 0x03:
                    { // 'CHAR';
                        Result = Helper.GetChar(BD[Valpos]);
                        Valpos = Valpos + 1;
                    } break;
                case 0x04:
                    { // 'WORD';
                        Result = "W#16#" + (BD[Valpos + 1]).ToString("X") + (BD[Valpos]).ToString("X");
                        Valpos = Valpos + 2;
                    } break;
                case 0x05:
                    { // 'INT';
                        Result = Convert.ToString(BitConverter.ToInt16(BD, Valpos));// Helper.GetInt(BD[Valpos + 1], BD[Valpos]));
                        Valpos = Valpos + 2;
                    } break;
                case 0x06:
                    { // 'DWORD';
                        Result = "DW#16#" + (BD[Valpos + 3]).ToString("X") + (BD[Valpos + 2]).ToString("X") + (BD[Valpos + 1]).ToString("X") + (BD[Valpos]).ToString("X");
                        Valpos = Valpos + 4;
                    } break;
                case 0x07:
                    { // 'DINT';
                        //Result = "L#" + Convert.ToString(libnodave.getS32from(BD, Valpos));
                        Result = "L#" + Convert.ToString(BitConverter.ToInt32(BD, Valpos));
                        //Result = "L#" + Convert.ToString(Helper.GetDInt(BD[Valpos + 3], BD[Valpos + 2], BD[Valpos + 1], BD[Valpos]));
                        Valpos = Valpos + 4;
                    } break;
                case 0x08:
                    { // 'REAL';
                        Result = BitConverter.ToDouble(BD, Valpos).ToString("0.000000e+000"); //libnodave.getFloatfrom(BD, Valpos).ToString("0.000000e+000"); //Helper.GetReal(BD[Valpos + 3], BD[Valpos + 2], BD[Valpos + 1], BD[Valpos]);
                        Valpos = Valpos + 4;
                    } break;
                case 0x09:
                    { // 'DATE';
                        Result = Helper.GetDate(BD[Valpos + 1], BD[Valpos]);
                        Valpos = Valpos + 2;
                    } break;
                case 0x0A:
                    { // 'TIME_OF_DAY';
                        Result = Helper.GetTOD(BD, Valpos);
                        //Result = Helper.GetTOD(BD[Valpos + 3], BD[Valpos + 2], BD[Valpos + 1], BD[Valpos]);
                        Valpos = Valpos + 4;
                    } break;
                case 0x0b:
                    { // 'TIME';
                        Result = Helper.GetDTime(BD, Valpos);
                        //Result = Helper.GetDTime(BD[Valpos + 3], BD[Valpos + 2], BD[Valpos + 1], BD[Valpos]);
                        Valpos = Valpos + 4;
                    } break;
                case 0x0C:
                    { // 'S5TIME';
                        Result = Helper.GetS5Time(BD[Valpos + 1], BD[Valpos]);
                        Valpos = Valpos + 2;
                    } break;
                case 0x0E:
                    { // 'DATE_AND_TIME';                        
                        Result = Helper.GetDaT(libnodave.getU32from(BD, Valpos), libnodave.getU32from(BD, Valpos + 4));
                        //Result = Helper.GetDaT(Helper.GetDWord(BD[Valpos], BD[Valpos + 1], BD[Valpos + 2], BD[Valpos + 3]), Helper.GetDWord(BD[Valpos + 4], BD[Valpos + 5], BD[Valpos + 6], BD[Valpos + 7]));
                        Valpos = Valpos + 8;
                    } break;
                case 0x13:
                    { // 'STRING';

                        Result = Helper.GetS7String(Valpos, -1, BD);
                        Valpos = Valpos + BD[Valpos + 1] + 2;
                    } break;
                default: Result = "UNKNOWN (" + Convert.ToString(b) + ")"; break;
            }

            return Result;
        }
    }
}