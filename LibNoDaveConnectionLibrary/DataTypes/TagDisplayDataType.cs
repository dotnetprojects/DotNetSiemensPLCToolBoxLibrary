using System;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    public enum TagDisplayDataType
    {
        //For Bool and Timer and Counter
        Bool,
        //For Integer Formats
        Decimal,
        Hexadecimal,
        Binary,
        Float,
        Pointer,
        //For Time Formats
        TimeSpan,
        S5Time,
        Time,
        //For dateTime
        DateTime,
        S7DateTime,
        S7TimeOfDay,
        S7Date,
        //
        ByteArray,
        //For CharArray, String
        String
    }
}
    
