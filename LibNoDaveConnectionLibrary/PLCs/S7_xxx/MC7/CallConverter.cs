using System;
using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    static class CallConverter
    {
        //In this Class a UC is converted to a Call and also backwards...
        public static void ConvertUCToCall(S7FunctionBlock myFct, S7ProgrammFolder myFld, MC7ConvertingOptions myOpt, byte[] addInfoFromBlock)
        {
            if (myOpt.GenerateCallsfromUCs)
            {
                int inBld = 0; //1=nach BLD 1
                S7FunctionBlockRow newRow = null;

                Dictionary<string, string> Parameters = new Dictionary<string, string>();
                List<S7FunctionBlockRow> retVal = new List<S7FunctionBlockRow>();
                List<S7FunctionBlockRow> tempList = new List<S7FunctionBlockRow>();

                string akPar = "";

                string db = "";

                for (int n = 0; n < myFct.AWLCode.Count; n++)
                {
                    S7FunctionBlockRow row = myFct.AWLCode[n];
                    if (row.Command == Memnoic.opBLD[myOpt.Memnoic] && ( row.Parameter == "1" ||  row.Parameter == "7") && inBld==0)
                    {
                        retVal.AddRange(tempList);
                        tempList.Clear();

                        Parameters.Clear();
                        db = "";

                        inBld = Convert.ToInt32(row.Parameter);
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
                            S7DataRow para = myFld.BlocksOfflineFolder.GetInterface(row.Parameter);

                            newRow = new S7FunctionBlockRow();
                            newRow.Command = Memnoic.opCALL[myOpt.Memnoic];
                            newRow.Parameter = row.Parameter;
                            newRow.ExtParameter = new List<string>();
                            for (int i = 0; i < row.ExtParameter.Count; i++)
                            {
                                string s = row.ExtParameter[i];

                                string parnm = "";
                                S7DataRow akRow = Parameter.GetFunctionParameterFromNumber(para, i);
                                if (akRow != null)
                                    parnm = akRow.Name + ":=";
                                else
                                    parnm = "$$undef:=";

                                if (akRow != null)
                                {
                                    int posL = s.IndexOf(' ');
                                    int ak_address = 0;
                                    if (posL >= 0)
                                        ak_address = Convert.ToInt32(s.Substring(posL + 1).Split('.')[0]);
                                    else
                                    {
                                        ak_address = Convert.ToInt32(s.Substring(2).Split('.')[0])*8 +
                                                     Convert.ToInt32(s.Substring(2).Split('.')[1]);
                                    }

                                    int lokaldata_address = -1;
                                    if (s.Substring(0, 3) == "P#V")
                                        lokaldata_address = Convert.ToInt32(s.Substring(4).Split('.')[0]);

                                    if (akRow.DataType == S7DataRowType.STRING || akRow.DataType == S7DataRowType.DATE_AND_TIME ||
                                        akRow.DataType == S7DataRowType.STRUCT || akRow.DataType == S7DataRowType.UDT ||
                                        akRow.DataType == S7DataRowType.POINTER || akRow.IsArray)
                                    {
                                        string p1 = Parameters["P#V " + (lokaldata_address + 0).ToString() + ".0"];
                                        string p2 = Parameters["P#V " + (lokaldata_address + 2).ToString() + ".0"];
                                        
                                        string tmp = "";
                                        //tmp += p4.Substring(2);
                                        tmp += p2;
                                        newRow.ExtParameter.Add(parnm + tmp);
                                    }
                                    else if (akRow.DataType == S7DataRowType.ANY)
                                    {
                                        string tmp = s;
                                        if (Parameters.ContainsKey("P#V " + (lokaldata_address + 0).ToString() + ".0") &&
                                            Parameters.ContainsKey("P#V " + (lokaldata_address + 2).ToString() + ".0") &&
                                            Parameters.ContainsKey("P#V " + (lokaldata_address + 4).ToString() + ".0") &&
                                            Parameters.ContainsKey("P#V " + (lokaldata_address + 6).ToString() + ".0"))
                                        {
                                            string p1 = Parameters["P#V " + (lokaldata_address + 0).ToString() + ".0"];
                                            string p2 = Parameters["P#V " + (lokaldata_address + 2).ToString() + ".0"];
                                            string p3 = Parameters["P#V " + (lokaldata_address + 4).ToString() + ".0"];
                                            string p4 = Parameters["P#V " + (lokaldata_address + 6).ToString() + ".0"];

                                            tmp = "P#";
                                            if (p3 != "0")
                                                tmp += "DB" + p3 + ".";
                                            tmp += p4.Substring(2);
                                            tmp += " BYTE "; //Todo Byte 1 noch auswerten ob typ überhaupt BYTE!
                                            tmp += p2;
                                        }
                                        newRow.ExtParameter.Add(parnm + tmp);
                                    }
                                    else
                                    {
                                        if (Parameters.ContainsKey(s))
                                        {
                                            string par = Parameters[s];
                                            if (akRow.DataType == S7DataRowType.S5TIME && par[0] >= '0' && par[0] <= '9')
                                            {
                                                newRow.ExtParameter.Add(parnm +
                                                                        Helper.GetS5Time(
                                                                            BitConverter.GetBytes(Convert.ToInt32(par))[1],
                                                                            BitConverter.GetBytes(Convert.ToInt32(par))[0]));
                                            }
                                            else if (akRow.DataType == S7DataRowType.TIME && par[0] >= '0' && par[0] <= '9')
                                            {
                                                newRow.ExtParameter.Add(parnm +
                                                                        Helper.GetDTime(BitConverter.GetBytes(Convert.ToInt32(par)),0));
                                            }
                                            else if (akRow.DataType == S7DataRowType.CHAR && par[0] == 'B')
                                            {
                                                newRow.ExtParameter.Add(parnm + "'" +
                                                                        (char) Int32.Parse(par.Substring(5), System.Globalization.NumberStyles.AllowHexSpecifier) + "'");                                                
                                            }
                                            else
                                                newRow.ExtParameter.Add(parnm + Parameters[s]);
                                        }
                                        else
                                        {
                                            if (akRow.DataType == S7DataRowType.BOOL)
                                                newRow.ExtParameter.Add(parnm + s.Substring(2));
                                            else if (akRow.DataType == S7DataRowType.BLOCK_DB)
                                                newRow.ExtParameter.Add(parnm + "DB" + ak_address.ToString());
                                            else if (akRow.DataType == S7DataRowType.BLOCK_FB)
                                                newRow.ExtParameter.Add(parnm + "FB" + ak_address.ToString());
                                            else if (akRow.DataType == S7DataRowType.BLOCK_FC)
                                                newRow.ExtParameter.Add(parnm + "FC" + ak_address.ToString());
                                            else if (akRow.DataType == S7DataRowType.BLOCK_SDB)
                                                newRow.ExtParameter.Add(parnm + "SDB" + ak_address.ToString());
                                            else if (akRow.DataType == S7DataRowType.TIMER)
                                                newRow.ExtParameter.Add(parnm + "T" + ak_address.ToString());
                                            else if (akRow.DataType == S7DataRowType.COUNTER)
                                                newRow.ExtParameter.Add(parnm + "Z" + ak_address.ToString()); //todo use memnoic for Z                                                                                        
                                            else
                                            {                                                
                                                string ber = "";
                                                if (s.Substring(0,5) == "P#DBX")
                                                    ber = "DB";
                                                else if (s.Substring(0,5) == "P#DIX")
                                                    ber = "DI";
                                                else
                                                    ber = s.Substring(2, 1);

                                                if (akRow.ByteLength == 1)
                                                    ber += "B";
                                                else if (akRow.ByteLength == 2)
                                                    ber += "W";
                                                else if (akRow.ByteLength == 4)
                                                    ber += "D";
                                                
                                                newRow.ExtParameter.Add(parnm +
                                                                        ber.Replace('V', 'L') +
                                                                        ak_address.ToString());

                                            }

                                        }
                                    }
                                }
                            }
                        }
                        else if (row.Command == Memnoic.opBLD[myOpt.Memnoic] && (row.Parameter == "2" || row.Parameter == "8") && newRow != null)
                        {
                            newRow.CombinedCommands = tempList;
                            retVal.Add(newRow);
                            Parameters.Clear();
                            tempList = new List<S7FunctionBlockRow>();                            
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
