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
    internal static class Parameter
    {
        //PLCDataRow
        //Diconary ByteAddres, PLCDataRow

        //Create Extra Functions for GetValues and GetActualValues
        //This functions get the PLCDataRow Structure.
        private enum ParameterType : Byte
        {
             In = 0x01,
             DBIn = 0x09,
             Out = 0x02,
             DBOut = 0x0A,
             InOut = 0x03,
             DBInOut = 0x0b,
             FBStat = 0x04,
             DBStat = 0x0C,
             Temp = 0x05,
             Ret = 0x06,
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

        /// <summary>
        /// Find and assign the Actual value of an Intervace to an given S7Datarow
        /// </summary>
        /// <param name="row"></param>
        /// <param name="actualValues"></param>
        /// <param name="valuePos">OUT: an pointer for tracking the current value position in the parsing process</param>
        internal static void FillActualValuesInDataBlock(S7DataRow DbDeclaration, byte[] actualValues)
        {
            //only valid datatypes will return an "Value" all other will return null
            DbDeclaration.Value = GetVarTypeVal(DbDeclaration.DataType, actualValues, DbDeclaration.BlockAddress);

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

            //Only FB's can have Statics
            if (blkTP == DataTypes.PLCBlockType.FB || (blkTP == DataTypes.PLCBlockType.DB && isInstanceDB))
                parameterRoot.Add(parameterSTAT);

            //All blocks, but DB's can have Temporary Stack variables
            if (blkTP != DataTypes.PLCBlockType.DB)
                parameterRoot.Add(parameterTEMP);

            //All blocks have an RetVal
            parameterRoot.Add(parameterRETVAL);
            parameterRoot.ReadOnly = true; //lock the Root in place, so it may not be changed anymore

            if (blkTP == DataTypes.PLCBlockType.DB && !isInstanceDB)
                parameterRoot = parameterSTAT;

            int INp = 0;
            int OUTp = 0;
            int IN_OUTp = 0;
            int STATp = 0;
            int TEMPp = 0;
            int StackNr = 1;

            int pos = 7; //Start parsing of the Interface at Byte 7, since byte 0-6 are header information
            S7DataRow akParameter = parameterRoot;

            ParaList.Clear();

            while (pos <= (interfaceBytes.Length-2)) // && pos < BD.Length - 2)  //pos<BD.Length-2 was added so SDBs can be converted!! but is this needed?
            {
 
                //Parse Parameter type from Interface
                switch ((ParameterType)interfaceBytes[pos + 1])
                {
                    case ParameterType.In:
                    case ParameterType.DBIn:
                        {
                            GetVarTypeEN(parameterIN, (S7DataRowType)interfaceBytes[pos], false, false, "IN" + Convert.ToString(INp), interfaceBytes, startValueBytes, actualValueBytes, ref pos, ref ParaList, ref StackNr, "IN", ref INp, myBlk);
                        }
                        break;
                    case ParameterType.Out:
                    case ParameterType.DBOut:
                        {
                            GetVarTypeEN(parameterOUT, (S7DataRowType)interfaceBytes[pos], false, false, "OUT" + Convert.ToString(OUTp), interfaceBytes, startValueBytes, actualValueBytes, ref pos, ref ParaList, ref StackNr, "OUT", ref OUTp,  myBlk);
                        }
                        break;
                    case ParameterType.InOut:
                    case ParameterType.DBInOut:
                        {
                            GetVarTypeEN(parameterINOUT, (S7DataRowType)interfaceBytes[pos], false, false, "IN_OUT" + Convert.ToString(IN_OUTp), interfaceBytes, startValueBytes, actualValueBytes, ref pos, ref ParaList, ref StackNr, "IN_OUT", ref IN_OUTp,  myBlk);
                        }
                        break;
                    case ParameterType.FBStat:
                    case ParameterType.DBStat:
                        {
                            GetVarTypeEN(parameterSTAT, (S7DataRowType)interfaceBytes[pos], false, false, "STAT" + Convert.ToString(STATp), interfaceBytes, startValueBytes, actualValueBytes, ref pos, ref ParaList, ref StackNr, "STAT", ref STATp, myBlk);
                        }
                        break;
                    case ParameterType.Temp:
                        {
                            GetVarTypeEN(parameterTEMP, (S7DataRowType)interfaceBytes[pos], false, false, "TEMP" + Convert.ToString(TEMPp), interfaceBytes, startValueBytes, actualValueBytes, ref pos, ref ParaList, ref StackNr, "TEMP", ref TEMPp, myBlk);
                        }
                        break;
                    case ParameterType.Ret:
                        {
                            int tmp = 0;
                            GetVarTypeEN(parameterRETVAL, (S7DataRowType)interfaceBytes[pos], false, false, "RET_VAL", interfaceBytes, startValueBytes, actualValueBytes, ref pos, ref ParaList, ref StackNr, "RET_VAL", ref tmp, myBlk);
                        }
                        break;
                }
                pos += 2;
            }
            return parameterRoot;
        }
        
        /// <summary>
        /// Parses an Interface parameter Row from the Interface of an MC7 block 
        /// </summary>
        /// <param name="currPar">The Parent Interface Row of the rows to be parsed</param>
        /// <param name="startVal">The start value of the interface parameter row (null if there is no start value)</param>
        /// <param name="datatype">The Interface parameter data type"/></param>
        /// <param name="Struct">The parameter row is inside an struct</param>
        /// <param name="Array">the parameter row is inside an array</param>
        /// <param name="VarName">The variable name to be used</param>
        /// <param name="interfaceBytes">the interface bytes from the MC7 Code</param>
        /// <param name="startValueBytes">The current data bytes from the MC7 code</param>
        /// <param name="pos">OUT: the current parsing position</param>
        /// <param name="ParaList">OUT: the parsed parameter row number</param>
        /// <param name="StackNr">OUT: The current parsing stack depth</param>
        /// <param name="VarNamePrefix">The prefix to be used to generate the variable names from</param>
        /// <param name="VarCounter">OUT: the current variable count, used to generate the variable names</param>
        /// <param name="Valpos">OUT: the current parsing position for the actual values</param>
        /// <param name="myBlk">The block, where the interface belongs to</param>
        internal static void GetVarTypeEN(S7DataRow currPar,  S7DataRowType datatype, bool Struct, bool Array, string VarName, byte[] interfaceBytes, byte[] startValueBytes, byte[] actualValueBytes, ref int pos, ref List<string> ParaList, ref int StackNr, string VarNamePrefix, ref int VarCounter, S7Block myBlk)
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
                case S7DataRowType.BLOCK_FB:
                case S7DataRowType.BLOCK_FC:
                case S7DataRowType.BLOCK_DB:
                case S7DataRowType.BLOCK_SDB:
                case S7DataRowType.COUNTER:
                case S7DataRowType.TIMER:
                    var Par = new S7DataRow(VarNamePrefix + VarCounter.ToString(), datatype, myBlk);
                    currPar.Add(Par);
                    Par.StartValue = GetVarTypeVal(Par.DataType, startValueBytes, Par.BlockAddress);
                    Par.Value = GetVarTypeVal(Par.DataType, actualValueBytes, Par.BlockAddress);

                    VarCounter++;
                    break;

                case S7DataRowType.ARRAY: 
                    {
                        //First read the array length from the interface
                        //TODO: Check what happens on multidimensional arrays!
                        //TODO: Add some documentation here on how the interface is encoded. It is not quite clear what is happening here
                        int ArrayDim = interfaceBytes[pos + 2];
                        List<int> arrStart = new List<int>();
                        List<int> arrStop = new List<int>();

                        for (int i = 0; i <= ArrayDim - 1; i++)
                        {
                            arrStart.Add(BitConverter.ToInt16(interfaceBytes, pos + 3 + (i * 4)));
                            arrStop.Add(BitConverter.ToInt16(interfaceBytes, pos + 5 + (i * 4)));                         
                        }

                        GetVarTypeEN(currPar, (S7DataRowType)interfaceBytes[pos + 3 + (ArrayDim * 4)], true, true, VarName, interfaceBytes, startValueBytes, actualValueBytes, ref pos, ref ParaList, ref StackNr, VarNamePrefix, ref VarCounter, myBlk);
                        ((S7DataRow)currPar.Children[currPar.Children.Count - 1]).ArrayStart = arrStart;
                        ((S7DataRow)currPar.Children[currPar.Children.Count - 1]).ArrayStop = arrStop;
                        ((S7DataRow)currPar.Children[currPar.Children.Count - 1]).IsArray = true;
                        pos += 3 + (ArrayDim * 4);

                    } break;

                    //Structs are nested datatypes, so go one recursivly. Also UDT get converted to Structs, so they are indistiguishable from them
                case S7DataRowType.STRUCT: //Struct
                    {
                        if (Array) pos += 7; //its an Array of Struct

                        var akPar = new S7DataRow(VarNamePrefix + VarCounter.ToString(), datatype, myBlk);
                        currPar.Add(akPar);
                        akPar.StartValue = GetVarTypeVal(akPar.DataType, startValueBytes, akPar.BlockAddress);
                        akPar.Value = GetVarTypeVal(akPar.DataType, actualValueBytes, akPar.BlockAddress);

                        VarCounter++;

                        //Continue parsing insde the new Struct
                        int max = interfaceBytes[pos + 2] - 1;
                        for (int i = 0; i <= max; i++)
                        {
                            //in the case it is an nested Structure or Array (so not an elementary S7 type)
                            if (((S7DataRowType)interfaceBytes[pos + 3] == S7DataRowType.STRUCT) || 
                                ((S7DataRowType)interfaceBytes[pos + 3] == S7DataRowType.ARRAY))
                            {
                                pos += 3;
                                                              
                                GetVarTypeEN(akPar,  (S7DataRowType)interfaceBytes[pos], true, false, VarName + "." + VarNamePrefix + VarCounter.ToString(), interfaceBytes, startValueBytes, actualValueBytes, ref pos, ref ParaList, ref StackNr, VarNamePrefix, ref VarCounter, myBlk);
                                pos -= 3;
                            }                           
                            else //its an Elementary S7 datatype
                            {
                                GetVarTypeEN(akPar, (S7DataRowType)interfaceBytes[pos + 3], true, false, VarName + "." + VarNamePrefix + VarCounter.ToString(), interfaceBytes, startValueBytes, actualValueBytes, ref pos, ref ParaList, ref StackNr, VarNamePrefix, ref VarCounter, myBlk);
                            }
                            pos += 2;
                        }
                        if (Array) pos -= 7; pos += 1;
                    } break; 
                    
                default:
                    System.Diagnostics.Trace.WriteLine("Found unknown Datatype while parsing an Interface:" + Convert.ToString(datatype) + ")");
                    break;
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
        /// Get corresponding Data from either Start Values or Actual Values (depending on what data is given in "Data")
        /// </summary>
        /// <param name="dataType">The data-type of the interface row</param>
        /// <param name="data">Either an Array containing the Start Values or Actual Values</param>
        /// <param name="valpos">OUT: Current position of parsing</param>
        /// <returns>the parsed value acording to the given datatyep and adress. If the datatype is invalid (such as struct) it returns Null</returns>
        internal static object GetVarTypeVal(S7DataRowType dataType, byte[] data,  ByteBitAddress valpos)
        {
            if (data == null) return null;

            //Parse Value from data depending on its interface type
            object Result;
            switch (dataType)
            {
                case S7DataRowType.BOOL:
                    { // 'BOOL';
                        Result = libnodave.getBit(data[valpos.ByteAddress], valpos.BitAddress);
                    } break;
                case S7DataRowType.BYTE:
                    { // 'BYTE';
                        Result = data[valpos.ByteAddress];
                    } break;
                case S7DataRowType.CHAR:
                    { // 'CHAR';
                        Result = (char)data[valpos.ByteAddress];
                    } break;
                case S7DataRowType.WORD:
                    { // 'WORD';
                        Result = libnodave.getU16from(data, valpos.ByteAddress);
                    } break;
                case S7DataRowType.INT:
                    { // 'INT';
                        Result = libnodave.getS16from(data, valpos.ByteAddress);
                    } break;
                case S7DataRowType.DWORD:
                    { // 'DWORD';
                        Result = libnodave.getU32from(data, valpos.ByteAddress);
                    } break;
                case S7DataRowType.DINT:
                    { // 'DINT';
                        Result = libnodave.getS32from(data, valpos.ByteAddress);
                    } break;
                case S7DataRowType.REAL:
                    { // 'REAL';
                        Result = libnodave.getFloatfrom(data, valpos.ByteAddress);
                    } break;
                case S7DataRowType.DATE:
                    { // 'DATE';
                        Result = libnodave.getDatefrom(data, valpos.ByteAddress);
                    } break;
                case S7DataRowType.TIME_OF_DAY:
                    { // 'TIME_OF_DAY';
                        Result = libnodave.getTimeOfDayfrom(data, valpos.ByteAddress);                        
                    } break;
                case S7DataRowType.TIME:
                    { // 'TIME';
                        Result = libnodave.getTimefrom(data, valpos.ByteAddress);                        
                    } break;
                case S7DataRowType.S5TIME:
                    { // 'S5TIME';
                        Result = libnodave.getS5Timefrom(data, valpos.ByteAddress);
                    } break;
                case S7DataRowType.DATE_AND_TIME:
                    { // 'DATE_AND_TIME';                        
                        Result = libnodave.getDateTimefrom(data, valpos.ByteAddress);                        
                    } break;
                case S7DataRowType.STRING:
                    { // 'STRING';
                        Result = Helper.GetS7String(valpos.ByteAddress, -1, data);
                    } break;
                case S7DataRowType.SFB: //unclear, needs to be checked
                    { // 'SFB??';
                        Result = "SFB??";
                    } break;
                default:
                    Result = null;
                    break;
            }

            return Result;
        }
    }
}