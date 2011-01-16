using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public abstract class Network
    {
        public Network()
        {
            AWLCode = new List<FunctionBlockRow>();
        }

        public List<FunctionBlockRow> AWLCode { get; set; }
    }
}
