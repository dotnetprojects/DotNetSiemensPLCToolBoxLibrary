using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Enums
{
    [Flags]
    public enum TiaObjectStatesLegacy : short
    {
        Created = 1,
        Deleted = 4,
        Modified = 2,
        None = 0,
        Reserved = 8,                
        LinksModified = 0x10,          
        Reserved_0x20 = 0x20,                   
        Reserved_0x80 = 0x80,     
        TRefITransientdRef = 0x40                
    }
}
