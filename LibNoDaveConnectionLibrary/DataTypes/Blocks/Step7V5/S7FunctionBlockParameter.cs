namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    public class S7FunctionBlockParameter
    {
        public S7FunctionBlockParameter(S7FunctionBlockRow Parent)
        {
            this.Parent = Parent;
        }

        public S7FunctionBlockRow Parent { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public S7DataRowType ParameterDataType { get; set; }
        public S7FunctionBlockParameterDirection ParameterType { get; set; }

        public string GetValue(bool Symbolic)
        {
            if (!Symbolic)
                return Value;

            if (Parent != null)
            {
                if (Parent.Parent != null)
                {
                    var sym = Parent.Parent.SymbolTable;
                    if (sym != null && Name != "SPA")
                    {
                        var ent = sym.GetEntryFromOperand(Value);
                        if (ent != null)
                            return "\"" + ent.Symbol + "\"";
                    }
                }
            }
            return Value;
        }
    }
}