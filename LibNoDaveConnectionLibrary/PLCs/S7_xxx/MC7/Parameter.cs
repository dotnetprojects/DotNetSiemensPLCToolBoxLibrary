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
using System.Linq;

using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    internal static class Parameter
    {

        //PLCDataRow
        //Diconary ByteAddres, PLCDataRow

        //Create Extra Functions for GetValues and GetActualValues
        //This functions get the PLCDataRow Structure.


        internal static S7DataRow GetFunctionParameterFromNumber(S7DataRow parameters, int index)
        {
            if (parameters==null || parameters.Children==null)
                return null;
            int n = 0;
            int akIdx = index;
            while (n < parameters.Children.Count)
            {
                S7DataRow tmp = ((S7DataRow)parameters.Children[n]);
                if (akIdx >= tmp.Children.Count)
                {
                    akIdx -= tmp.Children.Count;
                    n++;
                }
                else
                {
                    return ((S7DataRow)tmp.Children[akIdx]);
                }
            }
            return null;
        }

        /// <summary>
        /// Compares the structure of two Interface declarations and returns true if they are compatible and false if not
        /// They are compatible if the Structure and Datatypes match up. Datarow names are ignored, since the interface remain
        /// compatible as long as the general structure matches up.
        /// Also Actual values are ignores
        /// </summary>
        /// <param name="Block1">The first interface to compare</param>
        /// <param name="Block2">The second interface to campare</param>
        /// <returns></returns>
        internal static bool IsInterfaceCompatible (IDataRow Block1, IDataRow Block2)
        {
            //Compare basic configuration
            //if (Block1.BlockAddress != Block2.BlockAddress) return false; //The adress must be the same
            if (Block1.DataType != Block2.DataType) return false; //Datatypes must match up

            //Check if any of the compared blocks contains an "TEMP" section. This section must be ignored
            //this assumes that "TEMP" sections are always the last ones
            //Todo! implement better solution without looping 3 times through the children
            int Childcount1 = Block1.Children.Count;
            foreach (var Child in Block1.Children ) { if(Child.Name == "TEMP") Childcount1 -= 1; }
            int Childcount2 = Block2.Children.Count;
            foreach (var Child in Block2.Children) { if (Child.Name == "TEMP") Childcount2 -= 1; }

            if (Childcount1 != Childcount2) return false; //if the blocks have different amounts of children, then they cant be compatible

            for (int i = 0; i < Childcount1; i++)
            {
                if (!IsInterfaceCompatible(Block1.Children[i], Block2.Children[i])) return false; //Recurse through children
            }

            //if none of the Children was incompatible, then we must be comptible ...
            return true;
        }

        /// <summary>
        /// Parses the interface from an Step7 Source code as stored in Step7 Project Files
        /// </summary>
        /// <param name="txt">The Step7 Code to be parsed</param>
        /// <param name="ParaList">An list of Parameters that where found in the Step7 Declaration</param>
        /// <param name="blkTP">The block type that is beeing Parsed</param>
        /// <param name="isInstanceDB">Indicates if an block is an Instance DB</param>
        /// <param name="myFld">The BlocksOffline Folder where the parsed block code belongs to</param>
        /// <param name="myBlk">The Block where the Parsed Step7 code belongs to</param>
        /// <param name="actualValues">the current values of the DB, if it is an DB</param>
        /// <returns></returns>
        internal static S7DataRow GetInterfaceOrDBFromStep7ProjectString(string txt, ref List<String> ParaList, PLCBlockType blkTP, bool isInstanceDB, BlocksOfflineFolder myFld, S7Block myBlk, byte[] actualValues = null)
        {
            S7DataRow parameterRoot = new S7DataRow("ROOTNODE", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterRootWithoutTemp = new S7DataRow("ROOTNODE", S7DataRowType.STRUCT, myBlk);

            if (myBlk is S7FunctionBlock)
            {
                (myBlk as S7FunctionBlock).ParameterWithoutTemp = parameterRootWithoutTemp;
            }

            S7DataRow parameterIN = new S7DataRow("IN", S7DataRowType.STRUCT,myBlk);
            S7DataRow parameterOUT = new S7DataRow("OUT", S7DataRowType.STRUCT,myBlk);
            S7DataRow parameterINOUT = new S7DataRow("IN_OUT", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterSTAT = new S7DataRow("STATIC", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterTEMP = new S7DataRow("TEMP", S7DataRowType.STRUCT, myBlk);
            //S7DataRow parameterRETVAL = new S7DataRow("RET_VAL", S7DataRowType.STRUCT, myBlk);

            S7DataRow akDataRow = parameterRoot;

            parameterIN.isRootBlock = true;
            parameterOUT.isRootBlock = true;
            parameterINOUT.isRootBlock = true;
            parameterINOUT.isInOut = true;
            parameterSTAT.isRootBlock = true;
            parameterTEMP.isRootBlock = true;

            bool tempAdded = false;

            int Valpos = 0;

            if (txt == null)
            {
                if (blkTP != PLCBlockType.DB)
                    parameterRoot.Add(parameterTEMP);
                return parameterRoot;
            }

            //Todo: read the complete DB from mc5 code first, Read the containing UDTs, compare the UDTs with the Structs, if the UDTs and Structs are not Equal, marke the PLCDataRow as TimeStampConflict
            
            string[] rows = txt.Split(new string[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);

            S7DataRow lastrow = null;


            Dictionary<string, S7Block> blkHelpers = new Dictionary<string, S7Block>();

            for (int n=0;n<rows.Length;n++) // (string row in rows)
            {
                string rowTr = rows[n].Replace("\0", "").Trim();
                int poscm = rowTr.IndexOf("\t//");
                if (poscm <= 0)
                    poscm = rowTr.Length;

                string switchrow = rowTr.Substring(0,poscm);
                string commnew = "";
                if (poscm != rowTr.Length)
                    commnew = rowTr.Substring(poscm + 1);

                if (rowTr.StartsWith("//"))
                {
                    if (lastrow != null)
                        lastrow.Comment += rowTr;
                    else
                    {
                        if (commnew != null)
                            if (string.IsNullOrEmpty(akDataRow.Comment))
                                akDataRow.Comment += rowTr;
                            else
                                akDataRow.Comment += "\r\n" + rowTr;
                    }
                }
                else
                {
                    switch (switchrow.Trim())
                    {

                        case "VAR_INPUT":
                            akDataRow = parameterIN;
                            akDataRow.Comment = commnew;
                            parameterRoot.Add(parameterIN);
                            parameterRootWithoutTemp.Add(parameterIN);                            
                            break;
                        case "VAR_OUTPUT":
                            akDataRow = parameterOUT;
                            akDataRow.Comment = commnew;
                            parameterRoot.Add(parameterOUT);
                            parameterRootWithoutTemp.Add(parameterOUT);
                            break;
                        case "VAR_IN_OUT":
                            akDataRow = parameterINOUT;
                            akDataRow.Comment = commnew;

                            parameterRoot.Add(parameterINOUT);
                            parameterRootWithoutTemp.Add(parameterINOUT);
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
                            parameterRootWithoutTemp.Add(parameterSTAT);
                            break;
                        case "END_STRUCT;":
                        case "END_STRUCT ;":
                            akDataRow = ((S7DataRow)akDataRow.Parent);
                            break;
                        case "STRUCT":
                        case "END_VAR":
                        case "":
                            if (commnew != null)
                                if (string.IsNullOrEmpty(akDataRow.Comment))
                                    akDataRow.Comment += commnew;
                                else
                                    akDataRow.Comment += "\r\n" + commnew;
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

                            var p1 = rows[n].IndexOf(" OF ");
                            int p2 = 0, p3 = 0;
                            if (p1 > 0)
                            {
                                p2 = rows[n].IndexOf(";", 0);
                                p3 = rows[n].IndexOf("//", 0);
                            }

                            var isArray = (((p1 < p2) || p2 < 0) && ((p1 < p3) || p3 < 0));
                            if (rows[n].Contains("ARRAY") && rows[n].Contains(" OF ") && (isArray && !rows[n].Contains("\t")))
                            {
                                if (rows.Length > n + 1)
                                {
                                    if (rowTr.Contains("//"))
                                    {
                                        int pos = rowTr.IndexOf("//");
                                        rowTr = rowTr.Substring(0, pos) + " " + rows[n + 1].Trim() + rowTr.Substring(pos);
                                    }
                                    else if (rowTr.Contains("OF STRUCT"))
                                    {
                                    }
                                    else if (rowTr[rowTr.Length - 1] != ';')
                                    {
                                        rowTr += " " + rows[n + 1].Trim();
                                    }
                                    n++;
                                }
                            }

                            for (int j = 0; j < rowTr.Length; j++)
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

                            tmpType = tmpType.Trim().ToUpper();

                            if (tmpType.EndsWith("OF"))
                            {
                                n++;
                                tmpType += " " + rows[n].Replace("\t", "").Replace("\r", "");
                            }
                            S7DataRow addRW = new S7DataRow(tmpName, S7DataRowType.UNKNOWN, myBlk);
                            lastrow = addRW;

                            if (tmpType.Replace(" ","").Contains("ARRAY["))
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

                            addRW.Comment = tmpComment.Replace("$'", "'").Replace("$$", "$");

                            if (Step7Attributes.Count > 0)
                                addRW.Attributes = Step7Attributes;

                            int akRowTypeNumber = 0;
                            if (tmpType.Contains("SFB"))
                            {
                                addRW.DataType = S7DataRowType.SFB;
                                akRowTypeNumber = Convert.ToInt32(tmpType.Substring(4));

                                string blkDesc = "SFB" + akRowTypeNumber.ToString();
                                S7FunctionBlock tmpBlk;
                                if (blkHelpers.ContainsKey(blkDesc))
                                    tmpBlk = (S7FunctionBlock)blkHelpers[blkDesc];
                                else
                                {
                                    tmpBlk = ((S7FunctionBlock)myFld.GetBlock(blkDesc));
                                    blkHelpers.Add(blkDesc, tmpBlk);
                                }
                                
                                if (tmpBlk != null && tmpBlk.Parameter != null && tmpBlk.Parameter.Children != null)
                                    addRW.AddRange(tmpBlk.ParameterWithoutTemp.DeepCopy().Children);                                    
                            }
                            else if (tmpType.Contains("UDT"))
                            {
                                addRW.DataType = S7DataRowType.UDT;
                                akRowTypeNumber = Convert.ToInt32(tmpType.Substring(4));

                                string blkDesc = "UDT" + akRowTypeNumber.ToString();
                                S7DataBlock tmpBlk;
                                if (blkHelpers.ContainsKey(blkDesc))
                                    tmpBlk = (S7DataBlock)blkHelpers[blkDesc];
                                else
                                {
                                    tmpBlk = ((S7DataBlock)myFld.GetBlock(blkDesc));
                                    blkHelpers.Add(blkDesc, tmpBlk);
                                }

                                if (tmpBlk != null && tmpBlk.Structure != null && tmpBlk.Structure.Children != null)
                                    addRW.AddRange(((S7DataRow)tmpBlk.Structure).DeepCopy().Children);

                            }
                            else if (tmpType.Contains("BLOCK_FB"))
                            {
                                addRW.DataType = S7DataRowType.BLOCK_FB;
                                //akRowTypeNumber = Convert.ToInt32(tmpType.Substring(3));

                                //PLCFunctionBlock tmpBlk = ((PLCFunctionBlock)myFld.GetBlock("FB" + akRowTypeNumber.ToString()));
                                //if (tmpBlk != null && tmpBlk.Parameter != null && tmpBlk.Parameter.Children != null)
                                //    addRW.AddRange(tmpBlk.Parameter.Children);
                            }
                            else if (tmpType.Contains("FB"))
                            {
                                addRW.DataType = S7DataRowType.FB;
                                akRowTypeNumber = Convert.ToInt32(tmpType.Substring(3));

                                string blkDesc = "FB" + akRowTypeNumber.ToString();
                                S7FunctionBlock tmpBlk;
                                if (blkHelpers.ContainsKey(blkDesc))
                                    tmpBlk = (S7FunctionBlock) blkHelpers[blkDesc];
                                else
                                {
                                    tmpBlk = ((S7FunctionBlock)myFld.GetBlock(blkDesc));
                                    blkHelpers.Add(blkDesc, tmpBlk);
                                }

                                if (tmpBlk != null && tmpBlk.Parameter != null && tmpBlk.Parameter.Children != null)
                                    addRW.AddRange(tmpBlk.ParameterWithoutTemp.DeepCopy().Children);
                            }
                            else if (tmpType.Contains("STRING"))
                            {
                                addRW.DataType = S7DataRowType.STRING;
                                int pos1 = tmpType.IndexOf("[");
                                int pos2 = tmpType.IndexOf("]", pos1);
                                addRW.StringSize = Convert.ToInt32(tmpType.Substring(pos1 + 1, pos2 - pos1 - 2));
                            }
                            else
                                addRW.DataType = (S7DataRowType)Enum.Parse(typeof(S7DataRowType), tmpType.Replace("\0", "").Replace("\t", "").Replace(";", "").Trim().ToUpper());

                            addRW.DataTypeBlockNumber = akRowTypeNumber;

                            if (tmpValue != "")
                            {
                                //Todo: Startvalues bei arrays...
                                //Mehrere Values...
                                //TRUE,6(FALSE),TRUE,TRUE,7(FALSE)
                                if (addRW.IsArray)
                                {
                                    addRW.StartValue = tmpValue;
                                }
                                else
                                {
                                    addRW.StartValue = Helper.StringValueToObject(tmpValue, addRW.DataType);
                                }
                            }
                            else
                            {
                                if (!addRW.IsArray)
                                    addRW.StartValue = Helper.DefaultValueForType(addRW.DataType);
                            }

                            //if (actualValues != null)
                            //{
                            //    addRW.Value = GetVarTypeVal((byte)addRW.DataType, actualValues, ref Valpos);
                            //}

                            akDataRow.Add(addRW);
                            ParaList.Add(tmpName);

                            if (addRW.DataType == S7DataRowType.STRUCT)
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
            }
            if (blkTP != PLCBlockType.DB && blkTP != PLCBlockType.UDT && tempAdded == false)
            {               
                parameterRoot.Add(parameterTEMP);               
            }

            if (actualValues != null)
            {
                int vPos = 0, bPos = 0;
                //FillActualValuesInDataBlock(parameterRoot, actualValues, ref vPos, ref bPos);
            }

            return parameterRoot;
        }

        internal static void FillActualValuesInDataBlock(S7DataRow row, byte[] actualValues, ref int valuePos, ref int bitPos)
        {
            if (valuePos >= actualValues.Length) 
                return;
            else
            {
                if (row.DataType != S7DataRowType.STRUCT && row.DataType != S7DataRowType.UDT)
                {
                    if (row.DataType == S7DataRowType.BOOL)
                    {
                        //Todo Array support here...
                        row.Value = libnodave.getBit(actualValues[valuePos], bitPos);
                        bitPos++;
                        if (bitPos > 7)
                        {
                            bitPos = 0;
                            valuePos++;
                        }
                    }
                    else
                    {
                        if (bitPos != 0)
                            valuePos++;
                        if (valuePos % 2 != 0 && row.DataType != S7DataRowType.BYTE)
                            valuePos++;
                        bitPos = 0;
                        row.Value = GetVarTypeVal((byte)row.DataType, actualValues, ref valuePos);
                    }
                }
                else
                {
                    if (bitPos != 0)
                        valuePos++;
                    if (valuePos % 2 != 0 && row.DataType != S7DataRowType.BYTE)
                        valuePos++;
                    bitPos = 0;
                    foreach (var child in row.Children)
                    {
                        FillActualValuesInDataBlock(((S7DataRow)child), actualValues, ref valuePos, ref bitPos);
                    }
                }
            }
        }


        /// <summary>
        /// Parses the interface from an MC7 Interface block
        /// </summary>
        /// <param name="interfaceBytes">The interface bytes from the MC7 code</param>
        /// <param name="actualvalueBytes">The corresponding Current data values. Only valid for DB</param>
        /// <param name="ParaList">OUT: The parsed Parameter list</param>
        /// <param name="blkTP">the block type of the block interface to be parsed</param>
        /// <param name="isInstanceDB">Indicates if the data block belongs to an Function block</param>
        /// <param name="myBlk">The block header</param>
        /// <returns></returns>
        internal static S7DataRow GetInterface(byte[] interfaceBytes, byte[] actualvalueBytes, ref List<String> ParaList, DataTypes.PLCBlockType blkTP, bool isInstanceDB, S7Block myBlk)        
        {
            S7DataRow parameterRoot = new S7DataRow("ROOTNODE", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterIN = new S7DataRow("IN", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterOUT = new S7DataRow("OUT", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterINOUT = new S7DataRow("IN_OUT", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterSTAT = new S7DataRow("STATIC", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterTEMP = new S7DataRow("TEMP", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterRETVAL = new S7DataRow("RET_VAL", S7DataRowType.STRUCT, myBlk);


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


            int pos = 7;
            int Valpos = 0;

            S7DataRow akParameter = parameterRoot;

            ParaList.Clear();

            while (pos <= (interfaceBytes.Length-2)) // && pos < BD.Length - 2)  //pos<BD.Length-2 was added so SDBs can be converted!! but is this needed?
            {
                object startVal;
                if (Helper.IsWithStartVal(interfaceBytes[pos + 1]) && actualvalueBytes != null)
                {
                    if (interfaceBytes[pos] != 0x10) //Datentyp == Array...
                        startVal = GetVarTypeVal(interfaceBytes[pos], actualvalueBytes, ref Valpos);
                    else
                    {
                        Valpos = Valpos + 6;
                        startVal = GetVarTypeVal(interfaceBytes[pos + 3 + (interfaceBytes[pos + 2] * 4)], actualvalueBytes, ref Valpos);
                    }
                }
                else
                    startVal = null;
                switch (interfaceBytes[pos + 1])
                {
                    case 0x01:
                    case 0x09:
                        {
                            GetVarTypeEN(parameterIN, startVal, interfaceBytes[pos], false, false, "IN" + Convert.ToString(INp), interfaceBytes, actualvalueBytes, ref pos, ref ParaList, ref StackNr, "IN", ref INp, ref Valpos, myBlk);
                        }
                        break;
                    case 0x02:
                    case 0x0A:
                        {
                            GetVarTypeEN(parameterOUT, startVal, interfaceBytes[pos], false, false, "OUT" + Convert.ToString(OUTp), interfaceBytes, actualvalueBytes, ref pos, ref ParaList, ref StackNr, "OUT", ref OUTp, ref Valpos, myBlk);
                        }
                        break;
                    case 0x03:
                    case 0x0b:
                        {
                            GetVarTypeEN(parameterINOUT, startVal, interfaceBytes[pos], false, false, "IN_OUT" + Convert.ToString(IN_OUTp), interfaceBytes, actualvalueBytes, ref pos, ref ParaList, ref StackNr, "IN_OUT", ref IN_OUTp, ref Valpos, myBlk);
                        }
                        break;
                    case 0x04:
                    case 0x0C:
                        {
                            GetVarTypeEN(parameterSTAT, startVal, interfaceBytes[pos], false, false, "STAT" + Convert.ToString(STATp), interfaceBytes, actualvalueBytes, ref pos, ref ParaList, ref StackNr, "STAT", ref STATp, ref Valpos, myBlk);
                        }
                        break;
                    case 0x05:
                        {
                            GetVarTypeEN(parameterTEMP, startVal, interfaceBytes[pos], false, false, "TEMP" + Convert.ToString(TEMPp), interfaceBytes, actualvalueBytes, ref pos, ref ParaList, ref StackNr, "TEMP", ref TEMPp, ref Valpos, myBlk);
                        }
                        break;
                    case 0x06:
                        {
                            int tmp = 0;
                            GetVarTypeEN(parameterRETVAL, startVal, interfaceBytes[pos], false, false, "RET_VAL", interfaceBytes, actualvalueBytes, ref pos, ref ParaList, ref StackNr, "RET_VAL", ref tmp, ref Valpos, myBlk);
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

        /*internal static S7DataRow GetInterface(int Start, int Count, byte[] BD, DataTypes.PLCBlockType blkTP, bool isInstanceDB, S7Block myBlk)        
        {
            S7DataRow parameterRoot = new S7DataRow("ROOTNODE", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterIN = new S7DataRow("IN", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterOUT = new S7DataRow("OUT", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterINOUT = new S7DataRow("IN_OUT", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterSTAT = new S7DataRow("STATIC", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterTEMP = new S7DataRow("TEMP", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterRETVAL = new S7DataRow("RET_VAL", S7DataRowType.STRUCT, myBlk);


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

            S7DataRow akParameter = parameterRoot;


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
        }*/

        //internal PLCDataRow GetInterfaceSubrows(PLCDataRow currRow)

            /// <summary>
            /// Parses an Interface parameter Row from the Interface of an MC7 block 
            /// </summary>
            /// <param name="currPar">The Parent Interface Row of the rows to be parsed</param>
            /// <param name="startVal">The start value of the interface parameter row (null if there is no start value)</param>
            /// <param name="b">The Interface parameter data type"/></param>
            /// <param name="Struct">The parameter row is inside an struct</param>
            /// <param name="Arry">the parameter row is inside an array</param>
            /// <param name="VarName">The variable name to be used</param>
            /// <param name="interfaceBytes">the interface bytes from the MC7 Code</param>
            /// <param name="actualvalueBytes">The current data bytes from the MC7 code</param>
            /// <param name="pos">OUT: the current parsing position</param>
            /// <param name="ParaList">OUT: the parsed parameter row number</param>
            /// <param name="StackNr">OUT: The current parsing stack depth</param>
            /// <param name="VarNamePrefix">The prefix to be used to generate the variable names from</param>
            /// <param name="VarCounter">OUT: the current variable count, used to generate the variable names</param>
            /// <param name="Valpos">OUT: the current parsing position for the actual values</param>
            /// <param name="myBlk">The block, where the interface belongs to</param>
        internal static void GetVarTypeEN(S7DataRow currPar, object startVal, byte b, bool Struct, bool Arry, string VarName, byte[] interfaceBytes, byte[] actualvalueBytes, ref int pos, ref List<string> ParaList, ref int StackNr, string VarNamePrefix, ref int VarCounter, ref int Valpos, S7Block myBlk)
        {
            int i, max, dim;

            S7DataRowType Result = S7DataRowType.BOOL;

            switch (b)
            {
                case 0x01:
                    Result = S7DataRowType.BOOL;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x02:
                    Result = S7DataRowType.BYTE;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x03:
                    Result = S7DataRowType.CHAR;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x04:
                    Result = S7DataRowType.WORD;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x05:
                    Result = S7DataRowType.INT;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x06:
                    Result = S7DataRowType.DWORD;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x07:
                    Result = S7DataRowType.DINT;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x08:
                    Result = S7DataRowType.REAL;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x09:
                    Result = S7DataRowType.DATE;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x0A:
                    Result = S7DataRowType.TIME_OF_DAY;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x0b:
                    Result = S7DataRowType.TIME;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x0C:
                    Result = S7DataRowType.S5TIME;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x0E:
                    Result = S7DataRowType.DATE_AND_TIME;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;

                case 0x10: //Array...
                    {
                        dim = interfaceBytes[pos + 2];
                        List<int> arrStart = new List<int>();
                        List<int> arrStop = new List<int>();

                        for (i = 0; i <= dim - 1; i++)
                        {
                            arrStart.Add(BitConverter.ToInt16(interfaceBytes, pos + 3 + (i * 4)));
                            arrStop.Add(BitConverter.ToInt16(interfaceBytes, pos + 5 + (i * 4)));                         
                        }
                        GetVarTypeEN(currPar, "", interfaceBytes[pos + 3 + (dim * 4)], true, true, VarName, interfaceBytes, actualvalueBytes, ref pos, ref ParaList, ref StackNr, VarNamePrefix, ref VarCounter, ref Valpos, myBlk);
                        ((S7DataRow)currPar.Children[currPar.Children.Count - 1]).ArrayStart = arrStart;
                        ((S7DataRow)currPar.Children[currPar.Children.Count - 1]).ArrayStop = arrStop;
                        ((S7DataRow)currPar.Children[currPar.Children.Count - 1]).IsArray = true;
                        pos += 3 + (dim * 4);

                    } break;
                case 0x11: //Struct
                    {
                        if (Arry) pos += 7;
                        Result = S7DataRowType.STRUCT;
                        var akPar = new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { StartValue = startVal };
                        currPar.Add(akPar);
                        VarCounter++;
                        max = interfaceBytes[pos + 2] - 1;
                        for (i = 0; i <= max; i++)
                        {



                            if ((interfaceBytes[pos + 3] == 0x11) || (interfaceBytes[pos + 3] == 0x10))
                            {
                                pos += 3;


                                if (Helper.IsWithStartVal(interfaceBytes[pos + 1]) && actualvalueBytes != null)
                                {
                                    if (interfaceBytes[pos] != 0x10) //Datentyp == Array...
                                        startVal = GetVarTypeVal(interfaceBytes[pos], actualvalueBytes, ref Valpos);
                                    else
                                    {
                                        Valpos = Valpos + 6;
                                        startVal = GetVarTypeVal(interfaceBytes[pos + 3 + (interfaceBytes[pos + 2] * 4)], actualvalueBytes, ref Valpos);
                                    }
                                }
                                else
                                    startVal = null;


                                GetVarTypeEN(akPar, startVal, interfaceBytes[pos], true, false, VarName + "." + VarNamePrefix + VarCounter.ToString(), interfaceBytes, actualvalueBytes, ref pos, ref ParaList, ref StackNr, VarNamePrefix, ref VarCounter, ref Valpos, myBlk);
                                pos -= 3;
                            }
                            else
                            {
                                if (Helper.IsWithStartVal(interfaceBytes[pos + 4]) && actualvalueBytes != null)
                                {
                                    if (interfaceBytes[pos] != 0x10) //Datentyp == Array...
                                        startVal = GetVarTypeVal(interfaceBytes[pos + 3], actualvalueBytes, ref Valpos);
                                    else
                                    {
                                        Valpos = Valpos + 6;
                                        startVal = GetVarTypeVal(interfaceBytes[pos + 6 + (interfaceBytes[pos + 2] * 4)], actualvalueBytes, ref Valpos);
                                    }
                                }
                                else
                                    startVal = null;

                                GetVarTypeEN(akPar, startVal, interfaceBytes[pos + 3], true, false, VarName + "." + VarNamePrefix + VarCounter.ToString(), interfaceBytes, actualvalueBytes, ref pos, ref ParaList, ref StackNr, VarNamePrefix, ref VarCounter, ref Valpos, myBlk);
                            }
                            pos += 2;
                        }
                        if (Arry) pos -= 7; pos += 1;
                    } break;

                case 0x13:
                    {
                        Result = S7DataRowType.STRING;
                        currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                        if (Arry)
                            currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal, StringSize = interfaceBytes[pos + 9] });
                        else
                            currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal, StringSize = interfaceBytes[pos + 2] });
                        pos += 1;
                        VarCounter++;
                    }
                    break;
                case 0x14:
                    Result = S7DataRowType.POINTER;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x16:
                    Result = S7DataRowType.ANY;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x17:
                    Result = S7DataRowType.BLOCK_FB;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x18:
                    Result = S7DataRowType.BLOCK_FC;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x19:
                    Result = S7DataRowType.BLOCK_DB;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x1A:
                    Result = S7DataRowType.BLOCK_SDB;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x1C:
                    Result = S7DataRowType.COUNTER;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
                    VarCounter++;
                    break;
                case 0x1D:
                    Result = S7DataRowType.TIMER;
                    currPar.Add(new S7DataRow(VarNamePrefix + VarCounter.ToString(), Result, myBlk) { Value = startVal });
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

        /// <summary>
        /// Get the current data value for an interface Row
        /// </summary>
        /// <param name="b">The data-type of the interface row</param>
        /// <param name="BD">The actual data of data block</param>
        /// <param name="Valpos">OUT: Current position of parsing</param>
        /// <returns></returns>
        internal static object GetVarTypeVal(byte b, byte[] BD, ref int Valpos)
        {

            if (BD == null) return null;
            if (BD.Length <= Valpos) return null;

            object Result;
            switch (b)
            {
                case 0x01:
                    { // 'BOOL';
                        if (BD[Valpos] == 0)
                            Result = false;
                        else
                            Result = true;
                        Valpos = Valpos + 1;
                    } break;
                case 0x02:
                    { // 'BYTE';
                        Result = BD[Valpos];
                        Valpos = Valpos + 1;
                    } break;
                case 0x03:
                    { // 'CHAR';
                        Result = (char)BD[Valpos];
                        Valpos = Valpos + 1;
                    } break;
                case 0x04:
                    { // 'WORD';
                        Result = libnodave.getU16from(BD, Valpos);
                        Valpos = Valpos + 2;
                    } break;
                case 0x05:
                    { // 'INT';
                        Result = libnodave.getS16from(BD, Valpos);
                        Valpos = Valpos + 2;
                    } break;
                case 0x06:
                    { // 'DWORD';
                        Result = libnodave.getU32from(BD, Valpos);
                        Valpos = Valpos + 4;
                    } break;
                case 0x07:
                    { // 'DINT';
                        Result = libnodave.getS32from(BD, Valpos);
                        Valpos = Valpos + 4;
                    } break;
                case 0x08:
                    { // 'REAL';
                        Result = libnodave.getFloatfrom(BD, Valpos);
                        Valpos = Valpos + 4;
                    } break;
                case 0x09:
                    { // 'DATE';
                        Result = libnodave.getDatefrom(BD, Valpos);
                        Valpos = Valpos + 2;
                    } break;
                case 0x0A:
                    { // 'TIME_OF_DAY';
                        Result = libnodave.getTimeOfDayfrom(BD, Valpos);                        
                        Valpos = Valpos + 4;
                    } break;
                case 0x0b:
                    { // 'TIME';
                        Result = libnodave.getTimefrom(BD, Valpos);                        
                        Valpos = Valpos + 4;
                    } break;
                case 0x0C:
                    { // 'S5TIME';
                        Result = libnodave.getS5Timefrom(BD, Valpos);
                        Valpos = Valpos + 2;
                    } break;
                case 0x0E:
                    { // 'DATE_AND_TIME';                        
                        Result = libnodave.getDateTimefrom(BD, Valpos);                        
                        Valpos = Valpos + 8;
                    } break;
                case 0x13:
                    { // 'STRING';
                        Result = Helper.GetS7String(Valpos, -1, BD);
                        Valpos = Valpos + BD[Valpos] + 2;
                    } break;
                case 0x21:
                    { // 'SFB??';
                        Result = "SFB??";
                    } break;
                default: Result = "UNKNOWN (" + Convert.ToString(b) + ")"; break;
            }

            return Result;
        }
    }
}