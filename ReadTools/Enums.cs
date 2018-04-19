using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolReader
{
    public enum NCK_Block
    {
        // Summary:
        //     Systemdaten
        BlockY = 16,
        //
        // Summary:
        //     NCK-Anweisungsgruppen
        BlockYNCFL = 17,
        //
        // Summary:
        //     Einstellbare Nullpunktverschiebung
        BlockFU = 18,
        //
        // Summary:
        //     Aktive Nullpunktverschiebung
        BlockFA = 19,
        //
        // Summary:
        //     Schneidendaten, Korrekturdaten
        BlockTO = 20,
        //
        // Summary:
        //     Rechenparameter
        BlockRP = 21,
        //
        // Summary:
        //     Settingdaten
        BlockSE = 22,
        //
        // Summary:
        //     SGUD-Block
        BlockSGUD = 23,
        //
        // Summary:
        //     Lokale Benutzerdaten
        BlockLUD = 24,
        //
        // Summary:
        //     Werkzeugträger-Parameter
        BlockTC = 25,
        //
        // Summary:
        //     Maschinendaten
        BlockM = 26,
        BlockWAL = 28,
        //
        // Summary:
        //     Diagnosedaten, die nur für entwicklungsinterne Zwecke
        BlockDIAG = 30,
        BlockCC = 31,
        //
        // Summary:
        //     Externe Nullpunktverschiebung
        BlockFE = 32,
        //
        // Summary:
        //     Werkzeugdaten, allgemeine Daten
        BlockTD = 33,
        //
        // Summary:
        //     Schneidendaten, Überwachungsdaten
        BlockTS = 34,
        //
        // Summary:
        //     Werkzeugdaten, schleifspezifische Daten
        BlockTG = 35,
        //
        // Summary:
        //     Werkzeugdaten, anwenderdefinierte Daten
        BlockTU = 36,
        //
        // Summary:
        //     Schneidendaten, anwenderdefinierte Daten
        BlockTUE = 37,
        //
        // Summary:
        //     Werkzeugdaten, Verzeichnis
        BlockTV = 38,
        //
        // Summary:
        //     Magazindaten, allgemeine Daten
        BlockTM = 39,
        //
        // Summary:
        //     Magazindaten, Platzdaten
        BlockTP = 40,
        //
        // Summary:
        //     Magazindaten, Mehrfachzuordnung von Platzdaten
        BlockTPM = 41,
        //
        // Summary:
        //     Magazindaten, Platztypen
        BlockTT = 42,
        //
        // Summary:
        //     Magazindaten, Verzeichnis
        BlockTMV = 43,
        //
        // Summary:
        //     Magazindaten, Konfigurationsdaten
        BlockTMC = 44,
        //
        // Summary:
        //     MGUD-Block
        BlockMGUD = 45,
        //
        // Summary:
        //     UGUD-Block
        BlockUGUD = 46,
        //
        // Summary:
        //     GUD4-Block
        BlockGUD4 = 47,
        //
        // Summary:
        //     GUD5-Block
        BlockGUD5 = 48,
        //
        // Summary:
        //     GUD6-Block
        BlockGUD6 = 49,
        //
        // Summary:
        //     GUD7-Block
        BlockGUD7 = 50,
        //
        // Summary:
        //     GUD8-Block
        BlockGUD8 = 51,
        //
        // Summary:
        //     GUD9-Block
        BlockGUD9 = 52,
        //
        // Summary:
        //     Schutzbereiche
        BlockPA = 53,
        BlockGD1 = 54,
        //
        // Summary:
        //     Nibbling
        BlockNIB = 55,
        //
        // Summary:
        //     Event-Typen
        BlockETP = 56,
        //
        // Summary:
        //     Datenlisten für die Protokollierung
        BlockETPD = 57,
        //
        // Summary:
        //     Kanalspezifische Synchronaktionen
        BlockSYNACT = 58,
        //
        // Summary:
        //     Diagnosebaustein
        BlockDIAGN = 59,
        //
        // Summary:
        //     NCK-spezifische Anwendervariablen für Synchronaktion
        BlockVSYN = 60,
        //
        // Summary:
        //     Überwachungsanwenderdaten
        BlockTUS = 61,
        //
        // Summary:
        //     Magazin-Anwenderdaten
        BlockTUM = 62,
        //
        // Summary:
        //     Magazinplatz-Anwenderdaten
        BlockTUP = 63,
        BlockTF = 64,
        //
        // Summary:
        //     Basisframe: einstellbarer Frame, der immer wirkt
        BlockFB = 65,
        //
        // Summary:
        //     Spindelzustandsdaten bei Spindelumsetzung
        BlockSSP2 = 66,
        //
        // Summary:
        //     Programmglobale Benutzerdaten
        BlockPUD = 67,
        //
        // Summary:
        //     Schneidenbezogene ortsabhängige Summenkorrekturen
        BlockTOS = 68,
        //
        // Summary:
        //     Schneidenbezogene ortsabhängige Summenkorrekturen
        BlockTOST = 69,
        //
        // Summary:
        //     Schneidenbezogene ortsabhängige Summenkorrekturen
        BlockTOE = 70,
        //
        // Summary:
        //     Schneidenbezogene ortsabhängige Summenkorrekturen
        BlockTOET = 71,
        //
        // Summary:
        //     Adapterdaten
        BlockAD = 72,
        //
        // Summary:
        //     Schneidendaten, transformierte Korrekturdaten
        BlockTOT = 73,
        //
        // Summary:
        //     Arbeitskorrekturen: Verzeichnis
        BlockAEV = 74,
        //
        // Summary:
        //     NCK-Anweisungsgruppen Fanuc
        BlockYFAFL = 75,
        //
        // Summary:
        //     System-Frame
        BlockFS = 76,
        //
        // Summary:
        //     Servo-Daten
        BlockSD = 77,
        //
        // Summary:
        //     Applikationsspezifische Daten
        BlockTAD = 78,
        //
        // Summary:
        //     Applikationsspezifische Schneidendaten
        BlockTAO = 79,
        //
        // Summary:
        //     Applikationsspezifische Überwachungsdaten
        BlockTAS = 80,
        //
        // Summary:
        //     Applikationsspezifische Magazindaten
        BlockTAM = 81,
        //
        // Summary:
        //     Applikationsspezifische Magazinplatzdaten
        BlockTAP = 82,
        BlockMEM = 83,
        //
        // Summary:
        //     Alarm-Ereignisse, ältestes zuerst
        BlockSALAC = 84,
        //
        // Summary:
        //     Aktive Hilfsfunktionen
        BlockAUXFU = 85,
        BlockTDC = 86,
        BlockCP = 87,
        eBCK_BlockSDME = 110,
        //
        // Summary:
        //     Programmzeiger bei Unterbrechung
        BlockSPARPI = 111,
        //
        // Summary:
        //     erweiterte Zustandsdaten im WKS
        BlockSEGA = 112,
        //
        // Summary:
        //     Erweiterte Zustandsdaten im MKS
        BlockSEMA = 113,
        //
        // Summary:
        //     Zustandsdaten Spindel
        BlockSSP = 114,
        //
        // Summary:
        //     Zustandsdaten im WKS
        BlockSGA = 115,
        //
        // Summary:
        //     Zustandsdaten im MKS
        BlockSMA = 116,
        //
        // Summary:
        //     Letzter Alarm
        BlockSALAL = 117,
        //
        // Summary:
        //     Alarm mit top Priorität
        BlockSALAP = 118,
        BlockSALA = 119,
        //
        // Summary:
        //     Synchronaktionen
        BlockSSYNAC = 120,
        //
        // Summary:
        //     Programmzeiger für Satzsuchlauf
        BlockSPARPF = 121,
        //
        // Summary:
        //     Programmzeiger im Automatikbetrieb
        BlockSPARPP = 122,
        //
        // Summary:
        //     Aktive G-Funktionen
        BlockSNCF = 123,
        //
        // Summary:
        //     Teileprogramminformation
        BlockSPARP = 125,
        //
        // Summary:
        //     Teileprogrammspezifische Zustandsdaten
        BlockSINF = 126,
        //
        // Summary:
        //     Zustandsdaten
        BlockS = 127,
        Block0x80 = 128,
        Block0x81 = 129,
        Block0x82 = 130,
        Block0x83 = 131,
        Block0x84 = 132,
        Block0x85 = 133,
        //
        // Summary:
        //     Intern
        BlockO = 253,
        //
        // Summary:
        //     Unbekannt
        BlockUnknown = 255,
    }

    public enum NCK_BlockTVColumn
    {
        One=1,
        Two=2,
        TNo = 3,
        Identnumber = 4,
        Duplo = 5,
        Edges= 6,
        Depot = 7,
        Place = 8,
        Nine= 9
    }

    public enum NCK_BlockTSLine
    {  /////////////////////////////////////////////////////////
        ////// Beginn mit Datenbaustein TS (Schneidedaten, Korrekturdaten):

        //    //P1 = Vorwarngrenze Standzeit in Minuten ($TC_MOP1)
        //    //P2 = Verbleibende Standzeit in Minuten ($TC_MOP2)
        //    //P3 = Vorwarngrenze Stückzahl ($TC_MOP3)
        //    //P4 = verbleibende Stückzahl ($TC_MOP4)
        //    //P5 = Sollstandzeit ($TC_MOP11)
        //    //P6 = Sollstückzahl ($TC_MOP13)
        //    //P7 = Vorwarngrenze Verschleiß (Vorwarngrenze) (ab SW 5.1) ($TC_MOP5)
        //    //    Dieser Parameter kann nur gesetzt werden, wenn Bit 5 von Maschinendatum     $MN_MM_TOOL_MANAGEMENT_MASK entsprechend gesetzt ist.
        //    //P8 = verbleibender Verschleiß (Istwert) (ab SW 5.1) ($TC_MOP6) nicht schreibbar
        //    //P9 = Sollwert Verschleiß (ab SW 5.1) ($TC_MOP15)
        ThreshholdTime = 1,
        Resdurabillity = 2,
        ThreshholdParts = 3,
        RestParts = 4,
        SetTime = 5,
        SetParts = 6,
        ThreshholdWastage = 7,
        RestWastage = 8,
        SetWastage = 9
    }
    //public enum NCK_BlockTSLine
    //{
    //    One = 1,
    //    Two = 2,
    //    TNo = 3,
    //    Four = 4,
    //    Five = 5,
    //    Six = 6,
    //    Seven = 7,
    //    Eight = 8,
    //    Nine = 9
    //}
}
