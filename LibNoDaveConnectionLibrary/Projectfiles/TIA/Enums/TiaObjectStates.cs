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
        Modified = 2,
        None = 0,
        Destroyed = 8,                   //Reserved on Legacy
        Transient = 0x10,                //LinksModified on Legacy
        TRef = 0x20,                     //Reserved_0x20 on Legacy
        TRefCreatedViaDelete = 0x80,     //Reserved_0x80 on Legacy
        TRefIdRef = 0x40                 //Transient on Legacy
    }
}
