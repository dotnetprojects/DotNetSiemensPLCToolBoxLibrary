namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    public enum TagDataSource
    {
        Inputs = 0x81,
        Outputs = 0x82,
        Flags = 0x83,
        Datablock = 0x84,
        InstanceDatablock = 0x85,
        //Todo: Block the following two types in the Tags
        LocalData = 0x86, //This types is not possible within Tags
        PreviousLocalData = 0x87, //This types is not possible within Tags
        SystemDataBlock = 0x89, //This types is maybe not possible within Tags
        Timer = 29,
        Counter = 28,
        AnalogIn200 = 0x6,
        AnalogOut200 = 0x7,
        Counter200 = 30,
        Timer200 = 31
    }
}
