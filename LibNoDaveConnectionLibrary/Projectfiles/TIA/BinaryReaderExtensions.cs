using System.IO;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA
{
    public static class BinaryReaderExtensions
    {
        public static TiaObjectId ReadTiaObjectId(this BinaryReader rd)
        {
            return new TiaObjectId(rd.ReadInt32(), rd.ReadInt64());
        }
    }
}