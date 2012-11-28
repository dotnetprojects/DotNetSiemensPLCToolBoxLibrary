using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.Source
{
    public static class AWLSourceParser
    {
        /// <summary>
        /// Parses the AWL Text and returns the S7Blocks
        /// </summary>
        /// <param name="AWL"></param>
        /// <param name="SymbolTable">Can be null</param>
        /// <returns></returns>
        public static List<S7Block> ParseAWL(string AWL, string SymbolTable)
        {
            var retVal = new List<S7Block>();

            var rd = new StringReader(AWL);

            string line;

            bool withinBlock = false;

            S7Block akBlock = null;
            while ((line = rd.ReadLine()) != null)
            {
                var trimmedLine = line.TrimStart();

                if (!withinBlock && trimmedLine.StartsWith("TYPE"))
                {
                    akBlock = new S7DataBlock() { BlockType = DataTypes.PLCBlockType.UDT, BlockLanguage = PLCLanguage.DB };
                    retVal.Add(akBlock);
                }
                else if (!withinBlock && trimmedLine.StartsWith("DATA_BLOCK"))
                {
                    akBlock = new S7DataBlock() { BlockType = DataTypes.PLCBlockType.DB, BlockLanguage = PLCLanguage.DB };
                    retVal.Add(akBlock);
                }
                else if (!withinBlock && trimmedLine.StartsWith("FUNCTION"))
                {
                    akBlock = new S7FunctionBlock() { BlockType = DataTypes.PLCBlockType.FC, BlockLanguage = PLCLanguage.AWL};
                    retVal.Add(akBlock);

                    var pos1 = trimmedLine.LastIndexOf(':');
                    var symOrDirectName = trimmedLine.Substring(8, pos1 - 8).Trim();
                    var type = trimmedLine.Substring(pos1).Trim();

                    S7DataRow parameterRoot = new S7DataRow("ROOTNODE", S7DataRowType.STRUCT, akBlock);
                    S7DataRow parameterIN = new S7DataRow("IN", S7DataRowType.STRUCT, akBlock);
                    S7DataRow parameterOUT = new S7DataRow("OUT", S7DataRowType.STRUCT, akBlock);
                    S7DataRow parameterINOUT = new S7DataRow("IN_OUT", S7DataRowType.STRUCT, akBlock);
                    S7DataRow parameterSTAT = new S7DataRow("STATIC", S7DataRowType.STRUCT, akBlock);
                    S7DataRow parameterTEMP = new S7DataRow("TEMP", S7DataRowType.STRUCT, akBlock);

                    parameterOUT.Add(new S7DataRow("RET_VAL", (S7DataRowType)Enum.Parse(typeof(S7DataRowType), type), akBlock));

                    parameterRoot.Children.Add(parameterIN);
                    parameterRoot.Children.Add(parameterOUT);
                    parameterRoot.Children.Add(parameterINOUT);
                    parameterRoot.Children.Add(parameterSTAT);
                    parameterRoot.Children.Add(parameterTEMP);                    
                }
                else if (!withinBlock && trimmedLine.StartsWith("FUNCTION_BLOCK"))
                {
                    akBlock = new S7FunctionBlock() { BlockType = DataTypes.PLCBlockType.FB, BlockLanguage = PLCLanguage.AWL };
                    retVal.Add(akBlock);

                    S7DataRow parameterRoot = new S7DataRow("ROOTNODE", S7DataRowType.STRUCT, akBlock);
                    S7DataRow parameterIN = new S7DataRow("IN", S7DataRowType.STRUCT, akBlock);
                    S7DataRow parameterOUT = new S7DataRow("OUT", S7DataRowType.STRUCT, akBlock);
                    S7DataRow parameterINOUT = new S7DataRow("IN_OUT", S7DataRowType.STRUCT, akBlock);
                    S7DataRow parameterTEMP = new S7DataRow("TEMP", S7DataRowType.STRUCT, akBlock);

                    parameterRoot.Children.Add(parameterIN);
                    parameterRoot.Children.Add(parameterOUT);
                    parameterRoot.Children.Add(parameterINOUT);
                    parameterRoot.Children.Add(parameterTEMP);     
                }
            }
            return null;
        }
    }
}
