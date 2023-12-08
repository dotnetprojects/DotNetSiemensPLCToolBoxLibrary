using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Enums;
using System;
using System.Runtime.InteropServices;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TiaFileHeader
    {
        public TiaFileType FileType;
        public TiaFileProtocol FileProtocol;
        public int Id;
        public Guid Key;
        public int VersionMajor;
        public int VersionMinor;
        public int VersionBuild;
        public int VersionRevision;
        public int Res1;
        public byte Node;
        public byte Res2;
    }
}