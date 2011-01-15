using System;
using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    static class CallConverter
    {
        //In this Class a UC is converted to a Call and also backwards...
        public static void ConvertUCToCall(PLCFunctionBlock myFct, S7ProgrammFolder myFld, MC7ConvertingOptions myOpt, byte[] addInfoFromBlock)
        {
            if (myOpt.GenerateCallsfromUCs)
            {
                int inBld = 0; //1=nach BLD 1
                PLCFunctionBlockRow newRow = null;

                Dictionary<string, string> Parameters = new Dictionary<string, string>();
                List<PLCFunctionBlockRow> retVal = new List<PLCFunctionBlockRow>();
                List<PLCFunctionBlockRow> tempList = new List<PLCFunctionBlockRow>();

                string akPar = "";

                string db = "";

                for (int n = 0; n < myFct.AWLCode.Count; n++)
                {
                    PLCFunctionBlockRow row = myFct.AWLCode[n];
                    if (row.Command == Memnoic.opBLD[myOpt.Memnoic] && row.Parameter == "1")
                    {
                        retVal.AddRange(tempList);
                        tempList.Clear();

                        Parameters.Clear();
                        inBld = 1;
                        newRow = null;
                        tempList.Add(row);
                    }
                    else if (inBld > 0)
                    {
                        tempList.Add(row);
                        if (row.Command == "=" && n > 0 && myFct.AWLCode[n - 1].Command == Memnoic.opBLD[myOpt.Memnoic])
                        {
                            //Do nothing, but this line needs to be there!
                        }
                        else if (row.Command == Memnoic.opU[myOpt.Memnoic] || row.Command == Memnoic.opUN[myOpt.Memnoic] ||
                                 row.Command == Memnoic.opO[myOpt.Memnoic] || row.Command == Memnoic.opON[myOpt.Memnoic] ||
                                 row.Command == Memnoic.opO[myOpt.Memnoic] || row.Command == Memnoic.opON[myOpt.Memnoic] ||
                                 row.Command == Memnoic.opX[myOpt.Memnoic] || row.Command == Memnoic.opXN[myOpt.Memnoic] ||
                                 row.Command == Memnoic.opL[myOpt.Memnoic])
                        {
                            akPar = row.Parameter;
                        }
                        else if (row.Command == "AUF")
                        {
                            db = row.Parameter + ".";
                        }
                        else if (row.Command == "CLR")
                        {
                            akPar = "FALSE";
                        }
                        else if (row.Command == "SET")
                        {
                            akPar = "TRUE";
                        }
                        else if ((row.Command == "=") && akPar != "")
                        {
                            Parameters.Add(
                                "P#V " +
                                (row.Parameter.Replace("L", "").Replace("W", "").Replace("B", "").Replace("D", "")),
                                db + akPar);
                            akPar = "";
                            db = "";
                        }
                        else if (row.Command == Memnoic.opT[myOpt.Memnoic] && akPar != "")
                        {
                            Parameters.Add(
                                "P#V " +
                                (row.Parameter.Replace("L", "").Replace("W", "").Replace("B", "").Replace("D", "")) + ".0",
                                db + akPar);
                            akPar = "";
                            db = "";
                        }
                        else if (row.Command == Memnoic.opUC[myOpt.Memnoic] && newRow == null)
                        {
                            //Block Interface auslesen (von FC oder vom Programm)
                            //myFld.BlocksOfflineFolder.GetBlock()
                            PLCDataRow para = myFld.BlocksOfflineFolder.GetInterface(row.Parameter);

                            newRow = new PLCFunctionBlockRow();
                            newRow.Command = Memnoic.opCALL[myOpt.Memnoic];
                            newRow.Parameter = row.Parameter;
                            newRow.ExtParameter = new List<string>();
                            for (int i = 0; i < row.ExtParameter.Count; i++)
                            {
                                string s = row.ExtParameter[i];

                                string parnm = "";
                                PLCDataRow akRow = Parameter.GetFunctionParameterFromNumber(para, i);
                                parnm = akRow.Name + ":=";

                                if (akRow != null)
                                {
                                    int lokaldata_address = (int)Convert.ToDouble(s.Substring(4).Replace('.', ','));

                                    if (akRow.DataType == PLCDataRowType.STRING || akRow.DataType == PLCDataRowType.DATE_AND_TIME ||
                                        akRow.DataType == PLCDataRowType.STRUCT || akRow.DataType == PLCDataRowType.UDT ||
                                        akRow.DataType == PLCDataRowType.POINTER || akRow.IsArray)
                                    {
                                        string p1 = Parameters["P#V " + (lokaldata_address + 0).ToString() + ".0"];
                                        string p2 = Parameters["P#V " + (lokaldata_address + 2).ToString() + ".0"];
                                        
                                        string tmp = "";
                                        //tmp += p4.Substring(2);
                                        tmp += p2;
                                        newRow.ExtParameter.Add(parnm + tmp);
                                    }
                                    else if (akRow.DataType == PLCDataRowType.ANY)
                                    {
                                        string p1 = Parameters["P#V " + (lokaldata_address + 0).ToString() + ".0"];
                                        string p2 = Parameters["P#V " + (lokaldata_address + 2).ToString() + ".0"];
                                        string p3 = Parameters["P#V " + (lokaldata_address + 4).ToString() + ".0"];
                                        string p4 = Parameters["P#V " + (lokaldata_address + 6).ToString() + ".0"];

                                        string tmp = "P#";
                                        if (p3 != "0")
                                            tmp += "DB" + p3 + ".";
                                        tmp += p4.Substring(2);
                                        tmp += " BYTE "; //Todo Byte 1 noch auswerten ob BYTE!
                                        tmp += p2;
                                        newRow.ExtParameter.Add(parnm + tmp);
                                    }
                                    else
                                    {
                                        if (Parameters.ContainsKey(s))
                                            newRow.ExtParameter.Add(parnm + Parameters[s]);
                                        else
                                        {
                                            if (akRow.DataType == PLCDataRowType.BOOL)
                                                newRow.ExtParameter.Add(parnm + s.Substring(2));
                                            else if (akRow.DataType == PLCDataRowType.BLOCK_DB)
                                                newRow.ExtParameter.Add(parnm + "DB" + lokaldata_address.ToString());
                                            else if (akRow.DataType == PLCDataRowType.BLOCK_FB)
                                                newRow.ExtParameter.Add(parnm + "FB" + lokaldata_address.ToString());
                                            else if (akRow.DataType == PLCDataRowType.BLOCK_FC)
                                                newRow.ExtParameter.Add(parnm + "FC" + lokaldata_address.ToString());
                                            else if (akRow.DataType == PLCDataRowType.BLOCK_SDB)
                                                newRow.ExtParameter.Add(parnm + "SDB" + lokaldata_address.ToString());
                                            else if (akRow.DataType == PLCDataRowType.TIMER)
                                                newRow.ExtParameter.Add(parnm + "T" + lokaldata_address.ToString());
                                            else if (akRow.DataType == PLCDataRowType.COUNTER)
                                                newRow.ExtParameter.Add(parnm + "Z" + lokaldata_address.ToString()); //todo use memnoic for Z
                                            else if (akRow.ByteLength == 1)
                                                newRow.ExtParameter.Add(parnm + s.Substring(2, 1) + "B" + lokaldata_address.ToString());
                                            else if (akRow.ByteLength == 2)
                                                newRow.ExtParameter.Add(parnm + s.Substring(2, 1) + "W" + lokaldata_address.ToString());
                                            else if (akRow.ByteLength == 4)
                                                newRow.ExtParameter.Add(parnm + s.Substring(2, 1) + "D" + lokaldata_address.ToString());
                                            else
                                            {

                                            }
                                            
                                        }
                                    }
                                }
                            }

                        }
                        else if (row.Command == Memnoic.opBLD[myOpt.Memnoic] && row.Parameter == "2" && newRow != null)
                        {
                            newRow.CombinedCommands = tempList;
                            retVal.Add(newRow);
                            Parameters.Clear();
                            tempList = new List<PLCFunctionBlockRow>();
                            //Do nothing, but this line needs to be there!
                        }
                        else
                        {
                            retVal.AddRange(tempList);
                            tempList.Clear();
                            inBld = 0;
                        }
                    }
                    else
                    {
                        retVal.Add(row);
                    }

                }
                myFct.AWLCode = retVal;
            }
        }
    }
}
