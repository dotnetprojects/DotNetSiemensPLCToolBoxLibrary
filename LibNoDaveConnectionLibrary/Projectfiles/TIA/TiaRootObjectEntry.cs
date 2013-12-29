using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA
{
    public class TiaRootObjectEntry
    {
        public TiaObjectId ObjectId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return ObjectId.ToString() + " (" + Name + ")";
        }
    }
}
