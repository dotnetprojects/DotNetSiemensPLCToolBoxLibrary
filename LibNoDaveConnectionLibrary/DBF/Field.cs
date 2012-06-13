using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DBF {
    public abstract class Field {
        public string get() {
            return string.Empty;
        }
        public dBaseType getType() {
            return dBaseType.C;
        }
    }
}
