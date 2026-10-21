using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    public static class FBStaticAccessConverter
    {
        public static void ReplaceStaticAccess(S7FunctionBlock myFct, S7ProgrammFolder myFld, S7ConvertingOptions myOpt)
        {
            if (myOpt.ReplaceDIAccessesWithSymbolNames && myFct.BlockType == PLCBlockType.FB)
            {
                bool largeAccess = false;

                // Only replace operands in place. Unsupported AR2 sequences must
                // retain every instruction, including sequences at the end of a block.
                foreach (var functionBlockRow in myFct.AWLCode)
                {
                    if (functionBlockRow.Command == "TAR2")
                    {
                        largeAccess = true;
                    }
                    else if (functionBlockRow.Command == "+AR2" && largeAccess)
                    {
                        // Offset-based symbolic replacement is not implemented.
                        continue;
                    }
                    else if (((S7FunctionBlockRow)functionBlockRow).Parameter.Contains("[AR2,P#") && ((S7FunctionBlockRow)functionBlockRow).Parameter.Substring(0, 2) == "DI" && !largeAccess)
                    {
                        string para = ((S7FunctionBlockRow)functionBlockRow).Parameter;
                        ByteBitAddress adr = new ByteBitAddress(para.Substring(10, para.Length - 11));
                        var parRow = S7DataRow.GetDataRowWithAddress(myFct.Parameter, adr, true);
                        if (parRow != null)
                        {
                            byte[] tmp = ((S7FunctionBlockRow)functionBlockRow).MC7;
                            ((S7FunctionBlockRow)functionBlockRow).Parameter = "#" + parRow.StructuredName.Substring(parRow.StructuredName.IndexOf('.') + 1);
                            ((S7FunctionBlockRow)functionBlockRow).MC7 = tmp;
                        }
                    }
                    else if (((S7FunctionBlockRow)functionBlockRow).Parameter.Contains("[AR2,P#") && ((S7FunctionBlockRow)functionBlockRow).Parameter.Substring(0, 2) == "DI" && largeAccess)
                    {
                        // Keep the indirect access unchanged until its offset can be resolved.
                        largeAccess = false;
                    }
                    else if (functionBlockRow.Command == "LAR2")
                    {
                        continue;
                    }
                    else
                    {
                        largeAccess = false;
                    }
                }
            }
        }
    }
}
