using System;
using System.IO;

namespace DotNetSiemensPLCToolBoxLibrary.General
{
    public static class BinaryReaderExtensions
    {
        public static Guid ReadGuid(this BinaryReader binaryReader)
        {
            return new Guid(binaryReader.ReadBytes(16));
        }

        public static Version ReadVersion(this BinaryReader binaryReader)
        {
            return new Version(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
        }
    }
}
