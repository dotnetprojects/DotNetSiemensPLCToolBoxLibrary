using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    public static class FBStaticAccessConverter
    {
        public static void ReplaceStaticAccess(S7FunctionBlock myFct, S7ProgrammFolder myFld, S7ConvertingOptions myOpt)
        {
            if (myOpt.ReplaceDIAccessesWithSymbolNames && myFct.BlockType==PLCBlockType.FB)
            {
                List<FunctionBlockRow> retVal = new List<FunctionBlockRow>();
                List<FunctionBlockRow> tempList = new List<FunctionBlockRow>();

                bool LargeAccess = false;
                int add_adresse = 0;

                foreach (var functionBlockRow in myFct.AWLCode)
                {
                    if (functionBlockRow.Command == "TAR2")
                    {
                        tempList.Add(functionBlockRow);
                        LargeAccess = true;
                    }
                    else if (functionBlockRow.Command == "+AR2" && LargeAccess)
                    {
                        tempList.Add(functionBlockRow);
                        add_adresse += Convert.ToInt32(Convert.ToDouble(((S7FunctionBlockRow) functionBlockRow).Parameter.Substring(2), new NumberFormatInfo() {NumberDecimalSeparator = "."}));
                    }
                    else if (((S7FunctionBlockRow)functionBlockRow).Parameter.Contains("[AR2,P#") && ((S7FunctionBlockRow)functionBlockRow).Parameter.Substring(0, 2) == "DI" && !LargeAccess)
                    {
                        string para = ((S7FunctionBlockRow) functionBlockRow).Parameter;
                        ByteBitAddress adr = new ByteBitAddress(para.Substring(10, para.Length - 11));
                        var parRow = S7DataRow.GetDataRowWithAddress(myFct.Parameter, adr);
                        if (parRow!=null)
                        {
                            byte[] tmp = ((S7FunctionBlockRow) functionBlockRow).MC7;
                            ((S7FunctionBlockRow) functionBlockRow).Parameter = "#" + parRow.StructuredName.Substring(parRow.StructuredName.IndexOf('.') + 1);
                            ((S7FunctionBlockRow) functionBlockRow).MC7 = tmp;
                        }
                        retVal.Add(functionBlockRow);
                    }
                    else if (((S7FunctionBlockRow)functionBlockRow).Parameter.Contains("[AR2,P#") && ((S7FunctionBlockRow)functionBlockRow).Parameter.Substring(0, 2) == "DI" && LargeAccess)
                    {
                        /*
                        string para = ((S7FunctionBlockRow)functionBlockRow).Parameter;
                        ByteBitAddress adr = new ByteBitAddress(para.Substring(10, para.Length - 11));
                        adr.ByteAddress += add_adresse;
                        var parRow = S7DataRow.GetDataRowWithAddress(myFct.Parameter, adr);
                        if (parRow != null)
                        {
                            byte[] tmp = ((S7FunctionBlockRow)functionBlockRow).MC7;
                            ((S7FunctionBlockRow)functionBlockRow).Parameter = "#" + parRow.StructuredName;
                            ((S7FunctionBlockRow)functionBlockRow).MC7 = tmp;
                        }
                        retVal.Add(functionBlockRow);
                         * */
                    }
                    else if (functionBlockRow.Command=="LAR2")
                    {

                    }
                    else
                    {
                        LargeAccess = false;
                        retVal.AddRange(tempList);
                        tempList.Clear();
                        retVal.Add(functionBlockRow);
                    }
                }

                myFct.AWLCode = retVal;
            }            
        }
    }
}
