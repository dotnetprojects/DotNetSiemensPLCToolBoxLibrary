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
                    else if (inBld == 1)
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
                        else if ((row.Command == "=" || row.Command == Memnoic.opT[myOpt.Memnoic]) && akPar != "")
                        {
                            Parameters.Add("P#V " + (row.Parameter.Replace("L", "").Replace("W", "").Replace("B", "").Replace("D", "")), akPar);
                            akPar = "";
                        }
                        else if (row.Command == Memnoic.opUC[myOpt.Memnoic] && newRow == null)
                        {
                            //Block Interface auslesen (von FC oder vom Programm)
                            //myFld.BlocksOfflineFolder.GetBlock()
                            newRow = new PLCFunctionBlockRow();
                            newRow.Command = Memnoic.opCALL[myOpt.Memnoic];
                            newRow.Parameter = row.Parameter;
                            newRow.ExtParameter = new List<string>();
                            for (int i = 0; i < row.ExtParameter.Count; i++)
                            {
                                string s = row.ExtParameter[i];
                                if (Parameters.ContainsKey(s))
                                    newRow.ExtParameter.Add(Parameters[s]);
                                else
                                    newRow.ExtParameter.Add(s);
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
