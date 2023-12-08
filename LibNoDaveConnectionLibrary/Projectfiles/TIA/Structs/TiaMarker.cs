using System;
using System.Runtime.InteropServices;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TiaMarker
    {
        public byte StartByte;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string MarkerText;

        public long TimeStampTicks;
        public byte EndByte;

        public DateTime GetDateTime()
        {
            return new DateTime(TimeStampTicks);
        }
    }
}