namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    /// <summary>
    /// The programming language in which an S7Block was created
    /// </summary>
    public enum PLCLanguage
    {
        unkown = 0,

        /// <summary>
        /// "Anweisungsliste" or AWL for short. The English term is
        /// "Statement list" or STL
        /// </summary>
        AWL = 1,

        /// <summary>
        /// "Kontaktplan" or KOP for short. The English term is
        /// "Ladder" or Lad
        /// </summary>
        KOP = 2,

        /// <summary>
        /// "Funktionsplan" or FUP for short. The English term is
        /// "Functional Block diagram" or FDB
        /// </summary>
        FUP = 3,

        /// <summary>
        /// "Structured Text" or SCL for short
        /// </summary>
        SCL = 4,

        /// <summary>
        /// The Datablock declaration language. Only used for Datablocks
        /// </summary>
        DB = 5,

        /// <summary>
        /// Graphset language
        /// </summary>
        GRAPH = 6,

        /// <summary>
        /// Continous Function Chart
        /// </summary>
        CFC = 7,

        /// <summary>
        /// Sequencial Funcion Chart
        /// </summary>
        SFC = 8,

        /// <summary>
        /// Same as AWL but for Safty blocks
        /// </summary>
        FAWL = 31,

        /// <summary>
        /// Same as KOP but for Safty blocks
        /// </summary>
        FKOP = 32,

        /// <summary>
        /// Same as FUP but for Safty blocks
        /// </summary>
        FFUP = 33,

        /// <summary>
        /// Same as SCL but for Safty blocks
        /// </summary>
        FSCL = 34,

        /// <summary>
        /// Same as DB but for Safty blocks
        /// </summary>
        FDB = 34,

        /// <summary>
        /// F-Call blocks are special code blocks that are neceseary to Execute Safty Programms
        /// Only applicable on Safety CPU's
        /// </summary>
        FCALL = 35,
    }
}