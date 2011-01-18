using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    interface IFunctionBlock
    {
        List<FunctionBlockRow> AWLCode { get; set; }

        List<Network> Networks { get; set; }
    }
}
