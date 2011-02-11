namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    public enum PLCBlockType
    {
        AllBlocks = 0, //Change to 0xffff
        AllEditableBlocks = 1, //Change to 0xfffe

        SourceBlock = 2, //This is a Source Block

        //Step 7 Types...
        OB = 0x08,
        DB = 0x0a,
        SDB = 0x0b,
        FC = 0x0c,
        SFC = 0x0d,
        FB = 0x0e,
        SFB = 0x0f,
        UDT = 0xff, //Normal Type of this Block Type is 0x00
        VAT = 0x1B, //Variable Table, only offline pssible!

        //Step 5 Types...
        S5_PB = 0xf04,  //4
        S5_FB = 0xf08,  //8
        S5_FX = 0xf05,  //5
        S5_DB = 0xf01,  //1
        S5_DX = 0xf0c,  //12
        S5_SB = 0xf02,  //2
        S5_OB = 0xf10,  //16
        S5_OK = 0xf51,  //81
        S5_PK = 0xf31,  //49
        S5_FK = 0xf41,  //65
        S5_FKX = 0xf5a, //90
        S5_SK = 0xf21,  //33
        S5_DK = 0xf5b,  //91
        S5_DKX = 0xf5c,  //91
        S5_BB = 0xf64,  //100

        
        S5_FV = 0xff1,   //Vorkopf zum FB
        S5_FVX = 0xff2,  //Vorkopf zum erw. FB
        S5_DV = 0xff3,   //Vorkopf Datenbaustein
        S5_DVX = 0xff4,  //Vorkopf erw. datenbaustein
        

        //TIA Portal 7 Types...
        S7V11_OB = 0xe08,
        S7V11_DB = 0xe0a,
        S7V11_SDB = 0xe0b,
        S7V11_FC = 0xe0c,
        S7V11_SFC = 0xe0d,
        S7V11_FB = 0xe0e,
        S7V11_SFB = 0xe0f,
        S7V11_UDT = 0xeff, //Normal Type of this Block Type is 0x00
        S7V11_VAT = 0xe1B, //Variable Table, only offline pssible!
    }
}
