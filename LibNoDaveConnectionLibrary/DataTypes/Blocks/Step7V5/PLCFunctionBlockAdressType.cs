using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    public enum PLCFunctionBlockAdressType
    {
        //Type:
        //0 = Normale Adresse Bsp: T0, E0.1, AW4
        //1 = Indirekte Adresse Bsp: T [DBW4], E [LD3]
        //2 = AR adressierung: T[ar1, p#0.0], DBX [ar1, p#0.0]
        //3 = Nur AR adressierung: [ar1, p#0.0]
        Direct = 0,
        Indirect = 1,
        IndirectWithAR = 2,
        CompleteIndirectWithAR = 3,
        FCParameter = 4
    }
}
