using System;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
/// <summary>
/// Represents an Information Block of an Program Block stored in the PLC memory
/// </summary>
/// <remarks>the parsing of data is mainly derived from the S7Comm Plug-in for Whireshark</remarks>
    public class S7BlockInfo
    {
        /// <summary>
        /// The Block type of the stored blocked
        /// </summary>
        /// <returns></returns>
        public PLCBlockType BlockType { get; set; }

        /// <summary>
        /// The Language in which the block was compiled and downloaded to the PLC
        /// </summary>
        /// <returns></returns>
        public DotNetSiemensPLCToolBoxLibrary.DataTypes.PLCLanguage CreationLanguage { get; set; }

        /// <summary>
        /// The Number of the Block
        /// </summary>
        /// <returns></returns>
        public int Number { get; set; }

        /// <summary>
        /// The Block Name of the Block stored as stored in the PLC. Maximum 8 characters
        /// </summary>
        /// <returns></returns>
        public string BlockName { get; set; }

        /// <summary>
        /// The Block Family (block Category) as stored as stored in the PLC. Maximum 8 characters
        /// </summary>
        /// <returns></returns>
        public string Family { get; set; }

        /// <summary>
        /// The Block Family (block Category) as stored as stored in the PLC. Maximum 8 characters
        /// </summary>
        /// <returns></returns>
        public string Autor { get; set; }

        /// <summary>
        /// The last code modification of the Block
        /// </summary>
        /// <returns></returns>
        public DateTime LastModifiedCode { get; set; }

        /// <summary>
        /// The last interface modification of the blocks interface
        /// Block interfaces are changed if you change In, Out, InOut parameters, Static parameters, or the structure of Data-blocks
        /// </summary>
        /// <returns></returns>
        public DateTime LastModifiedInterface { get; set; }

        /// <summary>
        /// The Block is currently linked in the CPU. Only linked Blocks are executed by the plc
        /// </summary>
        /// <returns></returns>
        public bool Linked { get; set; }

        /// <summary>
        /// Its not completely clear what this indicates. it probably means, that it is an "Normal" Data block
        /// or an "instance" Data block.
        /// </summary>
        /// <returns></returns>
        public bool StandardBlock { get; set; }

        /// <summary>
        /// If an block is not Retentive (true), the it gets reset to its initial values after each Power-cycle of the PLC.
        /// </summary>
        /// <returns></returns>
        public bool NonRetain { get; set; }

        /// <summary>
        /// The block is know-how protected. 
        /// </summary>
        /// <returns></returns>
        public bool KwnoHowProtected { get; set; }

        /// <summary>
        /// The current Data block version. The version can only contain Major and Minor versions, between 0.1 and 25.5
        /// </summary>
        /// <returns></returns>
        public Version BlockVersion { get; set; }

        /// <summary>
        /// The CRC checksum of the block. It is calculated via CRC, but the CRC parameters are not entirely clear.
        /// </summary>
        /// <returns></returns>
        public int Checksum { get; set; }

        /// <summary>
        /// The total compiled MC7 Code length in the PLC's RAM
        /// </summary>
        /// <returns></returns>
        public UInt32 CodeLength { get; set; }

        /// <summary>
        /// The total Code length occupied in the Lead memory of the PLC.
        /// </summary>
        /// <remarks> It seams that the real length is always 6 bytes more than reported by the PLC</remarks>
        /// <returns></returns>
        public UInt32 LoadMemorylength { get; set; }

        /// <summary>
        /// Not sure what this length is. I derived it from the S7Comm plug-in from Whireshark
        /// </summary>
        /// <returns></returns>
        public UInt32 SSBLengh { get; set; }

        /// <summary>
        /// Not sure what this length is. I derived it from the S7Comm plug-in from Whireshark
        /// </summary>
        /// <returns></returns>
        public UInt32 ADDLength { get; set; }

        /// <summary>
        /// The required local data of the block on the local data stack
        /// </summary>
        /// <returns></returns>
        public UInt32 LocalDataLength { get; set; }

        /// <summary>
        /// Creates an new empty Block info
        /// </summary>
        public S7BlockInfo()
        {
        }

        /// <summary>
        /// Create and parse an new block info instance from its corresponding binary data from the plc
        /// </summary>
        /// <remarks></remarks>
        /// <param name="RawData">The raw binary data from the PLC. Must be 78 Bytes from the PLC</param>
        public S7BlockInfo(byte[] RawData)
        {
            ParseBlockInfo(RawData, 0);
        }

        /// <summary>
        /// Create and parse an new block info instance from its corresponding binary data from the plc
        /// </summary>
        /// <param name="offset">Offset where to start parsing the data</param>
        /// <param name="RawData">The raw binary data from the PLC. Must be 78 Bytes from the PLC</param>
        public S7BlockInfo(byte[] RawData, int offset)
        {
            ParseBlockInfo(RawData, offset);
        }

        /// <summary>
        /// Create and parse an new block info instance from its corresponding binary data from the plc
        /// </summary>
        /// <param name="offset">Offset where to start parsing the data</param>
        /// <param name="RawData">The raw binary data from the PLC. Must be 78 Bytes from the PLC</param>
        public static S7BlockInfo Parse(byte[] RawData, int offset)
        {
            return new S7BlockInfo(RawData, offset);
        }

        /// <summary>
        /// Create and parse an new block info instance from its corresponding binary data from the plc
        /// </summary>
        /// <param name="RawData">The raw binary data from the PLC. Must be 78 Bytes from the PLC</param>
        public static S7BlockInfo Parse(byte[] RawData)
        {
            return new S7BlockInfo(RawData, 0);
        }

        /// <summary>
        /// Parse the block info from the PLC
        /// </summary>
        /// <param name="RawData"></param>
        /// <param name="offset"></param>
        private void ParseBlockInfo(byte[] RawData, int offset)
        {
            //Byte 0 and 1 are unknown
            //Byte 2 and 3 are always =  00 4A
            //Byte 4 and 5 is an unknown Variable
            //Byte 6 and 7 are always = Character: pp
            //Byte 8 unknown

            //Byte 9 Flags
            Linked = (RawData[offset + 9] & 1) > 0;
            StandardBlock = (RawData[offset + 9] & 2) > 0;
            NonRetain = (RawData[offset + 9] & 8) > 0;

            //Byte 10 = Creation Language
            CreationLanguage = (PLCLanguage)RawData[offset + 10];

            //Byte 11 = Block Type
            BlockType = (PLCBlockType)RawData[offset + 11];

            //Byte 12 and 13 = Number (byte Swaped)
            byte[] BlkNr = {
                RawData[offset + 13],
                RawData[offset + 12]
            };
            Number = BitConverter.ToUInt16(BlkNr, 0);

            //Byte 14 and 17 are Code Length
            LoadMemorylength = libnodave.getU32from(RawData, offset + 14);

            //Byte 18 to 21 are Block-security
            //This is strange, since it is an 4 byte integer. The values seem to be
            //0 = No protection
            //1 = Know How protected
            //>1 = unknown. This is probably used by the advanced Protection introduced in later Simatic Manager versions (5.5. i think)
            var Tmp = libnodave.getU32from(RawData, offset + 18);
            KwnoHowProtected = Tmp != 0;

            //Byte 22 to 27 are Last Modified Code
            LastModifiedCode = ParseTimeStamp(RawData, offset + 22);

            //Byte 28 to 33 are Last Modified Interface
            LastModifiedInterface = ParseTimeStamp(RawData, offset + 28);

            //Byte 34 to 35 SSB length
            SSBLengh = BitConverter.ToUInt16(RawData, offset + 34);

            //Byte 36 to 37 Add length
            ADDLength = BitConverter.ToUInt16(RawData, offset + 34);

            //Byte 38 to 39 Local data length
            LocalDataLength = BitConverter.ToUInt16(RawData, offset + 34);

            //Byte 40 and 41 = Length
            CodeLength = BitConverter.ToUInt16(RawData, offset + 40);

            //Byte 42 to 49 = Author
            Autor = "";
            for (int i = 42; i <= 49; i++)
            {
                Autor = Autor + (char)RawData[offset + i];
            }
            Autor = Autor.Replace("\0", string.Empty); //Remove Null chars

            //Byte 50 to 57 = Family
            Family = "";
            for (int i = 50; i <= 57; i++)
            {
                Family = Family + (char)RawData[offset + i];
            }
            Family = Family.Replace("\0", string.Empty); //Remove Null chars

            //Byte 58 to 65 = Name
            BlockName = "";
            for (int i = 58; i <= 65; i++)
            {
                BlockName = BlockName + (char)RawData[offset + i];
            }
            BlockName = BlockName.Replace("\0", string.Empty); //Remove Null chars

            //Byte 66 = Version
            //the Minor/Major version is encoded as an byte number where:
            //Major = plc version / 10 
            //minor = plc version % 10
            //this gives an version from 0.0 to 25.5 where each minor version can be at maximum .9
            BlockVersion = new Version(RawData[offset + 66] / 10, RawData[offset + 66] % 10);

            //Byte 67 = unknown, seems to be reserved

            //Byte 68 t0 69 Checksum
            Checksum = BitConverter.ToUInt16(RawData, offset + 68);

            //Byte 60 to 75 are unknown, and seem to be reserved
        }

        /// <summary>
        /// Parse an Timestamp from the block header
        /// </summary>
        /// <param name="RawData"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private DateTime ParseTimeStamp(byte[] RawData, int offset)
        {
            DateTime functionReturnValue = default(DateTime);
            //Copy Raw data to Local Storage
            byte[] _RawData = new byte[6];
            Array.ConstrainedCopy(RawData, offset, _RawData, 0, 6);

            //Decode Time
            //All Encoded Values are Added to this Base Time
            //The Base Time is: 01.01.1984

            //Byte 0: value * hex:1000000 (16777216) ms = ca 1/269,6 min
            //Byte 1: value * hex:10000 (65536) ms = ca 1/65,5 sec
            //Byte 2: value * hex:100 (256) ms = ca 1/256 ms
            //Byte 3: value ms (already milliseconds)
            //Byte 4: value * hex:100 (256) days = ca 1/256 days
            //Byte 5: value day (already days) 

            functionReturnValue = new DateTime(1984, 1, 1, 0, 0, 0, 0);
            functionReturnValue = functionReturnValue.AddMilliseconds(_RawData[0] * 0x1000000);
            functionReturnValue = functionReturnValue.AddMilliseconds(_RawData[1] * 0x10000);
            functionReturnValue = functionReturnValue.AddMilliseconds(_RawData[2] * 0x100);
            functionReturnValue = functionReturnValue.AddMilliseconds(_RawData[3]);
            functionReturnValue = functionReturnValue.AddDays(_RawData[4] * 0x100);
            functionReturnValue = functionReturnValue.AddDays(_RawData[5]);
            return functionReturnValue;
        }

    }
}
