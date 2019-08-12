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
    /// <summary>
    /// This class is responsible for parsing Interface declarations from MC7. It parses all variables from the interface declaration and optionally fills in the startvalues and
    /// actual values. This class is used by the MC7 converter
    /// </summary>
    /// <remarks>
    /// The Interface of an MC7 block is structured as follows:
    /// Header:
    ///     0       Block Type: 0x05 (DB) 0x10 (DI)
    ///     1-2     Again Block Number or FB Number on a DI   (but bytes swapped)
    ///     3-4     Interface Length minus this header (7 bytes)
    ///     5-6     Start Value Length
    ///     
    /// Interface:
    /// 7-x     Line 1
    /// x-x     Line 2
    /// ...
    /// 
    /// for more information about the format of an Interface Line, please look at GetVarTypeEN
    /// </remarks>
    internal static class Parameter
    {

        /// <summary>
        /// Represents the Type of Parameter in an Interfacer, Not the Datatype.
        /// Basically ther are In, Out, INOUT, Static and Temp
        /// The _Init versions are the same with the above, except they have initial values that must ge parsed
        /// the _EX_ version are slightly different, in that they include an extra parameter in the interface declaration row. 
        /// I think this parameter is some kind of BitFiled declaring some options, but i am not sure what it is? See GetVarEn function for details
        /// </summary>
        private enum ParameterType : Byte
        {
            IN = 0x01,
            IN_Init = 0x09,
            IN_Ex = 0x11,
            IN_Ex_Init = 0x19,

            OUT = 0x02,
            OUT_Init = 0x0a,
            OUT_Ex = 0x12,
            OUT_Ex_Init = 0x1a,

            IN_OUT = 0x03,
            IN_OUT_Init = 0x0b,
            IN_OUT_Ex = 0x13,
            IN_OUT_Ex_Init = 0x1b,

            STATIC = 0x04,
            STATIC_Init = 0x0C,
            STATIC_Ex = 0x14,
            STATIC_Ex_Init = 0x1c,

            TEMP = 0x05,
            TEMP_Ex = 0x15,
            RET = 0x06,
            RET_Ex = 0x16
        }

        /// <summary>
        /// Goes down in the parameter rows and finds the datarow corresponding to the In, Out or InOut parameter that corresponds to Index
        /// It goes throught the interface of the Function of FunctionBlock the same way as S7 would (first In, Out then InOut
        /// </summary>
        /// <param name="parameters">The interface of the block</param>
        /// <param name="index">The number of the Parameter of the block</param>
        /// <returns></returns>
        internal static S7DataRow GetFunctionParameterFromNumber(S7DataRow parameters, int index)
        {
            if (parameters == null || parameters.Children == null)
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
        internal static bool IsInterfaceCompatible(IDataRow Block1, IDataRow Block2)
        {
            //Compare basic configuration
            //if (Block1.BlockAddress != Block2.BlockAddress) return false; //The adress must be the same
            if (Block1.DataType != Block2.DataType) return false; //Datatypes must match up

            //Check if any of the compared blocks contains an "TEMP" section. This section must be ignored
            //this assumes that "TEMP" sections are always the last ones
            //Todo! implement better solution without looping 3 times through the children
            int Childcount1 = Block1.Children.Count;
            foreach (var Child in Block1.Children) { if (Child.Name == "TEMP") Childcount1 -= 1; }
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

            S7DataRow parameterIN = new S7DataRow("IN", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterOUT = new S7DataRow("OUT", S7DataRowType.STRUCT, myBlk);
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
                if (blkTP == PLCBlockType.FC)
                {
                    parameterRoot.Add(parameterIN);
                    parameterRoot.Add(parameterOUT);
                    parameterRoot.Add(parameterINOUT);
                    parameterRoot.Add(parameterTEMP);
                }
                else if (blkTP != PLCBlockType.DB)
                    parameterRoot.Add(parameterTEMP);
                return parameterRoot;
            }

            //Todo: read the complete DB from mc5 code first, Read the containing UDTs, compare the UDTs with the Structs, if the UDTs and Structs are not Equal, marke the PLCDataRow as TimeStampConflict

            string[] rows = txt.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            S7DataRow lastrow = null;


            Dictionary<string, S7Block> blkHelpers = new Dictionary<string, S7Block>();

            for (int n = 0; n < rows.Length; n++) // (string row in rows)
            {
                string rowTr = rows[n].Replace("\0", "").Trim();
                int poscm = rowTr.IndexOf("\t//");
                if (poscm <= 0)
                    poscm = rowTr.Length;

                string switchrow = rowTr.Substring(0, poscm);
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

                            if (tmpType.Replace(" ", "").Contains("ARRAY["))
                            {
                                List<int> arrayStart = new List<int>();
                                List<int> arrayStop = new List<int>();

                                int pos1 = tmpType.IndexOf("[");
                                int pos2 = tmpType.IndexOf("]", pos1);
                                string[] arrays = tmpType.Substring(pos1 + 1, pos2 - pos1 - 2).Split(',');

                                foreach (string array in arrays)
                                {
                                    string[] akar = array.Split(new string[] { ".." }, StringSplitOptions.RemoveEmptyEntries);
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
                                    tmpBlk = (S7FunctionBlock)blkHelpers[blkDesc];
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

        /// <summary>
        /// Find and assign the Actual value of an Intervace to an given S7Datarow
        /// </summary>
        /// <param name="row"></param>
        /// <param name="actualValues"></param>
        /// <param name="valuePos">OUT: an pointer for tracking the current value position in the parsing process</param>
        internal static void FillActualValuesInDataBlock(S7DataRow DbDeclaration, byte[] actualValues)
        {
            //only valid datatypes will return an "Value" all other will return null
            DbDeclaration.Value = GetVarCurrentValue(DbDeclaration.DataType, actualValues, DbDeclaration.BlockAddress);

            //go Through children
            foreach (S7DataRow row in DbDeclaration.Children)
            {
                FillActualValuesInDataBlock(row, actualValues);
            }
        }

        /// <summary>
        /// Parses the interface from an MC7 Interface block
        /// </summary>
        /// <param name="interfaceBytes">The interface bytes from the MC7 code</param>
        /// <param name="startValueBytes">The corresponding Current data values. Only valid for DB</param>
        /// <param name="ParaList">OUT: The parsed Parameter list</param>
        /// <param name="blkTP">the block type of the block interface to be parsed</param>
        /// <param name="isInstanceDB">Indicates if the data block belongs to an Function block</param>
        /// <param name="myBlk">The block header</param>
        /// <returns></returns>
        internal static S7DataRow GetInterface(byte[] interfaceBytes, byte[] startValueBytes, byte[] actualValueBytes, ref List<String> ParaList, DataTypes.PLCBlockType blkTP, bool isInstanceDB, S7Block myBlk)
        {
            //prepare basic Parameter declarations
            S7DataRow parameterRoot = new S7DataRow("ROOTNODE", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterIN = new S7DataRow("IN", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterOUT = new S7DataRow("OUT", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterINOUT = new S7DataRow("IN_OUT", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterSTAT = new S7DataRow("STATIC", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterTEMP = new S7DataRow("TEMP", S7DataRowType.STRUCT, myBlk);
            S7DataRow parameterRETVAL = new S7DataRow("RET_VAL", S7DataRowType.STRUCT, myBlk);

            //All blocks may have In, Out or In/Out parameters
            //Info: the order in which they are added to the Root is important
            parameterRoot.Add(parameterIN);
            parameterRoot.Add(parameterOUT);
            parameterRoot.Add(parameterINOUT);

            if (blkTP == DataTypes.PLCBlockType.FB || (blkTP == DataTypes.PLCBlockType.DB && isInstanceDB))
                parameterRoot.Add(parameterSTAT); //Only FB's can have Statics

            if (blkTP != DataTypes.PLCBlockType.DB)
                parameterRoot.Add(parameterTEMP);//All blocks, but DB's can have Temporary Stack variables

            parameterRoot.Add(parameterRETVAL);  //All blocks have an RetVal      
            parameterRoot.ReadOnly = true;     //lock the Root in place, so it may not be changed anymore

            if (blkTP == DataTypes.PLCBlockType.DB && !isInstanceDB)
                parameterRoot = parameterSTAT;

            //Initialize parsing help variables
            int StackNr = 1; //Start at Stack depth 1 by default
            int InterfacePos = 7; //Start parsing of the Interface at Byte 7, since byte 0-6 are header information
            int StartValuePos = 0; //Startvalues are starting at 0
            S7DataRow akParameter = parameterRoot; //initially the root is the current parameter
            ParaList.Clear(); //just to be sure its empty

            //Varname generators
            VarNameGenerator VarNameIn = new VarNameGenerator("IN");
            VarNameGenerator VarNameOut = new VarNameGenerator("Out");
            VarNameGenerator VarNameInOut = new VarNameGenerator("IN_OUT");
            VarNameGenerator VarNameStat = new VarNameGenerator("STAT");
            VarNameGenerator VarNameTemp = new VarNameGenerator("TEMP");
            VarNameGenerator VarNameRet = new VarNameGenerator("RET");

            //Parse until the whole interface is Parsed. -2 because this is the minimum length of an Interface row
            while (InterfacePos <= interfaceBytes.Length - 2)
            {
                //Parse the Top level from the Interface declaration
                S7DataRowType DataType = (S7DataRowType)interfaceBytes[InterfacePos];
                ParameterType ParaType = (ParameterType)interfaceBytes[InterfacePos + 1];

                switch (ParaType)
                {
                    case ParameterType.IN:
                    case ParameterType.IN_Init:
                    case ParameterType.IN_Ex:
                    case ParameterType.IN_Ex_Init:
                        {
                            VarNameGenerator VarNameGen = VarNameIn;
                            GetVarTypeEN(parameterIN, DataType, false, false, VarNameGen.GetNextVarName(), interfaceBytes, ref InterfacePos, startValueBytes, ref StartValuePos, ref ParaList, ref StackNr, VarNameGen, myBlk);
                        }
                        break;
                    case ParameterType.OUT:
                    case ParameterType.OUT_Init:
                    case ParameterType.OUT_Ex:
                    case ParameterType.OUT_Ex_Init:
                        {
                            VarNameGenerator VarNameGen = VarNameOut;
                            GetVarTypeEN(parameterOUT, DataType, false, false, VarNameGen.GetNextVarName(), interfaceBytes, ref InterfacePos, startValueBytes, ref StartValuePos, ref ParaList, ref StackNr, VarNameGen, myBlk);
                        }
                        break;
                    case ParameterType.IN_OUT:
                    case ParameterType.IN_OUT_Init:
                    case ParameterType.IN_OUT_Ex:
                    case ParameterType.IN_OUT_Ex_Init:
                        {
                            VarNameGenerator VarNameGen = VarNameInOut;
                            GetVarTypeEN(parameterINOUT, DataType, false, false, VarNameGen.GetNextVarName(), interfaceBytes, ref InterfacePos, startValueBytes, ref StartValuePos, ref ParaList, ref StackNr, VarNameGen, myBlk);
                        }
                        break;
                    case ParameterType.STATIC:
                    case ParameterType.STATIC_Init:
                    case ParameterType.STATIC_Ex:
                    case ParameterType.STATIC_Ex_Init:
                        {
                            VarNameGenerator VarNameGen = VarNameStat;
                            GetVarTypeEN(parameterSTAT, DataType, false, false, VarNameGen.GetNextVarName(), interfaceBytes, ref InterfacePos, startValueBytes, ref StartValuePos, ref ParaList, ref StackNr, VarNameGen, myBlk);
                            break;
                        }

                    case ParameterType.TEMP:
                    case ParameterType.TEMP_Ex:
                        {
                            VarNameGenerator VarNameGen = VarNameTemp;
                            GetVarTypeEN(parameterTEMP, DataType, false, false, VarNameGen.GetNextVarName(), interfaceBytes, ref InterfacePos, startValueBytes, ref StartValuePos, ref ParaList, ref StackNr, VarNameGen, myBlk);
                        }
                        break;
                    case ParameterType.RET:
                    case ParameterType.RET_Ex:
                        {
                            VarNameGenerator VarNameGen = VarNameRet;
                            GetVarTypeEN(parameterRETVAL, DataType, false, false, VarNameGen.GetNextVarName(), interfaceBytes, ref InterfacePos, startValueBytes, ref StartValuePos, ref ParaList, ref StackNr, VarNameGen, myBlk);
                        }
                        break;

                    default:

                        //There is an special case for Multi Instance Block paraemters
                        //These can only ever occur in STATIC areas of Funciton Blocks
                        switch (DataType)
                        {
                            case S7DataRowType.MultiInst_FB:
                            case S7DataRowType.MultiInst_SFB:
                                VarNameGenerator VarNameGen = VarNameStat;
                                GetVarTypeEN(parameterSTAT, DataType, false, false, VarNameGen.GetNextVarName(), interfaceBytes, ref InterfacePos, startValueBytes, ref StartValuePos, ref ParaList, ref StackNr, VarNameGen, myBlk);
                                break;

                            //if it is also not one of the Block parameters, then Abort because the value is unknown
                            default:
                                throw new Exception(string.Format("invalid or unknown interface declarations found while parsing the block interface at pos {0} with Paratype {1} and Datatype {2}", InterfacePos, interfaceBytes[InterfacePos + 1], interfaceBytes[InterfacePos]));
                        }
                        break;
                }
            }

            if (actualValueBytes != null) FillActualValuesInDataBlock(parameterRoot, actualValueBytes);

            return parameterRoot;
        }

        /// <summary>
        /// Parses an Interface parameter Row from the Interface of an MC7 block 
        /// </summary>
        /// <param name="currPar">The Parent Interface Row of the rows to be parsed</param>
        /// <param name="datatype">The Interface parameter data type"/></param>
        /// <param name="Struct">The parameter row is inside an struct</param>
        /// <param name="Array">the parameter row is inside an array</param>
        /// <param name="VarName">The variable name to be used for the current row</param>
        /// <param name="interfaceBytes">the interface bytes from the MC7 Code</param>
        /// <param name="InterfacePos">OUT: the current parsing position</param>
        /// <param name="startValueBytes">The current data bytes from the MC7 code</param>
        /// <param name="StartValuePos">OUT: the current parsing position for Initial values</param>
        /// <param name="ParaList">OUT: the parsed parameter row number</param>
        /// <param name="StackNr">OUT: The current parsing stack depth</param>
        /// <param name="VarNameGen">The prefix to be used to generate the variable names from</param>
        /// <param name="myBlk">The block, where the interface belongs to</param>
        internal static void GetVarTypeEN(S7DataRow currPar, S7DataRowType datatype, bool Struct, bool Array, string VarName, byte[] interfaceBytes, ref int InterfacePos, byte[] startValueBytes, ref int StartValuePos, ref List<string> ParaList, ref int StackNr, VarNameGenerator VarNameGen, S7Block myBlk)
        {
            switch (datatype)
            {
                case S7DataRowType.BOOL:
                case S7DataRowType.BYTE:
                case S7DataRowType.CHAR:
                case S7DataRowType.WORD:
                case S7DataRowType.INT:
                case S7DataRowType.DWORD:
                case S7DataRowType.DINT:
                case S7DataRowType.REAL:
                case S7DataRowType.DATE:
                case S7DataRowType.TIME_OF_DAY:
                case S7DataRowType.TIME:
                case S7DataRowType.S5TIME:
                case S7DataRowType.DATE_AND_TIME:
                case S7DataRowType.POINTER:
                case S7DataRowType.ANY:
                case S7DataRowType.COUNTER:
                case S7DataRowType.TIMER:
                case S7DataRowType.BLOCK_FB:
                case S7DataRowType.BLOCK_FC:
                case S7DataRowType.BLOCK_DB:
                case S7DataRowType.BLOCK_SDB:
                    //Parese Elementary unarray datatypes from the interface
                    //All above datatypes have the same format:
                    //the Length of the Interface itema is always 2 bytes
                    //
                    //InterfacePos + 0     = Datatype: 17 for Array
                    //InterfacePos + 1     = ParameterType: see "ParameterType" Enumeration
                    //
                    //if parametertype is an "Ex" version
                    //InterfacePos + 0     = Datatype: 17 for Array
                    //InterfacePos + 1     = ParameterType: see "ParameterType" Enumeration
                    //InterfacePos + 2     = Seemingly Random values, with unknown meaning

                    var Par = new S7DataRow(VarName, datatype, myBlk);
                    ParameterType parameterType = (ParameterType)interfaceBytes[InterfacePos + 1];

                    //There is also an special case for "Extended" paramters. These contain an additional value
                    if (HasExtendedParameter(parameterType))
                    {
                        int unkwonValue = interfaceBytes[InterfacePos + 2];
                        InterfacePos++; //advance parsing by one additional byte
                    }

                    //if the type has an Start value, then parse it from the Start values
                    if (HasInitialValues(parameterType))
                        Par.StartValue = GetVarInitialValue(datatype, startValueBytes, ref StartValuePos);

                    currPar.Add(Par);
                    InterfacePos += 2; //Interface element is always 2 bytes
                    break;

                case S7DataRowType.MultiInst_FB:
                case S7DataRowType.MultiInst_SFB:
                    //Parese Block datatypes  from the interface
                    //All above datatypes have the same format:
                    //the Length of the Interface itema is always 3 bytes
                    //
                    //InterfacePos + 0     = Datatype: one of the BLOCK_xx types
                    //InterfacePos + 1     = Block number LSB
                    //InterfacePos + 1     = Block number MSB

                    Par = new S7DataRow(VarName, datatype, myBlk);

                    //if the type has an Start value, then parse it from the Start values
                    int BlockNumber = BitConverter.ToInt16(interfaceBytes, InterfacePos + 1);
                    Par.DataTypeBlockNumber = BlockNumber;

                    currPar.Add(Par);
                    InterfacePos += 3; //Interface element is always 3 bytes
                    break;

                case S7DataRowType.STRING:
                    //Parse String definition from Interface
                    //Strings are a special case and have neither the format of Elementary nor the collection types or Array types
                    //the Interface itme has an fixed length of:  3 
                    //
                    //InterfacePos + 0     = Datatype: 0x13 for String
                    //InterfacePos + 1     = ParameterType: see "ParameterType" Enumeration
                    //InterfacePos + 2     = String Length
                    //
                    //Parameter Type is an EX version:
                    //InterfacePos + 0     = Datatype: 0x13 for String
                    //InterfacePos + 1     = ParameterType: see "ParameterType" Enumeration
                    //InterfacePos + 2     = Seemingly Random values, with unknown meaning
                    //InterfacePos + 3     = String Length

                    Par = new S7DataRow(VarName, datatype, myBlk);

                    //There is also an special case for "Extended" paramters. These contain an additional value
                    parameterType = (ParameterType)interfaceBytes[InterfacePos + 1];
                    if (HasExtendedParameter(parameterType))
                    {
                        int unkwonValue = interfaceBytes[InterfacePos + 2];
                        InterfacePos++; //advance parsing by one additional byte
                    }

                    Par.StringSize = interfaceBytes[InterfacePos + 2];

                    if (HasInitialValues(parameterType))
                        Par.StartValue = GetVarInitialValue(datatype, startValueBytes, ref StartValuePos);

                    currPar.Add(Par);
                    InterfacePos += 3;  //3 byte interface row length
                    break;

                case S7DataRowType.ARRAY:
                    //Read the Array Dimension from the Interface
                    //An array has the following format
                    //the Interface itme has an variable length of:  3 + (Dimensions * 4)
                    //
                    //InterfacePos + 0     = Datatype: 0x10 for Array
                    //InterfacePos + 1     = ParameterType: see "ParameterType" Enumeration
                    //InterfacePos + 2     = Dimensions: Amount of dimensions to be parsed
                    //InterfacePos + 3-4   = Lower Bound of 1st Dimension
                    //InterfacePos + 5-6   = Upper Bound of 1st Dimension
                    //InterfacePos + 7-8   = Lower Bound of 2nd Dimension if any
                    //InterfacePos + 9-10  = Upper Bound of 2nd Dimensino if any
                    //....      
                    //
                    //Parameter Type is an EX version:
                    //InterfacePos + 0     = Datatype: 0x10 for Array
                    //InterfacePos + 1     = ParameterType: see "ParameterType" Enumeration
                    //InterfacePos + 2     = Seemingly Random values, with unknown meaning
                    //InterfacePos + 3     = Dimensions: Amount of dimensions to be parsed
                    //InterfacePos + 4-5   = Lower Bound of 1st Dimension
                    //InterfacePos + 6-7   = Upper Bound of 1st Dimension
                    //InterfacePos + 8-9   = Lower Bound of 2nd Dimension if any
                    //InterfacePos + 10-11  = Upper Bound of 2nd Dimensino if any
                    //....      

                    //There is also an special case for "Extended" paramters. These contain an additional value
                    parameterType = (ParameterType)interfaceBytes[InterfacePos + 1];
                    if (HasExtendedParameter(parameterType))
                    {
                        int unkwonValue = interfaceBytes[InterfacePos + 2];
                        InterfacePos++; //advance parsing by one additional byte
                    }

                    int ArrayDim = interfaceBytes[InterfacePos + 2];
                    List<int> arrStart = new List<int>();
                    List<int> arrStop = new List<int>();

                    for (int i = 0; i <= ArrayDim - 1; i++)
                    {
                        arrStart.Add(BitConverter.ToInt16(interfaceBytes, InterfacePos + 3 + (i * 4)));
                        arrStop.Add(BitConverter.ToInt16(interfaceBytes, InterfacePos + 5 + (i * 4)));
                    }

                    //Parse down child elements
                    InterfacePos += 3 + (ArrayDim * 4);  //3 byte array header, and every dimension has an upper and lower bound, each an Uint16
                    GetVarTypeEN(currPar, (S7DataRowType)interfaceBytes[InterfacePos], true, true, VarName, interfaceBytes, ref InterfacePos, startValueBytes, ref StartValuePos, ref ParaList, ref StackNr, VarNameGen, myBlk);

                    //Mark the current Element as Array, so that the recently parsed elements are counted as Array objects
                    ((S7DataRow)currPar.Children[currPar.Children.Count - 1]).ArrayStart = arrStart;
                    ((S7DataRow)currPar.Children[currPar.Children.Count - 1]).ArrayStop = arrStop;
                    ((S7DataRow)currPar.Children[currPar.Children.Count - 1]).IsArray = true;

                    break;

                case S7DataRowType.STRUCT: //Struct
                    //Structs are nested datatypes, so go one recursivly. Also UDT get converted to Structs, so they are indistiguishable from them
                    //Structs have the following format:
                    //Structure Interface elements have an fixed length of 3 bytes or fixed length or 5 byte if children count is greater then 255.
                    //There is also an special case with 4 Bytes length
                    //
                    //Child count < 255:
                    //InterfacePos + 0     = Datatype: 0x11 for struct
                    //InterfacePos + 1     = ParameterType: see "ParameterType" Enumeration. NOTE it is never DBInit, which means it never has Initial values
                    //InterfacePos + 2     = Children count: the amount of sub-variables declared inside the structure
                    //
                    //There is an Special Case when the Child amount is greater than 255 Children. 
                    //in that case the original childcount on InterfacePos + 2 is set to FF and the next two bytes contain the real Child count
                    //
                    //Child count > 255:
                    //InterfacePos + 0     = Datatype: 0x11 for struct
                    //InterfacePos + 1     = ParameterType: see "ParameterType" Enumeration. NOTE it is never DBInit, which means it never has Initial values
                    //InterfacePos + 2     = Marker that there are mor children: always 255
                    //InterfacePos + 3     = Child count LSB
                    //InterfacePos + 4     = Child count MSB
                    //
                    //There is another special case, when the ParameterType is 0x14. In that case the layout is similar to the special case with more than 255 children,
                    //Except that the value InterfacePos + 2 has seemingly random Values. At the moment i do not know what these values mean
                    //
                    //Parameter Type is an EX version:
                    //InterfacePos + 0     = Datatype: 0x11 for struct
                    //InterfacePos + 1     = ParameterType: see "ParameterType" Enumeration. NOTE it is never DBInit, which means it never has Initial values
                    //InterfacePos + 2     = Seemingly Random values, with unknown meaning
                    //InterfacePos + 3     = Child count

                    var akPar = new S7DataRow(VarName, datatype, myBlk);
                    currPar.Add(akPar);

                    //There is also an special case for "Extended" paramters. These contain an additional value
                    parameterType = (ParameterType)interfaceBytes[InterfacePos + 1];
                    if (HasExtendedParameter(parameterType))
                    {
                        int unkwonValue = interfaceBytes[InterfacePos + 2];
                        InterfacePos++; //advance parsing by one additional byte
                    }

                    //Extract Children count from interface
                    int Children = interfaceBytes[InterfacePos + 2];
                    if (Children == 255)
                    {
                        //Reparse the children count from the next two bytes as an Integer as oposed to an single byte above
                        Children = BitConverter.ToUInt16(interfaceBytes, InterfacePos + 3);
                        InterfacePos += 5; //5 bytes for Structure Element length if more than 255 children, so it points to the next child element
                    }
                    else { InterfacePos += 3; } //3 bytes for Structure Element length, so it points to the next child element

                    //Continue parsing insde the new Struct
                    for (int i = 0; i < Children; i++)
                    {
                        GetVarTypeEN(akPar, (S7DataRowType)interfaceBytes[InterfacePos], true, false, VarNameGen.GetNextVarName(), interfaceBytes, ref InterfacePos, startValueBytes, ref StartValuePos, ref ParaList, ref StackNr, VarNameGen, myBlk);
                    }
                    break;

                default:
                    throw new Exception(string.Format("invalid or unknown interface declarations found while parsing the block interface at pos {0} with Paratype {1} and Datatype {2}", InterfacePos, interfaceBytes[InterfacePos + 1], interfaceBytes[InterfacePos]));
            }

            ParaList.Add(VarName);
            StackNr = StackNr + 1;
        }

        private static bool HasInitialValues(ParameterType pt)
        {
            switch (pt)
            {
                case ParameterType.IN_Init:
                case ParameterType.IN_Ex_Init:
                case ParameterType.IN_OUT_Init:
                case ParameterType.IN_OUT_Ex_Init:
                case ParameterType.OUT_Init:
                case ParameterType.OUT_Ex_Init:
                case ParameterType.STATIC_Init:
                case ParameterType.STATIC_Ex_Init:
                    return true;
                default: return false;
            }
        }

        private static bool HasExtendedParameter(ParameterType pt)
        {
            switch (pt)
            {
                case ParameterType.IN_Ex:
                case ParameterType.IN_Ex_Init:
                case ParameterType.IN_OUT_Ex:
                case ParameterType.IN_OUT_Ex_Init:
                case ParameterType.OUT_Ex:
                case ParameterType.OUT_Ex_Init:
                case ParameterType.STATIC_Ex:
                case ParameterType.STATIC_Ex_Init:
                case ParameterType.TEMP_Ex:
                case ParameterType.RET_Ex:
                    return true;
                default: return false;
            }
        }

        /// <summary>
        /// Get corresponding current value. These values essentially have the exact same layout as one is familiar with the "GetBytes" interfaces 
        /// From the communication library. 
        /// </summary>
        /// <param name="dataType">The data-type of the interface row</param>
        /// <param name="data">Either an Array containing the Start Values or Actual Values</param>
        /// <param name="valpos">OUT: Current position of parsing</param>
        /// <returns>the parsed value acording to the given datatyep and adress. If the datatype is invalid (such as struct) it returns Null</returns>
        internal static object GetVarCurrentValue(S7DataRowType dataType, byte[] data, ByteBitAddress valpos)
        {
            if (data == null) return null;

            //Parse Value from data depending on its interface type
            object Result;
            switch (dataType)
            {
                case S7DataRowType.BOOL:
                    { // 'BOOL';
                        Result = libnodave.getBit(data[valpos.ByteAddress], valpos.BitAddress);
                    }
                    break;
                case S7DataRowType.BYTE:
                    { // 'BYTE';
                        Result = data[valpos.ByteAddress];
                    }
                    break;
                case S7DataRowType.CHAR:
                    { // 'CHAR';
                        Result = (char)data[valpos.ByteAddress];
                    }
                    break;
                case S7DataRowType.WORD:
                    { // 'WORD';
                        Result = libnodave.getU16from(data, valpos.ByteAddress);
                    }
                    break;
                case S7DataRowType.INT:
                    { // 'INT';
                        Result = libnodave.getS16from(data, valpos.ByteAddress);
                    }
                    break;
                case S7DataRowType.DWORD:
                    { // 'DWORD';
                        Result = libnodave.getU32from(data, valpos.ByteAddress);
                    }
                    break;
                case S7DataRowType.DINT:
                    { // 'DINT';
                        Result = libnodave.getS32from(data, valpos.ByteAddress);
                    }
                    break;
                case S7DataRowType.REAL:
                    { // 'REAL';
                        Result = libnodave.getFloatfrom(data, valpos.ByteAddress);
                    }
                    break;
                case S7DataRowType.DATE:
                    { // 'DATE';
                        Result = libnodave.getDatefrom(data, valpos.ByteAddress);
                    }
                    break;
                case S7DataRowType.TIME_OF_DAY:
                    { // 'TIME_OF_DAY';
                        Result = libnodave.getTimeOfDayfrom(data, valpos.ByteAddress);
                    }
                    break;
                case S7DataRowType.TIME:
                    { // 'TIME';
                        Result = libnodave.getTimefrom(data, valpos.ByteAddress);
                    }
                    break;
                case S7DataRowType.S5TIME:
                    { // 'S5TIME';
                        Result = libnodave.getS5Timefrom(data, valpos.ByteAddress);
                    }
                    break;
                case S7DataRowType.DATE_AND_TIME:
                    { // 'DATE_AND_TIME';                        
                        Result = libnodave.getDateTimefrom(data, valpos.ByteAddress);
                    }
                    break;
                case S7DataRowType.STRING:
                    { // 'STRING';
                        Result = Helper.GetS7String(valpos.ByteAddress, -1, data);
                    }
                    break;
                case S7DataRowType.SFB: //unclear, needs to be checked
                    { // 'SFB??';
                        Result = "SFB??";
                    }
                    break;
                default:
                    Result = null;
                    break;
            }

            return Result;
        }

        /// <summary>
        /// Get corresponding Initial value
        /// </summary>
        /// <param name="dataType">The data-type of the interface row</param>
        /// <param name="data">Either an Array containing the Start Values or Actual Values</param>
        /// <param name="valpos">OUT: Current position of parsing</param>
        /// <returns>the parsed value acording to the given datatyep and adress. If the datatype is invalid (such as struct) it returns Null</returns>
        /// <remarks>
        /// The Initial values are encoded in an compacted manner. Meaning, if an varialbe does not have an
        /// Initial value, it does not appear in the start values list!
        /// this means the valpos must be tracked, and increased every time an Avlue was parsed from the array
        /// 
        /// WARNING! be aware that for somehow reason Siemens encoded the Startvalues in Little Endien format,
        /// completly contrary to the comon S7 format! This means one can not use the Libnodave.Getxxx funcions, 
        /// but rather has to use the usual Bitconverter
        /// 
        /// Basically the logic here uses the default libnodave.getxxx function, but with Bitconverter instead. 
        /// </remarks>
        internal static object GetVarInitialValue(S7DataRowType dataType, byte[] data, ref int valpos)
        {
            if (data == null) return null;

            //Parse Value from data depending on its interface type
            object Result;
            switch (dataType)
            {
                case S7DataRowType.BOOL:
                    Result = data[valpos] > 0;
                    valpos++;
                    break;
                case S7DataRowType.BYTE:
                    Result = data[valpos];
                    valpos++;
                    break;
                case S7DataRowType.CHAR:
                    { // 'CHAR';
                        Result = (char)data[valpos];
                        valpos++;
                    }
                    break;
                case S7DataRowType.WORD:
                    { // 'WORD';
                        Result = BitConverter.ToInt16(data, valpos);
                        valpos += 2;
                    }
                    break;
                case S7DataRowType.INT:
                    { // 'INT';
                        Result = BitConverter.ToInt16(data, valpos);
                        valpos += 2;
                    }
                    break;
                case S7DataRowType.DWORD:
                    { // 'DWORD';
                        Result = BitConverter.ToInt32(data, valpos);
                        valpos += 4;
                    }
                    break;
                case S7DataRowType.DINT:
                    { // 'DINT';
                        Result = BitConverter.ToInt32(data, valpos);
                        valpos += 4;
                    }
                    break;
                case S7DataRowType.REAL:
                    { // 'REAL';
                        Result = BitConverter.ToSingle(data, valpos);
                        valpos += 4;
                    }
                    break;
                case S7DataRowType.DATE:
                    { // 'DATE';
                        DateTime tmp = new DateTime(1990, 1, 1);
                        var tmp2 = TimeSpan.FromDays(BitConverter.ToUInt16(data, valpos));
                        tmp = tmp.Add(tmp2);
                        Result = tmp;
                        valpos += 2;
                    }
                    break;
                case S7DataRowType.TIME_OF_DAY:
                    { // 'TIME_OF_DAY';
                        long msval = BitConverter.ToUInt32(data, valpos);
                        Result = new DateTime(msval * 10000);
                        valpos += 4;
                    }
                    break;
                case S7DataRowType.TIME:
                    { // 'TIME';
                        long msval = BitConverter.ToInt32(data, valpos);
                        Result = TimeSpan.FromMilliseconds(msval);
                        valpos += 4;
                    }
                    break;
                case S7DataRowType.S5TIME:
                    { // 'S5TIME';
                        byte[] b1 = new byte[2];
                        b1[1] = data[valpos + 0];
                        b1[0] = data[valpos + 1];

                        Result = libnodave.getS5Timefrom(b1, 0);
                        valpos += 2;
                    }
                    break;
                case S7DataRowType.DATE_AND_TIME:
                    { // 'DATE_AND_TIME';                        
                        Result = libnodave.getDateTimefrom(data, valpos);
                        valpos += 8;
                    }
                    break;
                case S7DataRowType.STRING:
                    { // 'STRING';
                        Result = Helper.GetS7String(valpos, -1, data);
                        valpos += ((string)Result).Length + 2 - 2; //+2 because S7 strings have an one byte length filed and one byte used filed, and -2 because this library returns the string single quoted
                    }
                    break;
                //case S7DataRowType.SFB: //unclear, needs to be checked
                //    { // 'SFB??';
                //        Result = "SFB??";
                //    }
                //    break;
                default:
                    Result = null;
                    break;
            }

            return Result;
        }

        /// <summary>
        /// Small helper class to keep track of the current Variable name
        /// </summary>
        internal class VarNameGenerator
        {
            private string Prefix = "STAT";
            private int VarCounter = 0;

            public VarNameGenerator()
            { }

            public VarNameGenerator(string prefix)
            { Prefix = prefix; }

            /// <summary>
            /// Returns the next Variable name and increments the internal Variable count
            /// </summary>
            /// <returns></returns>
            public string GetNextVarName()
            {
                string Tmp = Prefix + VarCounter.ToString();
                VarCounter++;
                return Tmp;
            }
        }
    }
}