using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Enums;
using System;
using System.IO;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Structs
{    
    public struct TiaObjectHeader
    {
        public int Size;
        public int TypeId;
        public long InstanceId;
        public int ClusterId;
        public short ObjectInfo;
        public short res;
        public TiaObjectStates StateFlags;
        public sbyte OffsetCount;
        public sbyte MetaInfo;
        public Guid Guid;
        public byte[] Data;

        public string DataAsUtf8 => Encoding.UTF8.GetString(Data);

        public override string ToString()
        {
            return TypeId.ToString("x") + "-" + InstanceId.ToString("x") + "  (" + TypeId.ToString() + "-" + InstanceId.ToString() + ")";
        }

        public TiaObjectId GetTiaObjectId()
        {
            return new TiaObjectId(TypeId, InstanceId);
        }

        public static TiaObjectHeader Deserialize(BinaryReader reader)
        {
            var size = reader.ReadInt32();
            return new TiaObjectHeader()
            {
                Size = size, // 4
                TypeId = reader.ReadInt32(), // 4
                InstanceId = reader.ReadInt64(), // 8
                ClusterId = reader.ReadInt32(), // 4
                ObjectInfo = reader.ReadInt16(), // 2
                res = reader.ReadInt16(), // 2
                StateFlags = (TiaObjectStates)reader.ReadInt16(), // 2
                OffsetCount = reader.ReadSByte(), // 1
                MetaInfo = reader.ReadSByte(), // 1
                Guid = reader.ReadGuid(), // 16
                Data = reader.ReadBytes(size - 12), // 4+4+8+4+2+2+2+1+1+16
                //siemens scheint nicht 44 abzuziehen, sondern nur 12, aber vlt gehören die daten auch nicht dazu
            };
        }
    }
}
