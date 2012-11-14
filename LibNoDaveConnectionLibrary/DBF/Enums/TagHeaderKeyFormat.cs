namespace DotNetSiemensPLCToolBoxLibrary.DBF.Enums
{
    /// <summary>
    /// Format of a tag header key
    /// </summary>
    public enum TagHeaderKeyFormat : byte
    {
        /// <summary>
        /// Right, Left, DTOC
        /// </summary>
        RLDT = 0x00,
        /// <summary>
        /// Descending
        /// </summary>
        Descending = 0x08,
        /// <summary>
        /// String
        /// </summary>
        String = 0x10,
        /// <summary>
        /// Distinct
        /// </summary>
        Distinct = 0x20,
        /// <summary>
        /// Unique
        /// </summary>
        Unique = 0x40
    }
}
