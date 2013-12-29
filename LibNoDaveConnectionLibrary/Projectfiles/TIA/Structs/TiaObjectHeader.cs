using System.Runtime.InteropServices;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Enums;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Structs
{    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
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

        public override string ToString()
        {
            return TypeId.ToString() + "-" + InstanceId.ToString();
        }

        public TiaObjectId GetTiaObjectId()
        {
            return new TiaObjectId(TypeId, InstanceId);
        }
    }    
}
