using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IBlock
    {
        [JsonProperty(Order = -2)]
        int BlockNumber { get; }
        string SymbolOrName { get; }
    }
}
