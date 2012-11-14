namespace DotNetSiemensPLCToolBoxLibrary.DBF.Enums
{
    /// <summary>
    /// Data type of an field / key
    /// </summary>
    public enum dBaseType : byte
    {
        /// <summary>
        /// Binary
        /// </summary>
        B = 0x42,
        /// <summary>
        /// String
        /// </summary>
        C = 0x43,
        /// <summary>
        /// Date (YYYYMMDD)
        /// </summary>
        D = 0x44,
        /// <summary>
        /// Number (Double)
        /// </summary>
        N = 0x4E,
        /// <summary>
        /// Float
        /// </summary>
        F = 0x46,
        /// <summary>
        /// Logical (Boolean) Byte
        /// </summary>
        L = 0x4C,
        /// <summary>
        /// Memo
        /// </summary>
        M = 0x4D,
        /// <summary>
        /// DateTime
        /// </summary>
        T = 0x54
    }
}
