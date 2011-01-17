using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    public class S7FunctionBlockParameter
    {
        public string Comment { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public S7DataRowType ParameterDataType { get; set; }
        public S7FunctionBlockParameterDirection ParameterType { get; set; }        
    }
}
