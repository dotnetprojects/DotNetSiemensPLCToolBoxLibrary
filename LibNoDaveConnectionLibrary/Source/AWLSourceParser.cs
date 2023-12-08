using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using System;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.Source
{
    public static class AWLSourceParser
    {
        public enum ParseStep
        {
            //Header
            ReadBlockType,

            ReadBlockNumberOrName,
            ReadBlockRetVal,
            ReadBlockRetValType,
            ParseHeaderRow,
            ParseTitle,
            ParseVersion,
            ParseAttributes,

            //Struktur & Parameter
            ParseStructure,

            ParsePara,

            //Aktualwerte oder AWL-Code
            ParseAWLCode,
        }

        /// <summary>
        /// Parses the AWL Text and returns the S7Blocks
        /// </summary>
        /// <param name="AWL"></param>
        /// <param name="SymbolTable">Can be null</param>
        /// <returns></returns>
        public static List<S7Block> ParseAWL(string AWL, string SymbolTable)
        {
            var retVal = new List<S7Block>();

            string txt = "";
            string blockType = "";
            string blockNumberOrName = "";
            string blockRetValType = "";
            bool startOfQuote = false;

            ParseStep step = ParseStep.ReadBlockType;

            S7DataRow akDataRow = null;

            S7Block akBlock = null;

            foreach (char c in AWL)
            {
                if (!startOfQuote && (c == ' ' || c == '\n' || c == '\r' || c == '=' || c == ':') || c == '"')
                {
                    if (c == '"' && !startOfQuote)
                    {
                        startOfQuote = true;
                    }
                    else if (startOfQuote)
                    {
                        startOfQuote = false;
                    }

                    if (step == ParseStep.ReadBlockRetVal && txt != "" && txt != ":") //Header ohne RetVal
                    {
                        blockRetValType = "";
                        akBlock = CreateBlock(blockType, blockNumberOrName, blockRetValType);

                        step = ParseStep.ParseHeaderRow;
                    }

                    if (step == ParseStep.ParseTitle && (c == '\n' || c == '\r'))
                        step = ParseStep.ParseHeaderRow;

                    if (txt != "")
                    {
                        switch (step)
                        {
                            case ParseStep.ReadBlockType:
                                {
                                    blockType = txt.ToUpper();
                                    step = ParseStep.ReadBlockNumberOrName;
                                    break;
                                }
                            case ParseStep.ReadBlockNumberOrName:
                                {
                                    blockNumberOrName = txt;
                                    step = ParseStep.ReadBlockRetVal;
                                    break;
                                }
                            case ParseStep.ReadBlockRetVal:
                                {
                                    step = ParseStep.ReadBlockRetValType;
                                    break;
                                }
                            case ParseStep.ReadBlockRetValType:
                                {
                                    blockRetValType = txt;
                                    akBlock = CreateBlock(blockType, blockNumberOrName, blockRetValType);
                                    step = ParseStep.ParseHeaderRow;
                                    break;
                                }
                            case ParseStep.ParseHeaderRow:
                                {
                                    switch (txt.ToUpper())
                                    {
                                        case ("TITLE"):
                                            {
                                                step = ParseStep.ParseTitle;
                                                break;
                                            }
                                        case ("VERSION"):
                                            {
                                                step = ParseStep.ParseVersion;
                                                break;
                                            }
                                        case ("STRUCT"):
                                            {
                                                step = ParseStep.ParseStructure;
                                                break;
                                            }
                                        case ("BEGIN"):
                                            {
                                                step = ParseStep.ParseAWLCode;
                                                break;
                                            }
                                        case ("VAR_INPUT"):
                                        case ("VAR_OUTPUT"):
                                        case ("VAR_IN_OUT"):
                                        case ("VAR_TEMP"):
                                            {
                                                step = ParseStep.ParsePara;

                                                break;
                                            }
                                        default:
                                            {
                                                if (txt.StartsWith("{") && txt.EndsWith("}"))
                                                {
                                                    akBlock.Attributes = ParseStep7Attribute(txt);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case ParseStep.ParseTitle:
                                {
                                    akBlock.Title = txt;
                                    step = ParseStep.ParseHeaderRow;
                                    break;
                                }

                            case ParseStep.ParseVersion:
                                {
                                    akBlock.Version = txt;
                                    step = ParseStep.ParseHeaderRow;
                                    break;
                                }
                        }
                    }
                    txt = "";
                }
                else
                {
                    txt = txt + c;
                }
            }

            return retVal;
        }

        private static List<Step7Attribute> ParseStep7Attribute(string txt)
        {
            var retVal = new List<Step7Attribute>();

            txt = txt.Replace("{", "").Replace("}", "");

            return retVal;
        }

        private static S7Block CreateBlock(string blockType, string blockNumber, string type)
        {
            S7Block akBlock = null;

            switch (blockType.ToUpper())
            {
                case ("TYPE"):
                    {
                        akBlock = new S7DataBlock() { BlockType = DataTypes.PLCBlockType.UDT, BlockLanguage = PLCLanguage.DB };
                    }
                    break;

                case ("DATA_BLOCK"):
                    {
                        akBlock = new S7DataBlock() { BlockType = DataTypes.PLCBlockType.DB, BlockLanguage = PLCLanguage.DB };
                    }
                    break;

                case ("FUNCTION"):
                    {
                        akBlock = new S7FunctionBlock() { BlockType = DataTypes.PLCBlockType.FC, BlockLanguage = PLCLanguage.AWL };

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
                    break;

                case ("FUNCTION_BLOCK"):
                    {
                        akBlock = new S7FunctionBlock() { BlockType = DataTypes.PLCBlockType.FB, BlockLanguage = PLCLanguage.AWL };

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
                    break;
            }

            return akBlock;
        }
    }
}