namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class SymbolTableEntry : Step7ProjectFolder
    {
        public string Symbol { get; set; }
        public string Comment { get; set; }
        public string Operand { get; set; }
        public string OperandIEC { get; set; }
        public string DataType { get; set; }

        public override string ToString()
        {
            return Symbol + " (" + Operand + ")";
        }
    }
}
