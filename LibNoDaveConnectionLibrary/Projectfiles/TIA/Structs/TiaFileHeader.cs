using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Enums;
using System;
using System.IO;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Structs
{
    public struct TiaFileHeader
    {
        public TiaFileType FileType;
        public TiaFileProtocol FileProtocol;
        [Obsolete]
        public int StoreId;
        public Guid FileId;
        public Version Version;
        public Version FileFormat;
        public byte Node;
        public byte[] Hash;
        public byte EndByte;

        public static TiaFileHeader Deserialize(BinaryReader reader)
        {

            return new TiaFileHeader()
            {
                FileType = (TiaFileType)reader.ReadInt16(),
                FileProtocol = (TiaFileProtocol)reader.ReadInt16(),
                StoreId = reader.ReadInt32(),
                FileId = reader.ReadGuid(),
                Version = reader.ReadVersion(),
                FileFormat = reader.ReadVersion(),
                Node = reader.ReadByte(),
                Hash = reader.ReadBytes(40),
                EndByte = reader.ReadByte()
            };
        }
    }
}
