using DotNetSiemensPLCToolBoxLibrary.DBF.Enums;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Index
{
    public abstract class Field
    {
        public string get()
        {
            return string.Empty;
        }

        public dBaseType getType()
        {
            return dBaseType.C;
        }
    }
}
