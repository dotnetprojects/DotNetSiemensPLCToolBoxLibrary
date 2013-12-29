using System;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Enums
{
    [Flags]
    public enum TiaFileType : short
    {
        Composite = 0x40,
        CompositeIndex = 0x80,
        Index = 2,
        Linked = 8,
        None = 0,
        Session = 4,
        Storage = 1,
        Trace = 0x20,
        Unsupported = 0x10
    }

}
