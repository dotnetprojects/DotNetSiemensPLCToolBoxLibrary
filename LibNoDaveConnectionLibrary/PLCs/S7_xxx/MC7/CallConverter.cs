using System;
using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    static class CallConverter
    {
        //In this Class a UC is converted to a Call and also backwards...
        public static void ConvertUCToCall(S7FunctionBlock myFct, S7ProgrammFolder myFld, BlocksOfflineFolder myblkFld, S7ConvertingOptions myOpt, byte[] addInfoFromBlock)
        {
            if (myOpt.GenerateCallsfromUCs)
            {
                int inBld = 0; //1=nach BLD 1
                S7FunctionBlockRow newRow = null;

                Dictionary<string, string> Parameters = new Dictionary<string, string>();
                List<FunctionBlockRow> retVal = new List<FunctionBlockRow>();
                List<FunctionBlockRow> tempList = new List<FunctionBlockRow>();

                string akPar = "";
                string db = "";
                string label = "";
                bool afterCall = false;
                S7FunctionBlockRow callRow = null;

                for (int n = 0; n < myFct.AWLCode.Count; n++)
                {
                    S7FunctionBlockRow row = (S7FunctionBlockRow)myFct.AWLCode[n];
                    if (row.Command == Mnemonic.opBLD[(int)myOpt.Mnemonic] && ( row.Parameter == "1" ||  row.Parameter == "7") && inBld==0)
                    {
                        retVal.AddRange(tempList);
                        tempList.Clear();

                        Parameters.Clear();
                        db = "";

                        label = row.Label;

                        inBld = Convert.ToInt32(row.Parameter);
                        newRow = null;
                        afterCall = false;
                        callRow = null;
                        tempList.Add(row);
                    }
                    else if (inBld > 0)
                    {
                        tempList.Add(row);
                        if (row.Command == "=" && n > 0 && myFct.AWLCode[n - 1].Command == Mnemonic.opBLD[(int)myOpt.Mnemonic])
                        {
                            //Do nothing, but this line needs to be there!
                        }
                        else if (row.Command == Mnemonic.opU[(int)myOpt.Mnemonic] || row.Command == Mnemonic.opUN[(int)myOpt.Mnemonic] ||
                                 row.Command == Mnemonic.opO[(int)myOpt.Mnemonic] || row.Command == Mnemonic.opON[(int)myOpt.Mnemonic] ||
                                 row.Command == Mnemonic.opO[(int)myOpt.Mnemonic] || row.Command == Mnemonic.opON[(int)myOpt.Mnemonic] ||
                                 row.Command == Mnemonic.opX[(int)myOpt.Mnemonic] || row.Command == Mnemonic.opXN[(int)myOpt.Mnemonic] ||
                                 row.Command == Mnemonic.opL[(int)myOpt.Mnemonic])
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
                            if (afterCall == false)
                            {
                                string key = row.Parameter.Replace("L", "").Replace("W", "").Replace("B", "").Replace("D", "");
                                if (!Parameters.ContainsKey(key))
                                    Parameters.Add("P#V " + key, db + akPar);
                            }
                            else
                            {
                                string key = akPar.Replace("L", "").Replace("W", "").Replace("B", "").Replace("D", "");
                                if (!Parameters.ContainsKey(key))
                                    Parameters.Add("P#V " + key, db + row.Parameter);
                            }
                            akPar = "";
                            db = "";
                        }
                        else if (row.Command == Mnemonic.opT[(int)myOpt.Mnemonic] && akPar != "")
                        {
                            if (afterCall == false)
                            {
                                string key = row.Parameter.Replace("L", "").Replace("W", "").Replace("B", "").Replace("D", "");
                                if (!Parameters.ContainsKey(key))
                                    Parameters.Add("P#V " + key + ".0", db + akPar);
                            }
                            else
                            {
                                string key = akPar.Replace("L", "").Replace("W", "").Replace("B", "").Replace("D", "");
                                if (!Parameters.ContainsKey(key))
                                    Parameters.Add("P#V " + key, db + row.Parameter);
                            }
                            akPar = "";
                            db = "";
                        }
                        else if (row.Command == Mnemonic.opUC[(int)myOpt.Mnemonic])
                        {
                            //Commands after a Call --> Out-Para
                            callRow = row;
                            afterCall = true;
                        }
                        else if (row.Command == Mnemonic.opBLD[(int)myOpt.Mnemonic] && (row.Parameter == "2" || row.Parameter == "8"))
                        {
                            //Block Interface auslesen (von FC oder vom Programm)
                            //myFld.BlocksOfflineFolder.GetBlock()
                            S7DataRow para = myblkFld.GetInterface(callRow.Parameter);

                            newRow = new S7FunctionBlockRow();
                            newRow.Command = Mnemonic.opCALL[(int)myOpt.Mnemonic];
                            newRow.Parameter = callRow.Parameter;
                            //newRow.ExtParameter = new List<string>();
                            newRow.CallParameter=new List<S7FunctionBlockParameter>();

                            for (int i = 0; i < callRow.ExtParameter.Count; i++)
                            {
                                string s = callRow.ExtParameter[i];

                                string parnm = "";
                                S7DataRow akRow = Parameter.GetFunctionParameterFromNumber(para, i);
                                if (akRow != null)
                                    parnm = akRow.Name + "";
                                else
                                    parnm = "$$undef";


                                S7FunctionBlockParameter newPar = new S7FunctionBlockParameter();
                                newPar.Name = parnm;
                                if (akRow != null)
                                {
                                    newPar.ParameterDataType = akRow.DataType;
                                    if (akRow.Parent.Name == "OUT")
                                        newPar.ParameterType = S7FunctionBlockParameterDirection.OUT;
                                    else if (akRow.Parent.Name == "IN_OUT")
                                        newPar.ParameterType = S7FunctionBlockParameterDirection.IN_OUT;
                                    else
                                        newPar.ParameterType = S7FunctionBlockParameterDirection.IN;
                                    //newPar.ParameterType
                                }

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
                                        if (p1 != "" && p1 != "0")
                                            tmp += "P#DB" + p1 + "." + p2.Substring(2);
                                        else
                                            tmp += p2;
                                        newPar.Value = tmp;
                                        newRow.CallParameter.Add(newPar);
                                        //newRow.ExtParameter.Add(parnm + tmp);
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
                                            tmp += " BYTE "; //Todo Parse Byte 1 if the Type is Byte!
                                            tmp += p2;
                                        }
                                        newPar.Value = tmp;
                                        newRow.CallParameter.Add(newPar);
                                        //newRow.ExtParameter.Add(parnm + tmp);
                                    }
                                    else
                                    {
                                        if (Parameters.ContainsKey(s))
                                        {
                                            string par = Parameters[s];
                                            if (akRow.DataType == S7DataRowType.S5TIME && par[0] >= '0' && par[0] <= '9')
                                            {
                                                newPar.Value = Helper.GetS5Time(
                                                                            BitConverter.GetBytes(Convert.ToInt32(par))[1],
                                                                            BitConverter.GetBytes(Convert.ToInt32(par))[0]);
                                                newRow.CallParameter.Add(newPar);
                                                //newRow.ExtParameter.Add(parnm +
                                                //                        Helper.GetS5Time(
                                                //                            BitConverter.GetBytes(Convert.ToInt32(par))[1],
                                                //                            BitConverter.GetBytes(Convert.ToInt32(par))[0]));
                                            }
                                            else if (akRow.DataType == S7DataRowType.TIME && par[0] >= '0' && par[0] <= '9')
                                            {
                                                newPar.Value = Helper.GetDTime(BitConverter.GetBytes(Convert.ToInt32(par)),0);
                                                newRow.CallParameter.Add(newPar);
                                                //newRow.ExtParameter.Add(parnm +
                                                //                        Helper.GetDTime(BitConverter.GetBytes(Convert.ToInt32(par)),0));
                                            }
                                            else if (akRow.DataType == S7DataRowType.CHAR && par[0] == 'B')
                                            {
                                                newPar.Value =
                                                    (char)
                                                    Int32.Parse(par.Substring(5),
                                                                System.Globalization.NumberStyles.AllowHexSpecifier) +
                                                    "'";
                                                newRow.CallParameter.Add(newPar);
                                                //newRow.ExtParameter.Add(parnm + "'" +
                                                //                        (char)Int32.Parse(par.Substring(5), System.Globalization.NumberStyles.AllowHexSpecifier) + "'");
                                            }
                                            else
                                            {
                                                newPar.Value = Parameters[s];
                                                newRow.CallParameter.Add(newPar);
                                                //newRow.ExtParameter.Add(parnm + Parameters[s]);
                                            }
                                        }
                                        else
                                        {
                                            if (akRow.DataType == S7DataRowType.BOOL)
                                            {
                                                newPar.Value = s.Substring(2).Replace('V', 'L');
                                                newRow.CallParameter.Add(newPar);
                                                //newRow.ExtParameter.Add(parnm + s.Substring(2).Replace('V', 'L'));
                                            }
                                            else if (akRow.DataType == S7DataRowType.BLOCK_DB)
                                            {
                                                newPar.Value = "DB" + ak_address.ToString();
                                                newRow.CallParameter.Add(newPar);
                                                //newRow.ExtParameter.Add(parnm + "DB" + ak_address.ToString());
                                            }
                                            else if (akRow.DataType == S7DataRowType.BLOCK_FB)
                                            {
                                                newPar.Value = "FB" + ak_address.ToString();
                                                newRow.CallParameter.Add(newPar);
                                                //newRow.ExtParameter.Add(parnm + "FB" + ak_address.ToString());
                                            }
                                            else if (akRow.DataType == S7DataRowType.BLOCK_FC)
                                            {
                                                newPar.Value = "FC" + ak_address.ToString();
                                                newRow.CallParameter.Add(newPar);
                                            }
                                            else if (akRow.DataType == S7DataRowType.BLOCK_SDB)
                                            {
                                                newPar.Value = "SDB" + ak_address.ToString();
                                                newRow.CallParameter.Add(newPar);
                                            }
                                            else if (akRow.DataType == S7DataRowType.TIMER)
                                            {
                                                newPar.Value = Mnemonic.adT[(int)myOpt.Mnemonic] + ak_address.ToString();
                                                newRow.CallParameter.Add(newPar);
                                            }
                                            else if (akRow.DataType == S7DataRowType.COUNTER)
                                            {
                                                newPar.Value = Mnemonic.adZ[(int)myOpt.Mnemonic] + ak_address.ToString();
                                                newRow.CallParameter.Add(newPar);                                                                                      
                                            }
                                            else
                                            {
                                                string ber = "";
                                                if (s.Substring(0, 5) == "P#DBX")
                                                    ber = "DB";
                                                else if (s.Substring(0, 5) == "P#DIX")
                                                    ber = "DI";
                                                else
                                                    ber = s.Substring(2, 1);

                                                if (akRow.ByteLength == 1)
                                                    ber += "B";
                                                else if (akRow.ByteLength == 2)
                                                    ber += "W";
                                                else if (akRow.ByteLength == 4)
                                                    ber += "D";

                                                newPar.Value = ber.Replace('V', 'L') + ak_address.ToString();
                                                newRow.CallParameter.Add(newPar);
                                                //newRow.ExtParameter.Add(parnm +
                                                //                        ber.Replace('V', 'L') +
                                                //                        ak_address.ToString());

                                            }

                                        }
                                    }
                                }
                            }

                            newRow.CombinedCommands = tempList;
                            newRow.Label = label;

                            int sz = 0;
                            foreach (var functionBlockRow in newRow.CombinedCommands)
                            {
                                sz += ((S7FunctionBlockRow) functionBlockRow).ByteSize;
                            }
                            byte[] mcges=new byte[sz];
                            sz = 0;
                            foreach (var functionBlockRow in newRow.CombinedCommands)
                            {
                                Array.Copy(((S7FunctionBlockRow) functionBlockRow).MC7, 0, mcges, sz, ((S7FunctionBlockRow) functionBlockRow).ByteSize);
                                sz += ((S7FunctionBlockRow)functionBlockRow).ByteSize;
                            }
                            newRow.MC7 = mcges;

                            retVal.Add(newRow);
                            Parameters.Clear();
                            tempList = new List<FunctionBlockRow>();
                            inBld = 0;
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
