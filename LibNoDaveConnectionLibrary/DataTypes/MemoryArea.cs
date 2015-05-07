namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    public enum MemoryArea
    {
        None = 0x00, //This types is not possible within Tags
        Periphery = 0x80, //This types is not possible within Tags

        Inputs = 0x81,

        Outputs = 0x82,

        Flags = 0x83,

        Datablock = 0x84,

        InstanceDatablock = 0x85,

        

        //Todo: Block the following two types in the Tags
        LocalData = 0x86, //This types is not possible within Tags
        PreviousLocalData = 0x87, //This types is not possible within Tags
        SystemDataBlock = 0x89, //This types is maybe not possible within Tags
        BlockFB = 0x17, //This types is maybe not possible within Tags
        BlockFC = 0x18, //This types is maybe not possible within Tags
        BlockDB = 0x19, //This types is maybe not possible within Tags
        BlockSDB = 0x1A, //This types is maybe not possible within Tags
        Timer = 0x1D,

        Counter = 0x1C,

        AnalogIn200 = 0x6,

        AnalogOut200 = 0x7,

        Counter200 = 30,

        Timer200 = 31,

        S5_DX = 300,
    }
}
