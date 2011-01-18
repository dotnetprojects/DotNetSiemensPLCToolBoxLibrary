using System;
using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    static class LocalDataConverter
    {
        public static void ConvertLocaldataToSymbols(S7FunctionBlock myFct, S7ConvertingOptions myOpt)
        {          
            if (myOpt.ReplaceLokalDataAddressesWithSymbolNames)
            {
                List<S7DataRow> rows = null;
                Dictionary<String, String> parLst = new Dictionary<string, string>();

                if (myFct.Parameter != null && myFct.Parameter.Children!=null)
                    foreach (var plcDataRow in myFct.Parameter.Children)
                    {
                        if (plcDataRow.Name == "TEMP")
                        {
                            S7DataRow tmpRw = plcDataRow._GetExpandedChlidren(new S7DataBlockExpandOptions() {ExpandCharArrays = true})[0];
                            rows = S7DataRow.GetChildrowsAsList(tmpRw);
                            break;
                        }
                    }

                if (rows != null)
                {
                    foreach (var plcDataRow in rows)
                    {
                        if (plcDataRow.DataType != S7DataRowType.STRUCT && plcDataRow.DataType != S7DataRowType.UDT && plcDataRow.DataType != S7DataRowType.FB)
                            parLst.Add("P#L" + plcDataRow.BlockAddress.ToString(), "P##" + plcDataRow.StructuredName);
                        string tmp = plcDataRow.GetSymbolicAddress();
                        if (tmp != null)
                        {
                            parLst.Add("L" + tmp.Replace("X", ""), "#" + plcDataRow.StructuredName);
                        }
                    }
                }
                foreach (S7FunctionBlockRow plcFunctionBlockRow in myFct.AWLCode)
                {
                    if (!plcFunctionBlockRow.Parameter.Contains("'") && !plcFunctionBlockRow.Parameter.Contains("[AR")  && plcFunctionBlockRow.Parameter.Contains("["))
                    {
                        
                            int pos1 = plcFunctionBlockRow.Parameter.IndexOf("[")+1;
                            int pos2 = plcFunctionBlockRow.Parameter.IndexOf("]");
                            string par = plcFunctionBlockRow.Parameter.Substring(pos1, pos2 - pos1);
                            if (parLst.ContainsKey(par))
                            {
                                byte[] tmp = plcFunctionBlockRow.MC7;
                                plcFunctionBlockRow.Parameter = plcFunctionBlockRow.Parameter.Substring(0, pos1) + parLst[par] + "]";
                                plcFunctionBlockRow.MC7 = tmp;
                            }
                    }
                    else
                    {
                        string par = plcFunctionBlockRow.Parameter.Replace(" ", "");
                        if (parLst.ContainsKey(par))
                        {
                            byte[] tmp = plcFunctionBlockRow.MC7;
                            plcFunctionBlockRow.Parameter = "";

                            plcFunctionBlockRow.Parameter = parLst[par];

                            plcFunctionBlockRow.MC7 = tmp;
                        }
                    }
                    
                }
            }
        }
    }
}
