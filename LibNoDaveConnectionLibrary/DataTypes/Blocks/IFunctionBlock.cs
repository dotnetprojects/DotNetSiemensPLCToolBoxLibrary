using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    internal interface IFunctionBlock
    {
        List<FunctionBlockRow> AWLCode { get; set; }

        List<Network> Networks { get; set; }
    }
}