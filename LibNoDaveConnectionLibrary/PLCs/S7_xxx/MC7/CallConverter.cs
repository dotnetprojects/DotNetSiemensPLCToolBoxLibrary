using System;
using System.Collections.Generic;
using System.Globalization;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

using System.Linq;

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


                string registerDi = "DI";
                string registerDb = "";
                string registerAkku1 = "";
                string registerAkku2 = "";
                string registerAR1 = "";
                string registerAR2 = "";



                string diName = "";
                string akPar = "";
                string db = "";
                string label = "";
                string akku = "";
                bool afterCall = false;
                bool multiInstance = false;
                int multiInstanceOffset = 0;
                Pointer ar2Addr = new Pointer(0,0);

                S7FunctionBlockRow callRow = null;

                for (int n = 0; n < myFct.AWLCode.Count; n++)
                {
                    S7FunctionBlockRow row = (S7FunctionBlockRow)myFct.AWLCode[n];
                    if (row.Command == Mnemonic.opBLD[(int)myOpt.Mnemonic] && (row.Parameter == "1" || row.Parameter == "7" || row.Parameter == "3" || row.Parameter == "16" || row.Parameter == "14") && inBld == 0)
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
                        diName = "";
                        
                        multiInstance = false;
                        multiInstanceOffset = 0;
                        ar2Addr = new Pointer(0, 0);

                        tempList.Add(row);
                    }                    
                    else if (inBld == 1 || inBld == 7)
                    {
                        #region FC Aufruf
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
                        else if (row.Command == Mnemonic.opAUF[(int)myOpt.Mnemonic])
                        {
                            db = row.Parameter + ".";
                        }
                        else if (row.Command == Mnemonic.opCLR[(int)myOpt.Mnemonic])
                        {
                            akPar = "FALSE";
                        }
                        else if (row.Command == Mnemonic.opSET[(int)myOpt.Mnemonic] )
                        {
                            akPar = "TRUE";
                        }
                        else if (row.Command == Mnemonic.opTAR2[(int)myOpt.Mnemonic])
                        {
                            //look what we need to do here!!!
                        }
                        else if (row.Command == Mnemonic.opPAR2[(int)myOpt.Mnemonic])
                        {
                            //look what we need to do here!!!
                        }
                        else if (row.Command == Mnemonic.opLAR2[(int)myOpt.Mnemonic])
                        {
                            //look what we need to do here!!!
                        }
                        else if (row.Command == Mnemonic.opLAR1[(int)myOpt.Mnemonic])
                        {
                            //look what we need to do here!!!
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

                                 //Fix for replace db accesses with symbols
                                 if(myOpt.ReplaceDBAccessesWithSymbolNames && db != "")
                                 {
                                     string dbnr = db.Substring(0, db.Length - 1);
                                     string dbSymbol = Helper.TryGetSymbolFromOperand(myblkFld, dbnr);
                                     db = (dbSymbol != null ? "\"" + dbSymbol + "\"" : dbnr) + ".";
                                     akPar = Helper.TryGetStructuredName(myblkFld, dbnr, akPar);
                                 }
                                 //end fix
                                if (!Parameters.ContainsKey(key))
                                    Parameters.Add("P#V " + key + ".0", db + akPar);
                            }
                            else
                            {
                                string key = akPar.Replace("L", "").Replace("W", "").Replace("B", "").Replace("D", "");
                                if (!Parameters.ContainsKey("P#V " + key))
                                    Parameters.Add("P#V " + key, db + row.Parameter);
                            }
                            akPar = "";
                            db = "";
                        }
                        else if (row.Command == Mnemonic.opT[(int)myOpt.Mnemonic] && akPar == "")
                        {
                            //look what we need to do here!!!
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
                            S7DataRow para = myblkFld.GetInterface(callRow.Parameter);

                            newRow = new S7FunctionBlockRow();
                            newRow.Command = Mnemonic.opCALL[(int)myOpt.Mnemonic];
                            newRow.Parent = callRow.Parent;
                            newRow.Parameter = callRow.Parameter;
                            newRow.CallParameter = new List<S7FunctionBlockParameter>();

                            for (int i = 0; i < callRow.ExtParameter.Count; i++)
                            {
                                string s = callRow.ExtParameter[i];

                                string parnm = "";
                                S7DataRow akRow = Parameter.GetFunctionParameterFromNumber(para, i);
                                if (akRow != null)
                                    parnm = akRow.Name + "";
                                else
                                    parnm = "$$undef";


                                S7FunctionBlockParameter newPar = new S7FunctionBlockParameter(newRow);
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
                                        string p1 = "";
                                        string p2 = "";
                                        Parameters.TryGetValue("P#V " + (lokaldata_address + 0).ToString() + ".0", out p1);
                                        Parameters.TryGetValue("P#V " + (lokaldata_address + 2).ToString() + ".0", out p2);
                                        
                                        
                                        string tmp = "";
                                        if (p1 != "" && p1 != "0")
                                            tmp += "P#DB" + p1 + "." + (p2 == null ? "0" : p2.Substring(2));
                                        else
                                            tmp += p2;
                                        newPar.Value = tmp;
                                        newRow.CallParameter.Add(newPar);
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

                                            //Fix for wrong construction of any pointers
                                             var anyPtr = new S7AnyPointer(p1, p2, p3, p4);
                                             if (myOpt.ReplaceDBAccessesWithSymbolNames)
                                             {
                                                 //TODO: make the any pointer symbolic.
                                                 tmp = "" + anyPtr.ToString();
                                             }
                                             else
                                                 tmp = anyPtr.ToString();
                                             
                                             //end fix
                                             //tmp = "P#";
                                             //if (p3 != "0")
                                             //    tmp += "DB" + p3 + ".";
                                             //tmp += p4.Substring(2);
                                             //tmp += " BYTE "; //Todo Parse Byte 1 if the Type is Byte!
                                             //tmp += p2;
                                        }
                                        newPar.Value = tmp;
                                        newRow.CallParameter.Add(newPar);
                                    }
                                    else
                                    {
                                        if (Parameters.ContainsKey(s))
                                        {
                                            string par = Parameters[s];
                                            try
                                            {
                                                if (akRow.DataType == S7DataRowType.S5TIME && par[0] >= '0' && par[0] <= '9')
                                                {
                                                    newPar.Value = Helper.GetS5Time(
                                                                                BitConverter.GetBytes(Convert.ToInt32(par))[1],
                                                                                BitConverter.GetBytes(Convert.ToInt32(par))[0]);
                                                }
                                                else if (akRow.DataType == S7DataRowType.TIME && par[0] >= '0' && par[0] <= '9')
                                                {
                                                    newPar.Value = Helper.GetDTime(BitConverter.GetBytes(Convert.ToInt32(par)), 0);
                                                }
                                                else if (akRow.DataType == S7DataRowType.CHAR && par[0] == 'B')
                                                {
                                                    newPar.Value = (char)Int32.Parse(par.Substring(5), System.Globalization.NumberStyles.AllowHexSpecifier) + "'";
                                                }
                                                else if (akRow.DataType == S7DataRowType.REAL)
                                                {
                                                    var bt = new byte[4];
                                                    if (par.Length > 2 && int.TryParse(par.Substring(2), out var val))
                                                    {
                                                        libnodave.putS32at(bt, 0, val);
                                                        var real = libnodave.getFloatfrom(bt, 0);
                                                        newPar.Value = real.ToString("e6", CultureInfo.InvariantCulture);
                                                    }
                                                    else
                                                    {
                                                        newPar.Value = Parameters[s];
                                                    }
                                                }
                                                else
                                                {
                                                    newPar.Value = Parameters[s];
                                                }
                                            }
                                            catch(Exception ex)
                                            {
                                                newPar.Value = Parameters[s];
                                            }
                                        }
                                        else
                                        {
                                            if (akRow.DataType == S7DataRowType.BOOL)
                                            {
                                                newPar.Value = s.Substring(2).Replace('V', 'L');
                                            }
                                            else if (akRow.DataType == S7DataRowType.BLOCK_DB)
                                            {
                                                newPar.Value = "DB" + ak_address.ToString();
                                            }
                                            else if (akRow.DataType == S7DataRowType.BLOCK_FB)
                                            {
                                                newPar.Value = "FB" + ak_address.ToString();
                                            }
                                            else if (akRow.DataType == S7DataRowType.BLOCK_FC)
                                            {
                                                newPar.Value = "FC" + ak_address.ToString();
                                            }
                                            else if (akRow.DataType == S7DataRowType.BLOCK_SDB)
                                            {
                                                newPar.Value = "SDB" + ak_address.ToString();
                                            }
                                            else if (akRow.DataType == S7DataRowType.TIMER)
                                            {
                                                newPar.Value = Mnemonic.adT[(int)myOpt.Mnemonic] + ak_address.ToString();
                                            }
                                            else if (akRow.DataType == S7DataRowType.COUNTER)
                                            {
                                                newPar.Value = Mnemonic.adZ[(int)myOpt.Mnemonic] + ak_address.ToString();                                            
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

                                                 var access = ber.Replace('V', 'L') + ak_address;
 
                                                 //fix for temporary area not replaced with symbolnames
                                                 if(myOpt.ReplaceLokalDataAddressesWithSymbolNames)
                                                 {
                                                     var tempSymbol = S7DataRow.GetDataRowWithAddress(myFct.Parameter, new ByteBitAddress(ak_address, 0));
                                                     if (tempSymbol != null) access = "#" + tempSymbol.StructuredName.Replace("TEMP.","");
                                                 }
                                                 newPar.Value = access;
                                                
                                            }                                    
                                        }

                                        newRow.CallParameter.Add(newPar);
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

                            newRow = null;
                        }                                              
                        else
                        {
                            retVal.AddRange(tempList);
                            tempList.Clear();
                     
                            inBld = 0;
                        }
                        #endregion
                    }
                    else if (inBld == 3 || inBld == 16 || inBld == 14)
                    {                        
                        #region FB Aufruf
                        tempList.Add(row);
                        if (row.Command == "=" && n > 0 && myFct.AWLCode[n - 1].Command == Mnemonic.opBLD[(int)myOpt.Mnemonic])
                        {
                            //Do nothing, but this line needs to be there!
                        }
                        else if (row.Command == Mnemonic.opTDB[(int)myOpt.Mnemonic])
                        {
                            //Do nothing, but this line needs to be there!
                        }
                        else if (row.Command == Mnemonic.opBLD[(int)myOpt.Mnemonic] && row.Parameter=="11")
                        {
                            //whatever this BLD 11 in the FB Calls means...
                        }
                        else if (row.Command == Mnemonic.opTAR2[(int)myOpt.Mnemonic])
                        {
                            if (ar2Addr.MemoryArea==MemoryArea.None)
                                akPar = "P#DIX " + ar2Addr.ToString();
                            else
                                akPar = ar2Addr.ToString(myOpt.Mnemonic);
                        }
                        else if (row.Command == Mnemonic.opLAR2[(int)myOpt.Mnemonic])
                        {
                            if (row.Parameter.StartsWith("P#"))
                            {
                                ar2Addr = new Pointer(row.Parameter.Substring(2));
                            }
                            else
                            {
                                ar2Addr.ByteAddress = 0;
                                ar2Addr.BitAddress = 0;
                            }
                        }
                        else if (row.Command == Mnemonic.opPAR2[(int)myOpt.Mnemonic])
                        {
                            ar2Addr += new Pointer(row.Parameter);
                        }
                        else if (row.Command == Mnemonic.opAUF[(int)myOpt.Mnemonic] && (tempList.Count == 4))
                        {
                            diName = row.Parameter;
                        }
                        else if (row.Command == Mnemonic.opAUF[(int)myOpt.Mnemonic] && row.Parameter.Contains("[") && (tempList.Count == 6))
                        {
                            multiInstance = true;

                            diName = "";
                        }
                        else if (row.Command == Mnemonic.opU[(int)myOpt.Mnemonic] || row.Command == Mnemonic.opUN[(int)myOpt.Mnemonic] ||
                                 row.Command == Mnemonic.opO[(int)myOpt.Mnemonic] || row.Command == Mnemonic.opON[(int)myOpt.Mnemonic] ||
                                 row.Command == Mnemonic.opO[(int)myOpt.Mnemonic] || row.Command == Mnemonic.opON[(int)myOpt.Mnemonic] ||
                                 row.Command == Mnemonic.opX[(int)myOpt.Mnemonic] || row.Command == Mnemonic.opXN[(int)myOpt.Mnemonic] ||
                                 row.Command == Mnemonic.opL[(int)myOpt.Mnemonic])
                        {
                            akPar = db + row.Parameter;
                            db = "";
                        }
                        else if (row.Command == Mnemonic.opAUF[(int)myOpt.Mnemonic])
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
                                if (!Parameters.ContainsKey(row.Parameter))
                                    Parameters.Add(row.Parameter, akPar);
                            }
                            else
                            {
                                if (!Parameters.ContainsKey(akPar))
                                    Parameters.Add(akPar, row.Parameter);
                            }
                            akPar = "";
                            db = "";
                        }
                        else if (row.Command == Mnemonic.opT[(int)myOpt.Mnemonic] && akPar != "")
                        {
                            if (afterCall == false)
                            {
                                if (!Parameters.ContainsKey(row.Parameter))
                                    Parameters.Add(row.Parameter, akPar);
                            }
                            else
                            {
                                if (!Parameters.ContainsKey(akPar))
                                    Parameters.Add(akPar, row.Parameter);
                            }
                            akPar = "";
                            db = "";
                        }
                        else if (row.Command == Mnemonic.opT[(int)myOpt.Mnemonic])
                        {
                            if (afterCall == false)
                            {
                                if (!Parameters.ContainsKey(row.Parameter))
                                    Parameters.Add(row.Parameter, "");
                            }                            
                            akPar = "";
                            db = "";
                        }
                        else if (row.Command == Mnemonic.opUC[(int)myOpt.Mnemonic])
                        {
                            multiInstanceOffset = 0;
                            for (int j = tempList.Count - 2; j > -1; j--)
                            {
                                if (tempList[j].Command == Mnemonic.opPAR2[(int)myOpt.Mnemonic])
                                    multiInstanceOffset += (int)double.Parse((((S7FunctionBlockRow)tempList[j]).Parameter.Substring(2)), CultureInfo.InvariantCulture);
                                break;
                            }
                             
                            //Commands after a Call --> Out-Para
                            callRow = row;
                            afterCall = true;
                        }
                        else if (row.Command == Mnemonic.opBLD[(int)myOpt.Mnemonic] && (row.Parameter == "4" || row.Parameter == "17" || row.Parameter == "15"))
                        {
                            //Block Interface auslesen (von FC oder vom Programm)
                            S7DataRow para = myblkFld.GetInterface(callRow.Parameter);

                            newRow = new S7FunctionBlockRow();
                            newRow.Parent = callRow.Parent;
                            newRow.Command = Mnemonic.opCALL[(int)myOpt.Mnemonic];
                            newRow.Parameter = callRow.Parameter;
                            if (diName.Length > 2 && !diName.StartsWith("#"))
                                newRow.DiName = "DI" + int.Parse(diName.Substring(2));
                            else if (diName.StartsWith("#")) 
                                newRow.DiName = diName;
                            newRow.CallParameter = new List<S7FunctionBlockParameter>();


                            if (para != null)
                            { 
                            var allPar = para.Children.Where(itm => itm.Name == "IN" || itm.Name == "IN_OUT" || itm.Name == "OUT");
                                foreach (var s7DataRowMain in allPar)
                                {
                                    foreach (var s7DataRow in s7DataRowMain.Children)
                                    {
                                        S7FunctionBlockParameter newPar = new S7FunctionBlockParameter(newRow);
                                        newPar.Name = s7DataRow.Name;

                                        string par = null;
                                        if (!multiInstance) Parameters.TryGetValue(((S7DataRow)s7DataRow).BlockAddressInDbFormat.Replace("DB", "DI"), out par);
                                        else if (((S7DataRow)s7DataRow).DataType == S7DataRowType.ANY)
                                        {
                                            var anySizeAddr = ((S7DataRow)s7DataRow).BlockAddress.ByteAddress + multiInstanceOffset + 2;
                                            var anyPosAddr = ((S7DataRow)s7DataRow).BlockAddress.ByteAddress + multiInstanceOffset + 6;

                                            string anySize = "";
                                            string anyPos = "";
                                            Parameters.TryGetValue("DIW[AR2,P#" + anySizeAddr + ".0]", out anySize);
                                            Parameters.TryGetValue("DID[AR2,P#" + anyPosAddr + ".0]", out anyPos);
                                            par = anyPos + " BYTE " + anySize;
                                        }
                                        else
                                        {
                                            var addr = ((S7DataRow)s7DataRow).BlockAddressInDbFormat;
                                            if (!string.IsNullOrEmpty(addr))
                                            {
                                                var addrTp = addr.Substring(0, 3).Replace("DB", "DI");
                                                double bytepos = double.Parse(addr.Substring(3), CultureInfo.InvariantCulture) + multiInstanceOffset;
                                                Parameters.TryGetValue(addrTp + "[AR2,P#" + bytepos.ToString("0.0", CultureInfo.InvariantCulture) + "]", out par);
                                            }
                                        }

                                        if (par != null && par.Contains("[AR2"))
                                        {
                                            newPar.Value = par;
                                            var addr = par.Substring(10);
                                            addr = addr.Substring(0, addr.Length - 1);
                                            var pRow = S7DataRow.GetDataRowWithAddress(myFct.Parameter.Children.Where(itm => itm.Name != "TEMP").Cast<S7DataRow>(), new ByteBitAddress(addr));
                                            if (pRow != null) newPar.Value = ((S7DataRow)pRow).StructuredName.Substring(((S7DataRow)pRow).StructuredName.IndexOf('.') + 1);

                                        }
                                        else
                                        {
                                            switch (((S7DataRow)s7DataRow).DataType)
                                            {
                                                case S7DataRowType.BLOCK_DB:
                                                    newPar.Value = "DB" + par;
                                                    break;
                                                case S7DataRowType.BLOCK_FC:
                                                    newPar.Value = "FC" + par;
                                                    break;
                                                case S7DataRowType.BLOCK_FB:
                                                    newPar.Value = "FB" + par;
                                                    break;
                                                case S7DataRowType.TIMER:
                                                    newPar.Value = Mnemonic.adT[(int)myOpt.Mnemonic] + par.ToString();
                                                    break;
                                                case S7DataRowType.COUNTER:
                                                    newPar.Value = Mnemonic.adZ[(int)myOpt.Mnemonic] + par.ToString();
                                                    break;
                                                case S7DataRowType.TIME:
                                                    if (par != null && par.StartsWith("L#"))
                                                    {
                                                        var arr = BitConverter.GetBytes(Convert.ToInt32(par.Substring(2)));
                                                        Array.Reverse(arr);
                                                        newPar.Value = Helper.GetDTime(arr, 0);
                                                    }
                                                    else
                                                    {
                                                        newPar.Value = par;
                                                    }
                                                    break;
                                                default:
                                                    newPar.Value = par;
                                                    break;
                                            }
                                        }

                                        newRow.CallParameter.Add(newPar);
                                    }
                                }
                            }

                            
                            newRow.CombinedCommands = tempList;
                            newRow.Label = label;

                            int sz = 0;
                            foreach (var functionBlockRow in newRow.CombinedCommands)
                            {
                                sz += ((S7FunctionBlockRow)functionBlockRow).ByteSize;
                            }
                            byte[] mcges = new byte[sz];
                            sz = 0;
                            foreach (var functionBlockRow in newRow.CombinedCommands)
                            {
                                Array.Copy(((S7FunctionBlockRow)functionBlockRow).MC7, 0, mcges, sz, ((S7FunctionBlockRow)functionBlockRow).ByteSize);
                                sz += ((S7FunctionBlockRow)functionBlockRow).ByteSize;
                            }
                            newRow.MC7 = mcges;

                            retVal.Add(newRow);
                            Parameters.Clear();
                            tempList = new List<FunctionBlockRow>();
                            inBld = 0;

                            newRow = null;
                        }
                        else
                        {
                            retVal.AddRange(tempList);
                            tempList.Clear();

                            inBld = 0;
                        }
                        #endregion
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
