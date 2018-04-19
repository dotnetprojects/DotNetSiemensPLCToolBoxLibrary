
namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    public enum NCK_Area : byte
    {
        /// <summary>
        /// N: NC Daten
        /// </summary>
        AreaNCK = 0,
        /// <summary>
        /// B: Daten Betriebsartengruppe
        /// </summary>
        AreaBag = 1,
        /// <summary>
        /// C: Kanalzugeordnete Daten
        /// </summary>
        AreaChannel = 2,
        /// <summary>
        /// A: Achsspezifische Grundeinstellungen
        /// </summary>
        AreaAxis = 3,
        /// <summary>
        /// T: Wergzeugdaten
        /// </summary>
        AreaTool = 4,
        /// <summary>
        /// V: Vorschubantrieb
        /// </summary>
        AreaFeedDrive = 5,
        /// <summary>
        /// H: Hauptantrieb
        /// </summary>
        AreaMainDrive = 6,
        /// <summary>
        /// M: MMC-Daten
        /// </summary>
        AreaMMC = 7,
        /// <summary>
        /// ?: Unbekannt
        /// </summary>
        AreaUnknown = 255,
    }
}
