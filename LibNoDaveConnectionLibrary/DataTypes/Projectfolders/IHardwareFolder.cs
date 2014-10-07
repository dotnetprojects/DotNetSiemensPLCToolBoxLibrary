using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders
{
    public interface IHardwareFolder
    {
        int Rack { get; set; }
        int Slot { get; set; }
    }
}
