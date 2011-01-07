using System;
using System.Collections.Generic;
using System.Text;
using LibNoDaveConnectionLibrary.DataTypes.Blocks;
using LibNoDaveConnectionLibrary.DataTypes.Step7Project;

namespace LibNoDaveConnectionLibrary.MC7
{
    static class LocalDataConverter
    {
        public static void ConvertLocaldataToSymbols(PLCFunctionBlock myFct, MC7ConvertingOptions myOpt)
        {          
            if (myOpt.ReplaceLokalDataAddressesWithSymbolNames)
            {
                List<PLCDataRow> rows = null;
                Dictionary<String, String> parLst = new Dictionary<string, string>();

                if (myFct.Parameter != null && myFct.Parameter.Children!=null)
                    foreach (var plcDataRow in myFct.Parameter.Children)
                    {
                        if (plcDataRow.Name == "TEMP")
                        {
                            PLCDataRow tmpRw = plcDataRow._GetExpandedChlidren(new PLCDataBlockExpandOptions() {ExpandCharArrays = true})[0];
                            rows = PLCDataRow.GetChildrowsAsList(tmpRw);
                            break;
                        }
                    }

                if (rows != null)
                {
                    foreach (var plcDataRow in rows)
                    {
                        if (plcDataRow.DataType != PLCDataRowType.STRUCT && plcDataRow.DataType != PLCDataRowType.UDT && plcDataRow.DataType != PLCDataRowType.FB)
                            parLst.Add("P#L" + plcDataRow.BlockAddress.ToString(), "P##" + plcDataRow.StructuredName);
                        string tmp = plcDataRow.GetSymbolicAddress();
                        if (tmp != null)
                        {
                            parLst.Add("L" + tmp.Replace("X", ""), "#" + plcDataRow.StructuredName);
                        }
                    }
                }
                foreach (var plcFunctionBlockRow in myFct.AWLCode)
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
