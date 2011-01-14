using System;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    public class DiagnosticEntry
    {
        public DateTime TimeStamp;
        public string Message;
        public DiagnosticEntry(int id)
        {
            _id = id;
            switch (_id)
            {
                case (0x1381):
                    Message = "Manuelle Neustart- (Warmstart-) -Anforderung";
                    break;
                case (0x4301):
                    Message = "Betriebszustandsübergang von STOP nach ANLAUF";
                    break;
                case (0x4302):
                    Message = "Betriebszustandsübergang von ANLAUF nach RUN";
                    break;
                case (0x4303):
                    Message = "STOP durch Stopschalter-Bedienung";
                    break;
                case (0x4304):
                    Message = "STOP durch PG-Stop-Bedienung oder wegen SFB 20 \"STOP\"";
                    break;
                case (0x4305):
                    Message = "HALT: Haltepunkt erreicht";
                    break;
                case (0x4306):
                    Message = "HALT: Haltepunkt verlassen";
                    break;
                case (0x4307):
                    Message = "Start Urlöschen durch PG-Bedienung";
                    break;
                case (0x4308):
                    Message = "Start Urlöschen durch Schalterbedienung";
                    break;
                case (0x4309):
                    Message = "Start Urlöschen automatisch (ungepuffertes NETZ-EIN)";
                    break;
                case (0x430A):
                    Message = "HALT verlassen, Übergang in den STOP";
                    break;
                case (0x430D):
                    Message = "STOP durch andere CPU bei Multicomputing";
                    break;
                case (0x430E):
                    Message = "Urlöschen durchgeführt";
                    break;
                case (0x430F):
                    Message = "STOP der Baugruppe durch STOP einer CPU";
                    break;
                case (0x4510):
                    Message = "STOP wegen Verletzung des Datumsbereichs der CPU";
                    break;
                case (0x4318):
                    Message = "Beginn des CiR-Vorgangs";
                    break;
                case (0x4319):
                    Message = "CiR-Vorgang beendet";
                    break;
                case (0x113A):
                    Message = "Startanforderung für Weckalarm-OB mit Sonderbehandlung (nur S7-300)";
                    break;
                case (0x1155):
                    Message = "Statusalarm bei PROFIBUS DP";
                    break;
                case (0x1156):
                    Message = "Update-Alarm bei PROFIBUS DP";
                    break;
                case (0x1157):
                    Message = "Manufacture Alarm bei PROFIBUS DP";
                    break;
                case (0x1158):
                    Message = "Statusalarm bei PROFINET IO";
                    break;
                case (0x1159):
                    Message = "Update-Alarm bei PROFINET IO";
                    break;
                case (0x115A):
                    Message = "Manufacture Alarm bei PROFINET IO";
                    break;
                case (0x115B):
                    Message = "IO: Profile Specific Alarm";
                    break;
                case (0x116A):
                    Message = "Technologiesynchronalarm";
                    break;
                case (0x1382):
                    Message = "Automatische Neustart- (Warmstart-) -Anforderung";
                    break;
                case (0x1383):
                    Message = "Manuelle Wiederanlaufanforderung";
                    break;
                case (0x1384):
                    Message = "Automatische Wiederanlaufanforderung";
                    break;
                case (0x1385):
                    Message = "Manuelle Kaltstartanforderung";
                    break;
                case (0x1386):
                    Message = "Automatische Kaltstartanforderung";
                    break;
                case (0x1387):
                    Message = "Master-CPU: Manuelle Kaltstartanforderung";
                    break;
                case (0x1388):
                    Message = "Master-CPU: Automatische Kaltstartanforderung";
                    break;
                case (0x138A):
                    Message = "Master-CPU: Manuelle Neustart- (Warmstart-) -Anforderung";
                    break;
                case (0x138B):
                    Message = "Master-CPU: Automatische Neustart- (Warmstart-) -Anforderung";
                    break;
                case (0x138C):
                    Message = "Reserve-CPU: Manuelle Anlaufanforderung";
                    break;
                case (0x138D):
                    Message = "Reserve-CPU: Automatische Anlaufanforderung";
                    break;
                case (0x2521):
                    Message = "BCD - Wandlungsfehler";
                    _ob = 121;
                    break;
                case (0x2522):
                    Message = "Bereichslängenfehler beim Lesen";
                    _ob = 121;
                    break;
                case (0x2523):
                    Message = "Bereichslängenfehler beim Schreiben";
                    _ob = 121;
                    break;
                case (0x2524):
                    Message = "Bereichsfehler beim Lesen";
                    _ob = 121;
                    break;
                case (0x2525):
                    Message = "Bereichsfehler beim Schreiben";
                    _ob = 121;
                    break;
                case (0x2526):
                    Message = "Timer-Nummernfehler";
                    _ob = 121;
                    break;
                case (0x2527):
                    Message = "Zähler-Nummernfehler";
                    _ob = 121;
                    break;
                case (0x2528):
                    Message = "Ausrichtungsfehler beim Lesen";
                    _ob = 121;
                    break;
                case (0x2529):
                    Message = "Ausrichtungsfehler beim Schreiben";
                    _ob = 121;
                    break;
                case (0x2530):
                    Message = "Schreibfehler beim Zugriff auf den DB";
                    _ob = 121;
                    break;
                case (0x2531):
                    Message = "Schreibfehler beim Zugriff auf den DI";
                    _ob = 121;
                    break;
                case (0x2532):
                    Message = "Bausteinnummernfehler beim Aufschlagen eines DB";
                    _ob = 121;
                    break;
                case (0x2533):
                    Message = "Bausteinnummernfehler beim Aufschlagen eines DI";
                    _ob = 121;
                    break;
                case (0x2534):
                    Message = "Bausteinnummernfehler beim FC-Aufruf";
                    _ob = 121;
                    break;
                case (0x2535):
                    Message = "Bausteinnummernfehler beim FB-Aufruf";
                    _ob = 121;
                    break;
                case (0x253A):
                    Message = "DB nicht geladen";
                    _ob = 121;
                    break;
                case (0x253C):
                    Message = "FC nicht geladen";
                    _ob = 121;
                    break;
                case (0x253D):
                    Message = "SFC nicht geladen";
                    _ob = 121;
                    break;
                case (0x253E):
                    Message = "FB nicht geladen";
                    _ob = 121;
                    break;
                case (0x253F):
                    Message = "SFB nicht geladen";
                    _ob = 121;
                    break;
                case (0x2942):
                    Message = "Peripherie-Zugriffsfehler, lesend";
                    _ob = 121;
                    break;
                case (0x2943):
                    Message = "Peripherie-Zugriffsfehler, schreibend";
                    _ob = 121;
                    break;
                case (0x3501):
                    Message = "Zykluszeitüberschreitung";
                    _ob = 80;
                    break;
                case (0x3502):
                    Message = "Anwenderschnittstelle (OB bzw. FRB) -Anforderungsfehler";
                    _ob = 80;
                    break;
                case (0x3503):
                    Message = "Zu lange Verzögerung der Bearbeitung einer Prioritätsklasse";
                    _ob = 80;
                    break;
                case (0x3505):
                    Message = "Uhrzeitalarm(e) abgelaufen durch Uhrzeitsprung";
                    _ob = 80;
                    break;
                case (0x3506):
                    Message = "Uhrzeitalarm(e) abgelaufen bei Wiedereintritt in RUN nach HALT";
                    _ob = 80;
                    break;
                case (0x3507):
                    Message = "Mehrfacher OB-Anforderungsfehler verursachte einen internen Puffer-Überlauf";
                    _ob = 80;
                    break;
                case (0x3508):
                    Message = "Taktsynchronalarm-Zeitfehler";
                    _ob = 80;
                    break;
                case (0x3509):
                    Message = "Alarmverlust durch zu hohe Alarmlast";
                    _ob = 80;
                    break;
                case (0x350A):
                    Message = "Wiederintritt in RUN nach CiR";
                    _ob = 80;
                    break;
                case (0x350B):
                    Message = "Technologiesynchronalarm-Zeitfehler";
                    _ob = 80;
                    break;
                case (0x3921):
                    Message = "BATTF: Ausfall mindestens einer Pufferbatterie im Zentralgerät/ beseitigt Hinweis: Das kommende Ereignis tritt nur beim Ausfall einer der Batterien (bei redundanten Pufferbatterien) auf. Fällt anschließend auch noch die andere Batterie aus, tritt das Ereignis nicht nochmals auf.";
                    _ob = 81;
                    break;
                case (0x3821):
                    Message = "BATTF: Ausfall mindestens einer Pufferbatterie im Zentralgerät/ beseitigt Hinweis: Das kommende Ereignis tritt nur beim Ausfall einer der Batterien (bei redundanten Pufferbatterien) auf. Fällt anschließend auch noch die andere Batterie aus, tritt das Ereignis nicht nochmals auf.";
                    _ob = 81;
                    break;
                case (0x3922):
                case (0x3822):
                    Message = "BAF: Ausfall der Pufferspannung im Zentralgerät/ beseitigt";
                    _ob = 81;
                    break;
                case (0x3923):
                case (0x3823):
                    Message = "Ausfall der 24V-Versorgung im Zentralgerät/ beseitigt";
                    _ob = 81;
                    break;
                case (0x3925):
                case (0x3825):
                    Message = "BATTF: Ausfall mindestens einer Pufferbatterie in einem redundanten Zentralgerät/ beseitigt";
                    _ob = 81;
                    break;
                case (0x3926):
                case (0x3826):
                    Message = "BAF: Ausfall der Pufferspannung in einem redundanten Zentralgerät/ beseitigt";
                    _ob = 81;
                    break;
                case (0x3927):
                case (0x3827):
                    Message = "Ausfall der 24V-Versorgung in einem redundanten Zentralgerät/ beseitigt";
                    _ob = 81;
                    break;
                case (0x3931):
                case (0x3831):
                    Message = "BATTF: Ausfall mindestens einer Pufferbatterie in mindestens einem Erweiterungsgerät/ beseitigt";
                    _ob = 81;
                    break;
                case (0x3932):
                case (0x3832):
                    Message = "BAF: Ausfall der Pufferspannung in mindestens einem Erweiterungsgerät/ beseitigt";
                    _ob = 81;
                    break;
                case (0x3933):
                case (0x3833):
                    Message = "Ausfall der 24V-Versorgung in mindestens einem Erweiterungsgerät/ beseitigt";
                    _ob = 81;
                    break;
                case (0x3942):
                    Message = "Baugruppe gestört";
                    _ob = 82;
                    break;
                case (0x3842):
                    Message = " Baugruppe o. k.";
                    _ob = 82;
                    break;
                case (0x3951):
                    Message = "PROFINET IO-Modul gezogen";
                    _ob = 83;
                    break;
                case (0x3954):
                    Message = "PROFINET IO-Submodul/Modul gezogen";
                    _ob = 83;
                    break;
                case (0x3854):
                    Message = "PROFINET IO-Submodul/Modul gesteckt und entspricht parametriertem Submodul/Modul";
                    _ob = 83;
                    break;
                case (0x3855):
                    Message = "PROFINET IO-Submodul/Modul gesteckt, entspricht aber nicht dem parametrierten Submodul/Modul";
                    _ob = 83;
                    break;
                case (0x3856):
                    Message = "PROFINET IO-Submodul/Modul gesteckt, jedoch Fehler bei Baugruppenparametrierung";
                    _ob = 83;
                    break;
                case (0x3858):
                    Message = "PROFINET IO-Submodul Zugriffsfehler beseitigt";
                    _ob = 83;
                    break;
                case (0x3861):
                    Message = "Baugruppe / Schnittstellenmodul gesteckt, Baugruppentyp o.k.";
                    _ob = 83;
                    break;
                case (0x3961):
                    Message = "Baugruppe / Schnittstellenmodul gezogen bzw. nicht ansprechbar";
                    _ob = 83;
                    break;
                case (0x3863):
                    Message = "Baugruppe / Schnittstellenmodul gesteckt, jedoch falscher Baugruppentyp";
                    _ob = 83;
                    break;
                case (0x3864):
                    Message = "Baugruppe / Schnittstellenmodul gesteckt, jedoch gestört (Baugruppenkennung nicht lesbar)";
                    _ob = 83;
                    break;
                case (0x3865):
                    Message = "Baugruppe gesteckt, jedoch Fehler bei Baugruppenparametrierung";
                    _ob = 83;
                    break;
                case (0x3866):
                    Message = "Baugruppe wieder ansprechbar, Lastspannungsfehler beseitigt";
                    _ob = 83;
                    break;
                case (0x3966):
                    Message = "Baugruppe nicht ansprechbar, Lastspannungsfehler";
                    _ob = 83;
                    break;
                case (0x3367):
                    Message = "Beginn Umparametrieren einer Baugruppe";
                    _ob = 83;
                    break;
                case (0x3267):
                    Message = "Ende Umparametrieren einer Baugruppe";
                    _ob = 83;
                    break;
                case (0x3968):
                    Message = "Umparametrieren einer Baugruppe mit Fehler beendet";
                    _ob = 83;
                    break;
                case (0x3571):
                    Message = "Zu große Schachtelungstiefe von Klammerebenen";
                    _ob = 88;
                    break;
                case (0x3572):
                    Message = "Zu große Schachtelungstiefe von Master Control Relais";
                    _ob = 88;
                    break;
                case (0x3573):
                    Message = "Zu große Schachtelungstiefe bei Synchronfehlern";
                    _ob = 88;
                    break;
                case (0x3574):
                    Message = "Zu große Schachtelungstiefe von Bausteinaufrufen (U-Stack)";
                    _ob = 88;
                    break;
                case (0x3575):
                    Message = "Zu große Schachtelungstiefe von Bausteinaufrufen (B-Stack)";
                    _ob = 88;
                    break;
                case (0x3576):
                    Message = "Fehler beim Allokieren von Lokaldaten";
                    _ob = 88;
                    break;
                case (0x3578):
                    Message = "Unbekannte Anweisung";
                    _ob = 88;
                    break;
                case (0x357A):
                    Message = "Sprunganweisung mit Ziel außerhalb des Bausteins";
                    _ob = 88;
                    break;
                case (0x3884):
                    Message = "Schnittstellenmodul gesteckt";
                    _ob = 83;
                    break;
                case (0x3984):
                    Message = "Schnittstellenmodul gezogen";
                    _ob = 83;
                    break;
                case (0x3981):
                    Message = "Schnittstellenfehler, kommend";
                    _ob = 84;
                    break;
                case (0x3881):
                    Message = "Schnittstellenfehler, gehend";
                    _ob = 84;
                    break;
                case (0x3582):
                    Message = "Speicherfehler vom Betriebssystem erkannt und beseitigt";
                    _ob = 84;
                    break;
                case (0x3583):
                    Message = "Häufung von erkannten und korrigierten Speicherfehlern";
                    _ob = 84;
                    break;
                case (0x3585):
                    Message = "Fehler im PC-Betriebssystem (nur bei Win LC RTX)";
                    _ob = 84;
                    break;
                case (0x3986):
                    Message = "Leistung einer H-Sync-Kopplung beeinträchtigt";
                    _ob = 84;
                    break;
                case (0x3587):
                    Message = "Mehrbitspeicherfehler erkannt und korrigiert";
                    _ob = 84;
                    break;
                case (0x35A1):
                    Message = "Anwenderschnittstelle (OB bzw. FRB) nicht vorhanden";
                    _ob = 85;
                    break;
                case (0x35A2):
                    Message = "OB nicht geladen (gestartet durch SFC oder durch Besy aufgrund Projektierung)";
                    _ob = 85;
                    break;
                case (0x35A3):
                    Message = "Fehler beim Zugriff durch Besy auf einen Baustein";
                    _ob = 85;
                    break;
                case (0x35A4):
                    Message = "PROFInet Interface-DB nicht ansprechbar";
                    _ob = 85;
                    break;
                case (0x34A4):
                    Message = "PROFInet Interface-DB wieder ansprechbar";
                    _ob = 85;
                    break;
                case (0x39B1):
                    Message = "Peripheriezugriffsfehler bei Prozeßabbildaktualisierung der Eingänge";
                    _ob = 85;
                    break;
                case (0x39B2):
                    Message = "Peripheriezugriffsfehler bei der Übertragung des Prozeßabbilds zu den Ausgabebaugruppen";
                    _ob = 85;
                    break;
                case (0x39B3):
                case (0x38B3):
                    Message = "Peripheriezugriffsfehler bei Prozeßabbildaktualisierung der Eingänge";
                    _ob = 85;
                    break;
                case (0x39B4):
                case (0x38B4):
                    Message = "Peripheriezugriffsfehler bei der Übertragung des Prozeßabbilds zu den Ausgabebaugruppen";
                    _ob = 85;
                    break;
                case (0x38C1):
                    Message = "Wiederkehr Erweiterungsgerät (1 bis 21), gehend";
                    _ob = 86;
                    break;
                case (0x39C1):
                    Message = "Ausfall Erweiterungsgerät (1 bis 21), kommend";
                    _ob = 86;
                    break;
                case (0x38C2):
                    Message = "Erweiterungsgerätwiederkehr mit Abweichung Soll-/Istausbau";
                    _ob = 86;
                    break;
                case (0x39C3):
                    Message = "Dezentrale Peripherie: Mastersystemausfall kommend";
                    _ob = 86;
                    break;
                case (0x39C4):
                    Message = "Dezentrale Peripherie: Station ausgefallen, kommend";
                    _ob = 86;
                    break;
                case (0x38C4):
                    Message = "Dezentrale Peripherie: Station ausgefallen, gehend";
                    _ob = 86;
                    break;
                case (0x39C5):
                    Message = "Dezentrale Peripherie: Station gestört, kommend";
                    _ob = 86;
                    break;
                case (0x38C5):
                    Message = "Dezentrale Peripherie: Station gestört, gehend";
                    _ob = 86;
                    break;
                case (0x38C6):
                    Message = "Erweiterungsgerätewiederkehr, jedoch Fehler bei Baugruppenparametrierung";
                    _ob = 86;
                    break;
                case (0x38C7):
                    Message = "DP: Stationswiederkehr, jedoch Fehler bei Baugruppenparametrierung";
                    _ob = 86;
                    break;
                case (0x38C8):
                    Message = "DP: Stationswiederkehr mit Abweichung Soll-/Istausbau";
                    _ob = 86;
                    break;
                case (0x39CA):
                    Message = "PROFINET IO-Systemausfall";
                    _ob = 86;
                    break;
                case (0x39CB):
                    Message = "PROFINET IO-Stationsausfall";
                    _ob = 86;
                    break;
                case (0x38CB):
                    Message = "PROFINET IO-Stationswiederkehr";
                    _ob = 86;
                    break;
                case (0x39CC):
                    Message = "PROFINET IO-Station gestört";
                    _ob = 86;
                    break;
                case (0x38CC):
                    Message = "PROFINET IO-Station Störung beseitigt";
                    _ob = 86;
                    break;
                case (0x39CD):
                    Message = "PROFINET IO-Stationswiederkehr, Sollausbau weicht von Istausbau ab.";
                    _ob = 86;
                    break;
                case (0x39CE):
                    Message = "PROFINET IO-Stationswiederkehr, Fehler bei der Baugruppenparametrierung";
                    _ob = 86;
                    break;
                case (0x35D2):
                    Message = "Senden der Diagnoseeinträge derzeit nicht möglich";
                    _ob = 87;
                    break;
                case (0x35D3):
                    Message = "Synchronisationstelegramme können nicht gesendet werden";
                    _ob = 87;
                    break;
                case (0x35D4):
                    Message = "Unzulässiger Uhrzeitsprung durch Uhrzeitsynchronisation";
                    _ob = 87;
                    break;
                case (0x35D5):
                    Message = "Fehler bei Übernahme der Synchronisationszeit";
                    _ob = 87;
                    break;
                case (0x35E1):
                    Message = "Falsche Telegrammkennung bei GD";
                    _ob = 87;
                    break;
                case (0x35E2):
                    Message = "GD-Paketstatus nicht in DB eintragbar";
                    _ob = 87;
                    break;
                case (0x35E3):
                    Message = "Telegrammlängenfehler bei GD";
                    _ob = 87;
                    break;
                case (0x35E4):
                    Message = "Unzulässige GD-Paketnummer empfangen";
                    _ob = 87;
                    break;
                case (0x35E5):
                    Message = "Fehler beim Zugriff auf DB bei Kommunikations-SFBs für projektierte S7-Verbindungen";
                    _ob = 87;
                    break;
                case (0x35E6):
                    Message = "GD-Gesamtstatus nicht in DB eintragbar";
                    _ob = 87;
                    break;
                case (0x4300):
                    Message = "NETZ-EIN gepuffert";
                    break;
                case (0x4520):
                    Message = "DEFEKT: STOP nicht erreichbar";
                    break;
                case (0x4521):
                    Message = "DEFEKT: Ausfall des Befehlsbearbeitungsprozessors";
                    break;
                case (0x4522):
                    Message = "DEFEKT: Ausfall des Uhrenbausteins";
                    break;
                case (0x4523):
                    Message = "DEFEKT: Ausfall des Zeittaktgebers";
                    break;
                case (0x4524):
                    Message = "DEFEKT: Ausfall der Zeitzellenaktualisierung";
                    break;
                case (0x4525):
                    Message = "DEFEKT: Ausfall der Synchronisation bei Multicomputing";
                    break;
                case (0x4926):
                    Message = "DEFEKT: Ausfall der Zeitüberwachung bei Peripheriezugriffen";
                    break;
                case (0x4527):
                    Message = "DEFEKT: Ausfall der Peripheriezugriffsüberwachung";
                    break;
                case (0x4528):
                    Message = "DEFEKT: Ausfall der Zykluszeitüberwachung";
                    break;
                case (0x4530):
                    Message = "DEFEKT: Speichertestfehler im internen Speicher";
                    break;
                case (0x4931):
                    Message = "STOP bzw. DEFEKT: Speichertestfehler im Modulspeicher";
                    break;
                case (0x4532):
                    Message = "DEFEKT: Ausfall von Kernressourcen";
                    break;
                case (0x4933):
                    Message = "Quersummenfehler";
                    break;
                case (0x4934):
                    Message = "DEFEKT: Speicher nicht vorhanden";
                    break;
                case (0x4935):
                    Message = "DEFEKT: Abbruch durch Watchdog/processor exceptions";
                    break;
                case (0x4536):
                    Message = "DEFEKT: Betriebsartenschalter defekt";
                    break;
                case (0x4540):
                    Message = "STOP:Speichererweiterung desinternen Arbeitsspeichers nicht lückenlos. Erste Speichererweiterung ist zu klein oder fehlt.";
                    break;
                case (0x4541):
                    Message = "STOP durch das Prioritätsklassen-Ablaufsystem";
                    break;
                case (0x4542):
                    Message = "STOP durch Objektverwaltungssystem";
                    break;
                case (0x4543):
                    Message = "STOP durch Test und Inbetriebsetzung";
                    break;
                case (0x4544):
                    Message = "STOP durch Diagnosesystem";
                    break;
                case (0x4545):
                    Message = "STOP durch Kommunikationssystem";
                    break;
                case (0x4546):
                    Message = "STOP durch CPU-Speicherverwaltung";
                    break;
                case (0x4547):
                    Message = "STOP durch Prozeßabbildverwaltung";
                    break;
                case (0x4548):
                    Message = "STOP durch Peripherieverwaltung";
                    break;
                case (0x4949):
                    Message = "STOP wegen Dauer-Prozeßalarm";
                    break;
                case (0x454A):
                    Message = "STOP durch Projektierung: Ein mit STEP 7 abgewählter OB war beim Anlauf in der CPU geladen.";
                    break;
                case (0x494D):
                    Message = "STOP durch Peripheriefehler";
                    break;
                case (0x494E):
                    Message = "STOP durch Netzausfall";
                    break;
                case (0x494F):
                    Message = "STOP durch Konfigurationsfehler";
                    break;
                case (0x4550):
                    Message = "DEFEKT: interner Systemfehler";
                    break;
                case (0x4555):
                    Message = "Wiederanlauf nicht möglich, da Überwachungszeitgrenze abgelaufen";
                    break;
                case (0x4556):
                    Message = "STOP: Urlöschanforderung durch Kommunikation / Dateninkonsistenz";
                    break;
                case (0x4357):
                    Message = "Baugruppenüberwachungszeit gestartet";
                    break;
                case (0x4358):
                    Message = "Alle Baugruppen sind betriebsbereit";
                    break;
                case (0x4959):
                    Message = "Nicht alle Baugruppen sind betriebsbereit";
                    break;
                case (0x4562):
                    Message = "STOP durch Programmierfehler (OB nicht geladen oder nicht möglich)";
                    break;
                case (0x4563):
                    Message = "STOP durch Peripheriezugriffsfehler (OB nicht geladen oder nicht möglich)";
                    break;
                case (0x4567):
                    Message = "STOP durch H-Ereignis";
                    break;
                case (0x4568):
                    Message = "STOP durch Zeitfehler (OB nicht geladen oder nicht möglich)";
                    break;
                case (0x456A):
                    Message = "STOP durch Diagnosealarm (OB nicht geladen oder nicht möglich)";
                    break;
                case (0x456B):
                    Message = "STOP durch Ziehen/Stecken (OB nicht geladen oder nicht möglich)";
                    break;
                case (0x456C):
                    Message = "STOP durch CPU-Hardwarefehler (OB nicht geladen oder nicht möglich)";
                    break;
                case (0x456D):
                    Message = "STOP durch Programmablauffehler (OB nicht geladen oder nicht möglich)";
                    break;
                case (0x456E):
                    Message = "STOP durch Kommunikationsfehler (OB nicht geladen oder nicht möglich)";
                    break;
                case (0x456F):
                    Message = "STOP durch Baugruppenträgerausfall (OB nicht geladen oder nicht möglich)";
                    break;
                case (0x4570):
                    Message = "STOP durch Bearbeitungsabbruch (OB nicht geladen oder nicht möglich)";
                    break;
                case (0x4571):
                    Message = "STOP durch Klammerstackfehler";
                    break;
                case (0x4572):
                    Message = "STOP durch Master-Control-Relais-Stackfehler";
                    break;
                case (0x4573):
                    Message = "STOP durch Überschreiten der Schachtelungstiefe bei Synchronfehlern";
                    break;
                case (0x4574):
                    Message = "STOP durch zu große U-Stack-Verschachtelung im Prioritätsklassen-Stack";
                    break;
                case (0x4575):
                    Message = "STOP durch zu große B-Stack-Verschachtelung im Prioritätsklassen-Stack";
                    break;
                case (0x4576):
                    Message = "STOP durch Fehler beim Allokieren von Lokaldaten";
                    break;
                case (0x4578):
                    Message = "STOP durch unbekannten Opcode";
                    break;
                case (0x457A):
                    Message = "STOP durch Codelängenfehler";
                    break;
                case (0x457B):
                    Message = "STOP durch nicht geladenen DB bei Onboard-Peripherie";
                    break;
                case (0x497C):
                    Message = "STOP durch integrierte Technologie";
                    break;
                case (0x457D):
                    Message = "Urlöschanforderung, weil die Version der internen Schnittstelle zur integrierten Technologie geändert wurde";
                    break;
                case (0x457F):
                    Message = "STOP durch STOP-Befehl";
                    break;
                case (0x4580):
                    Message = "STOP: Backup-Pufferinhalt inkonsistent (kein RUN-Übergang)";
                    break;
                case (0x4590):
                    Message = "STOP wegen Überlast der Internen Funktionen";
                    break;
                case (0x49A0):
                    Message = "STOP wegen Parametrierfehler oder unzulässige Differenz zwischen Soll- und Istausbau: Anlauf gesperrt";
                    break;
                case (0x49A1):
                    Message = "STOP wegen Parametrierfehler: Urlöschanforderung";
                    break;
                case (0x49A2):
                    Message = "STOP wegen Fehler beim Nachparametrieren:Anlauf gesperrt";
                    break;
                case (0x49A3):
                    Message = "STOP wegen Fehler beim Nachparametrieren: Urlöschanforderung";
                    break;
                case (0x49A4):
                    Message = "STOP: Inkonsistenz der Projektierungsdaten";
                    break;
                case (0x49A5):
                    Message = "STOP: Dezentrale Peripherie: Unstimmigkeiten der geladenen Projektierinformation";
                    break;
                case (0x49A6):
                    Message = "STOP: Dezentrale Peripherie: ungültige Projektierinformation";
                    break;
                case (0x49A7):
                    Message = "STOP: Dezentrale Peripherie: Projektierinformation nicht vorhanden";
                    break;
                case (0x49A8):
                    Message = "STOP: Fehleranzeige der Anschaltung für Dezentrale Peripherie";
                    break;
                case (0x43B0):
                    Message = "Firmwareupdate erfolgreich durchgeführt";
                    break;
                case (0x49B1):
                    Message = "Fehlerhafte Firmwareupdate-Daten";
                    break;
                case (0x49B2):
                    Message = "Firmwareupdate: Hardwarestand paßt nicht zur Firmware";
                    break;
                case (0x49B3):
                    Message = "Firmwareupdate: Baugruppentyp paßt nicht zur Firmware";
                    break;
                case (0x43B4):
                    Message = "Fehler bei der Firmware-Sicherung";
                    break;
                case (0x43B6):
                    Message = "Abbruch des Firmware-Updates von redundanten Baugruppen";
                    break;
                case (0x43D0):
                    Message = "Abweisung ANKOPPELN wegen Verletzung von Koordinierungsregeln";
                    break;
                case (0x43D1):
                    Message = "Abbruch der Sequenz ANKOPPELN/AUFDATEN";
                    break;
                case (0x49D2):
                    Message = "STOP der Reserve-CPU wegen STOP der Master-CPU während der Ankopplung";
                    break;
                case (0x43D3):
                    Message = "STOP einer Reserve-CPU";
                    break;
                case (0x49D4):
                    Message = "STOP eines Masters, da Partner-CPU auch Master ist (Kopplungsfehler)";
                    break;
                case (0x43D5):
                    Message = "Abweisung ANKOPPELN wegen ungleichem Speicherausbau des Teil-AS";
                    break;
                case (0x43D6):
                    Message = "Abweisung ANKOPPELN wegen ungleichem Systemprogramm des Teil-AS";
                    break;
                case (0x43D7):
                    Message = "Abweisung ANKOPPELN wegen Konfigurationsänderung";
                    break;
                case (0x49D8):
                    Message = "STOP/Fehlersuchbetrieb/DEFEKT: Hardwarefehler durch anderen Fehler erkannt";
                    break;
                case (0x49D9):
                    Message = "STOP wegen Synchronisationsmodul-Fehler";
                    break;
                case (0x49DA):
                    Message = "STOP wegen Synchronisationsfehler zwischen H-CPUs";
                    break;
                case (0x43DC):
                    Message = "Abbruch beim Ankoppeln mit Umschalten";
                    break;
                case (0x43DD):
                    Message = "Abweisung ANKOPPELN wegen laufender Test- oder anderer Online-Funktionen";
                    break;
                case (0x43DE):
                    Message = "Abbruch des Aufdatvorgangs wegen Überschreitung einer Überwachungszeit beim nten Versuch, erneuter Aufdatversuch initiiert";
                    break;
                case (0x43DF):
                    Message = "Endgültiger Abbruch des Aufdatvorgangs wegen Überschreitung einer Überwachungszeit nach der maximalen Anzahl von Versuchen, erneute Bedienung erforderlich";
                    break;
                case (0x43E0):
                    Message = "Wechsel von Solobetrieb nach Ankoppeln";
                    break;
                case (0x43E1):
                    Message = "Wechsel von Ankoppeln nach Aufdaten";
                    break;
                case (0x43E2):
                    Message = "Wechsel vom Systemzustand Aufdaten in Redundant";
                    break;
                case (0x43E3):
                    Message = "Master-CPU: Wechsel vom Systemzustand Redundant nach Solobetrieb";
                    break;
                case (0x43E4):
                    Message = "Reserve-CPU: Wechsel vom Systemzustand Redundant nach FEHLERSUCHE";
                    break;
                case (0x43E5):
                    Message = "Reserve-CPU: Wechsel von FEHLERSUCHE nach Ankoppeln oder STOP";
                    break;
                case (0x43E6):
                    Message = "Abbruch Ankoppeln der Reserve-CPU";
                    break;
                case (0x43E7):
                    Message = "Abbruch Aufdaten der Reserve-CPU";
                    break;
                case (0x43E8):
                    Message = "Reserve-CPU: Wechsel von Ankoppeln nach Anlauf";
                    break;
                case (0x43E9):
                    Message = "Reserve-CPU: Wechsel von Anlauf nach Aufdaten";
                    break;
                case (0x43F1):
                    Message = "Reserve-Master-Umschaltung";
                    break;
                case (0x43F2):
                    Message = "Kopplung inkompatibler H-CPUs durch Systemprogramm blockiert";
                    break;
                case (0x42F3):
                    Message = "Prüfsummenfehler vom Betriebssystem erkannt und korrigiert";
                    break;
                case (0x43F4):
                    Message = "Reserve-CPU: Sperre des Ankoppelns/Aufdatens mittels SFC90 in der Master-CPU";
                    break;
                case (0x530D):
                    Message = "Neue Anlaufinformation im Betriebszustand STOP";
                    break;
                case (0x510F):
                    Message = "Bei WinLC ist ein Problem aufgetreten, das zum STOP oder Defekt der CPU führte.";
                    break;
                case (0x5311):
                    Message = "Anlauf trotz fehlender Fertigmeldung der Baugruppe(n)";
                    break;
                case (0x5545):
                    Message = "Beginn des Umparametrierens im Rahmen einer Anlagenänderung im laufenden Betrieb";
                    break;
                case (0x5445):
                    Message = "Ende des Umparametrierens im Rahmen einer Anlagenänderung im laufenden Betrieb";
                    break;
                case (0x5961):
                    Message = "Parametrierfehler";
                    break;
                case (0x5962):
                    Message = "Parametrierfehler mit Anlaufhindernis";
                    break;
                case (0x5963):
                    Message = "Parametrierfehler mit Urlöschanforderung";
                    break;
                case (0x5966):
                    Message = "Parametrierfehler beim Umschalten";
                    break;
                case (0x5969):
                    Message = "Parametrierfehler mit Anlaufhindernis";
                    break;
                case (0x596A):
                    Message = "PROFINET IO: IP-Adresse eines IO-Device bereits vorhanden";
                    break;
                case (0x596B):
                    Message = "IP-Adresse einer Ethernet-Schnittstelle bereits vorhanden";
                    break;
                case (0x596C):
                    Message = "Name einer Ethernet-Schnittstelle bereits vorhanden";
                    break;
                case (0x596D):
                    Message = "Die vorhandene Netzkonfiguration passt nicht zu den Systemanforderungen oder der Projektierung.";
                    break;
                case (0x5371):
                    Message = "Dezentrale Peripherie: Ende der Synchronisation mit einem DP-Master";
                    break;
                case (0x5979):
                case (0x5879):
                    Message = "Diagnosemeldung von DP-Anschaltung: EXTF-LED an/aus";
                    break;
                case (0x5380):
                    Message = "Diagnosepuffereinträge von Alarm- und asynchronen Fehlerereignissen gesperrt";
                    break;
                case (0x5581):
                    Message = "Eine oder mehrere Lizenzen für Runtime-Software fehlen.";
                    break;
                case (0x5481):
                    Message = "Alle Lizenzen für Runtime-Software sind wieder vollständig.";
                    break;
                case (0x558A):
                    Message = "Unterschied zwischen der MLFB der projektierten und der gesteckten CPU";
                    break;
                case (0x558B):
                    Message = "Unterschied zwischen der Firmware-Version der projektierten und der gesteckten CPU";
                    break;
                case (0x597C):
                    Message = "DP-Kommando Global Control ausgefallen oder verschoben";
                    break;
                case (0x5395):
                    Message = "Dezentrale Peripherie: Rücksetzen eines DP-Masters";
                    break;
                case (0x5598):
                    Message = "Beginn potentieller Inkonsistenz mit DP-Mastersystemen durch CiR";
                    break;
                case (0x5498):
                    Message = "Ende potentieller Inkonsistenz mit DP-Mastersystemen durch CiR";
                    break;
                case (0x59A0):
                    Message = "Alarm in der CPU nicht zuordenbar";
                    break;
                case (0x59A1):
                    Message = "Konfigurationsfehler der integrierten Technologie";
                    break;
                case (0x53A2):
                    Message = "Laden der Technologie-Firmware erfolgreich durchgeführt";
                    break;
                case (0x59A3):
                    Message = "Fehler beim Laden der integrierten Technologie";
                    break;
                case (0x53A4):
                    Message = "Laden des Technologie-DB nicht erfolgreich";
                    break;
                case (0x55A5):
                    Message = "Versionskonflikt der internen Schnittstelle zur integrierten Technologie";
                    break;
                case (0x55A6):
                    Message = "Die Maximalanzahl der Technologieobjekte wurde überschritten.";
                    break;
                case (0x55A7):
                    Message = "Es ist bereits ein Technologie-DB dieses Typs vorhanden.";
                    break;
                case (0x53FF):
                    Message = "Rücksetzen in den Auslieferungszustand";
                    break;
                case (0x6500):
                    Message = "Verbindungsreferenz (ID) auf Baugruppe doppelt vorhanden";
                    break;
                case (0x6501):
                    Message = "Verbindungsressourcen nicht ausreichend";
                    break;
                case (0x6502):
                    Message = "Fehler in der Verbindungsbeschreibung";
                    break;
                case (0x6905):
                case (0x6805):
                    Message = "Ressourcenproblem bei fest projektierten Verbindungen/ beseitigt";
                    break;
                case (0x6510):
                    Message = "CFB-Strukturfehler im Instanz-DB bei Auswertung EPROM erkannt";
                    break;
                case (0x6514):
                    Message = "GD-Paketnummer auf der Baugruppe doppelt vorhanden";
                    break;
                case (0x6515):
                    Message = "Inkonsistente Längenangaben in GD-Projektierungsinformation";
                    break;
                case (0x6316):
                    Message = "Schnittstellenfehler beim Hochlauf des AS";
                    break;
                case (0x6521):
                    Message = "Weder Modul noch interner Speicher vorhanden";
                    break;
                case (0x6522):
                    Message = "Unzulässiges Modul: Modultausch und Urlöschen erforderlich";
                    break;
                case (0x6523):
                    Message = "Urlöschanforderung durch Fehler bei Zugriff auf Modul";
                    break;
                case (0x6524):
                    Message = "Urlöschanforderung durch Fehler im Bausteinkopf";
                    break;
                case (0x6526):
                    Message = "Urlöschanforderung wegen Speichertausch";
                    break;
                case (0x6527):
                    Message = "Speichertausch, deshalb kein Wiederanlauf möglich";
                    break;
                case (0x6528):
                    Message = "Objekthandlingsfunktion im STOP/HALT, kein Wiederanlauf möglich";
                    break;
                case (0x6529):
                    Message = "Kein Anlauf möglich während der Funktion \"Anwenderprogramm laden\"";
                    break;
                case (0x652A):
                    Message = "Kein Anlauf, da Baustein im Anwenderspeicher doppelt vorhanden";
                    break;
                case (0x652B):
                    Message = "Kein Anlauf, da Bausteinlänge zu groß für Modul: Modultausch erforderlich";
                    break;
                case (0x652C):
                    Message = "Kein Anlauf wegen unzulässigem OB auf dem Modul";
                    break;
                case (0x6532):
                    Message = "Kein Anlauf wegen unzulässiger Projektierinformation auf Modul";
                    break;
                case (0x6533):
                    Message = "Urlöschanforderung durch ungültigen Modulinhalt";
                    break;
                case (0x6534):
                    Message = "Kein Anlauf: Baustein auf Modul mehrfach vorhanden";
                    break;
                case (0x6535):
                    Message = "Kein Anlauf: Nicht genügend Speicher, um Baustein aus Modul aufzunehmen";
                    break;
                case (0x6536):
                    Message = "Kein Anlauf: Modul enthält eine unzulässige Bausteinnummer";
                    break;
                case (0x6537):
                    Message = "Kein Anlauf: Modul enthält einen Baustein unzulässiger Länge";
                    break;
                case (0x6538):
                    Message = "Lokaldaten oder Schreibschutzkennung (bei DB) eines Bausteins für CPU unzulässig";
                    break;
                case (0x6539):
                    Message = "Unzulässiger Befehl im Baustein (vom Compiler erkannt)";
                    break;
                case (0x653A):
                    Message = "Urlöschanforderung, da OB-Lokaldaten auf Modul zu kurz sind";
                    break;
                case (0x6543):
                    Message = "Kein Anlauf: Bausteintyp unzulässig";
                    break;
                case (0x6544):
                    Message = "Kein Anlauf: Attribut \"ablaufrelevant\" unzulässig";
                    break;
                case (0x6545):
                    Message = "Erstellungssprache unzulässig";
                    break;
                case (0x6546):
                    Message = "Maximale Anzahl der Projektierbausteine erreicht";
                    break;
                case (0x6547):
                    Message = "Parametrierfehler beim Parametrieren von Baugruppen (nicht über P-Bus, sondern Abbruch Download)";
                    break;
                case (0x6548):
                    Message = "Plausibilitätsfehler bei Bausteinprüfung";
                    break;
                case (0x6549):
                    Message = "Strukturfehler im Baustein";
                    break;
                case (0x6550):
                    Message = "Ein Baustein hat im Prüfwert (CRC) einen Fehler";
                    break;
                case (0x6551):
                    Message = "Ein Baustein hat keinen Prüfwert (CRC)";
                    break;
                case (0x6353):
                    Message = "Firmware-Update: Beginn des Firmwaredownload über das Netz";
                    break;
                case (0x6253):
                    Message = "Firmware-Update: Ende des Firmwaredownload über das Netz";
                    break;
                case (0x6560):
                    Message = "SCAN-Overflow";
                    break;
                case (0x6981):
                    Message = "Schnittstellenfehler kommend";
                    break;
                case (0x6881):
                    Message = "Schnittstellenfehler gehend";
                    break;
                case (0x6390):
                    Message = "Formatieren einer Micro Memory Card durchgeführt";
                    break;
                case (0x73A2):
                    Message = "Ausfall eines DP-Masters bzw. eines DP-Mastersystems";
                    _ob = 70;
                    break;
                case (0x72A3):
                    Message = "Redundanzwiederkehr am DP-Slave";
                    _ob = 70;
                    break;
                case (0x73A3):
                    Message = "Redundanzverlust am DP-Slave";
                    _ob = 70;
                    break;
                case (0x7301):
                    Message = "Redundanzverlust (1v2) durch Ausfall einer CPU";
                    _ob = 72;
                    break;
                case (0x7302):
                    Message = "Redundanzverlust (1v2) durch STOP der Reserve, der vom Anwender ausgelöst wurde";
                    _ob = 72;
                    break;
                case (0x7303):
                    Message = "H-System (1v2 ) in den redundanten Betrieb gegangen";
                    _ob = 72;
                    break;
                case (0x7320):
                    Message = "Fehler bei RAM-Vergleich";
                    _ob = 72;
                    break;
                case (0x7321):
                    Message = "Fehler beim Vergleich von Prozeßabbild-Ausgangswert";
                    _ob = 72;
                    break;
                case (0x7322):
                    Message = "Fehler beim Vergleich von Merkern, Zeiten oder Zählern";
                    _ob = 72;
                    break;
                case (0x7323):
                    Message = "Unterschiedliche Betriebssystemdaten erkannt";
                    _ob = 72;
                    break;
                case (0x7331):
                    Message = "Reserve-Master-Umschaltung wegen Masterausfall";
                    _ob = 72;
                    break;
                case (0x7333):
                    Message = "Reserve-Master-Umschaltung im Rahmen einer Anlagenänderung im laufenden Betrieb";
                    _ob = 72;
                    break;
                case (0x7334):
                    Message = "Reserve-Master-Umschaltung wegen Verbindungsstörung am Synchronisationsmodul";
                    _ob = 72;
                    break;
                case (0x7340):
                    Message = "Synchronisationsfehler im Anwenderprogramm durch abgelaufene Wartezeit";
                    _ob = 72;
                    break;
                case (0x7341):
                    Message = "Synchronisationsfehler im Anwenderprogramm durch Warten an unterschiedlichen Synchronisationspunkten";
                    _ob = 72;
                    break;
                case (0x7342):
                    Message = "Synchronisationsfehler im Betriebssystem durch Warten an unterschiedlichen Synchronisationspunkten";
                    _ob = 72;
                    break;
                case (0x7343):
                    Message = "Synchronisationsfehler im Betriebssystem durch abgelaufene Wartezeit";
                    _ob = 72;
                    break;
                case (0x7344):
                    Message = "Synchronisationsfehler im Betriebssystem durch falsche Daten";
                    _ob = 72;
                    break;
                case (0x7950):
                    Message = "Synchronisationsmodul fehlt";
                    _ob = 72;
                    break;
                case (0x7951):
                    Message = "Änderung am Synchronisationsmodul ohne NETZEIN";
                    _ob = 72;
                    break;
                case (0x7952):
                case (0x7852):
                    Message = "Synchronisation-Modul gezogen/gesteckt";
                    _ob = 72;
                    break;
                case (0x7953):
                    Message = "Änderung am Synchronisationsmodul ohne Urlöschen";
                    _ob = 72;
                    break;
                case (0x7954):
                    Message = "Synchronisationsmodul: Doppelvergabe einer Baugruppenträgernummer";
                    _ob = 72;
                    break;
                case (0x7955):
                case (0x7855):
                    Message = "Synchronisationsmodul-Fehler/ beseitigt";
                    _ob = 72;
                    break;
                case (0x7956):
                    Message = "Unzulässige Baugruppenträger-Nr. auf Synchronisationsmodul eingestellt";
                    _ob = 72;
                    break;
                case (0x7960):
                    Message = "Redundante Peripherie: Diskrepanzzeit bei Digitaleingang abgelaufen, Fehler noch nicht lokalisiert";
                    break;
                case (0x7961):
                    Message = "Redundante Peripherie, Digitaleingabe-Fehler: Signalwechsel nach Ablauf der Diskrepanzzeit";
                    break;
                case (0x7962):
                    Message = "Redundante Peripherie: Digitaleingabe-Fehler -";
                    break;
                case (0x796F):
                    Message = "Redundante Peripherie: Gesamtdepassivierung der Peripherie durchgeführt";
                    break;
                case (0x7970):
                    Message = "Redundante Peripherie: Digitalausgabe-Fehler -";
                    break;
                case (0x7980):
                    Message = "Redundante Peripherie: Diskrepanzzeit bei Analogeingang abgelaufen";
                    break;
                case (0x7981):
                    Message = "Redundante Peripherie: Analogeingabe-Fehler -";
                    break;
                case (0x7990):
                    Message = "Redundante Peripherie: Analogausgabe-Fehler -";
                    break;
                case (0x73C1):
                    Message = "Ankoppeln/Aufdaten wurde abgebrochen";
                    _ob = 72;
                    break;
                case (0x73C2):
                    Message = "Abbruch des Aufdatvorgangs wegen Überschreiten einer Überwachungszeit beim n-ten Versuch (1 ≤ n ≤ max. mögliche Anzahl der Aufdatversuche nach Abbruch durch Zeitüberschreitung)";
                    _ob = 72;
                    break;
                case (0x75D1):
                    Message = "Sicherheitsprogramm: Interner CPU-Fehler -";
                    break;
                case (0x75D2):
                    Message = "Fehler im Sicherheitsprogramm: Zykluszeitüberschreitung -";
                    break;
                case (0x79D3):
                case (0x78D3):
                    Message = "Fehler bei PROFIsafe-Kommunikation mit F-Peripherie -";
                    break;
                case (0x79D4):
                case (0x78D4):
                    Message = "Fehler bei sicherheitsgerichteter Kommunikation zwischen F-CPUs -";
                    break;
                case (0x79D5):
                case (0x78D5):
                    Message = "Fehler bei sicherheitsgerichteter Kommunikation zwischen F-CPUs -";
                    break;
                case (0x75D6):
                    Message = "Datenverfälschung im Sicherheitsprogramm vor Ausgabe an die F-Peripherie";
                    break;
                case (0x75D7):
                    Message = "Datenverfälschung im Sicherheitsprogramm vor Ausgabe an Partner-F-CPU";
                    break;
                case (0x73D8):
                    Message = "Sicherheitsbetrieb deaktiviert -";
                    break;
                case (0x75D9):
                    Message = "Ungültige REAL-Zahl in einem DB -";
                    break;
                case (0x75DA):
                    Message = "Sicherheitsprogramm: Fehler im Sicherheitsdatenformat -";
                    break;
                case (0x73DB):
                case (0x72DB):
                    Message = "Sicherheitsprogramm: Sicherheitsbetrieb aktiv/deaktiviert -";
                    break;
                case (0x75DC):
                    Message = "Ablaufgruppe, interner Protokollfehler -";
                    break;
                case (0x75DD):
                case (0x74DD):
                    Message = "Sicherheitsprogramm: Abschaltung einer fehlersicheren Ablaufgruppe aktiv/deaktiviert";
                    break;
                case (0x75DE):
                case (0x74DE):
                    Message = "Sicherheitsprogramm: Komplette Abschaltung des F-Programms aktiv/deaktiviert";
                    break;
                case (0x75DF):
                case (0x74DF):
                    Message = "Initialisierung F-Programm Beginn/Ende -";
                    break;
                case (0x75E1):
                    Message = "Sicherheitsprogramm: Fehler im FB \"F_PLK\" oder \"F_PLK_O\" oder \"F_CYC_CO\" oder \"F_TEST\" oder \"F_TESTC\"";
                    break;
                case (0x75E2):
                    Message = "Sicherheitsprogramm: Bereichslängenfehler -";
                    break;
                case (0x79E3):
                    Message = "F-Peripherie-Eingangskanal passiviert -";
                    break;
                case (0x78E3):
                    Message = "F-Peripherie-Eingangskanal depassiviert -";
                    break;
                case (0x79E4):
                    Message = "F-Peripherie-Ausgangskanal passiviert -";
                    break;
                case (0x78E4):
                    Message = "F-Peripherie-Ausgangskanal depassiviert -";
                    break;
                case (0x79E5):
                    Message = "F-Peripherie passiviert -";
                    break;
                case (0x78E5):
                    Message = "F-Peripherie depassiviert -";
                    break;
                case (0x79E6):
                    Message = "Inkonsistentes Sicherheitsprogramm -";
                    break;
                case (0x79E7):
                    Message = "Simulationsbaustein (F-Systembaustein) geladen -";
                    break;
            }

        }

        private int _ob;
        private int _id;

        private byte[] _extInfo;

        private DateTime _zeit;

        public override string ToString()
        {
            return "ID:" + _id.ToString() + " " + Message;
        }
    }
}
