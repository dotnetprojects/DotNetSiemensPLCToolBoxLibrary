namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public enum SelectPartType
    {
        BlocksOfflineFolder = 1,
        CPUFolder = 2,
        SourceFolder = 3,
        SymbolTable = 4,
        S7ProgrammFolder = 5,
        RootProgrammFolder = 6,
        BlocksOfflinePart = 1001, //Select a fb,fc,db,vat,...
        VariableTable = 1011,
        DataType = 1012,
        FunctionBlock = 1021,
        DataBlock = 1022,
        DataBlocks = 1023,
        VariableTableOrSymbolTable = 1030,
        
        Tag = 2001,
    }
}
