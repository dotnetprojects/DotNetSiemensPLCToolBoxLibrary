using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Enums
{
    [Flags]
    public enum TiaObjectStates : short
    {
        Created = 1,
        Deleted = 4,
        LinksModified = 0x10,
        Modified = 2,
        None = 0,
        Reserved_0x20 = 0x20,
        Destroyed = 8,
        Reserved_0x80 = 0x80,
        Transient = 0x40
    }
}
