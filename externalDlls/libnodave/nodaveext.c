#include "nodave.h" 
char * DECL2 daveStrerrorExt(int code) {
    switch (code) {
	case 0x8001: return "Der angeforderte Dienst kann im derzeitigen Zustand der Baugruppe nicht ausgefuehrt werden. Weitere Bausteinfunktionen sind deshalb nicht moeglich.";
	case 0x8003: return "S7-Protokollfehler. Es ist ein Fehler beim Uebertragen des Bausteins aufgetreten.";
	
	case 0x8101: return "Applikation, allgemeiner Fehler: Dienst ist der remoten Baugruppe unbekannt.";
	case 0x8104: return "Der Dienst ist auf der Baugruppe nicht implementiert oder es wird ein Telegrammfehler gemeldet.";
	
	case 0x8204: return "Die Typangabe zum Objekt ist inkonsistent.";
	case 0x8205: return "Ein kopierter Baustein ist bereits vorhanden und nicht eingekettet.";
	
	case 0x8301: return "Der Speicherplatz oder der Arbeitsspeicher auf der Baugruppe ist nicht ausreichend bzw. das angegebene Speichermedium ist nicht erreichbar.";
	case 0x8302: return "Es liegt ein Ressourcen-Engpass vor bzw. die Prozessor-Ressourcen sind nicht verfuegbar.";
	case 0x8304: return "Es ist kein weiterer Hochladevorgang mehr moeglich. Es liegt ein Ressourcen-Engpass vor.";
	case 0x8305: return "Die Funktionalitaet ist nicht verfuegbar.";
	case 0x8306: return "Arbeitsspeicher nicht ausreichend (bai kopieren, Einketten, AWP laden).";
	case 0x8307: return "Remanenter Teil des Arbeitsspeichers nicht ausreichend (bai kopieren, Einketten, AWP laden).";
	
	case 0x8401: return "S7-Protokollfehler: Falsche Reihenfolge der Dienste (z.B. beim Laden oder Hochladen eines Bausteins).";
	case 0x8402: return "Dienst kann wegen des Zustands des adressierten Objekts nicht ausgefuehrt werden.";	
	case 0x8404: return "S7-Protokoll: Die Funktion kann nicht ausgefuehrt werden.";
	case 0x8405: return "Der remote Baustein ist im Zustand DISABLE (PBK).Die Funktion kann nicht ausgefuehrt werden.";
	
	case 0x8500: return "S7-Protokollfehler: Falsche Telegramme.";
	case 0x8503: return "Meldung von der Baugruppe: Der Dienst wurde vorzeitig abgebrochen.";
	
	case 0x8701: return "Fehler bei der Adressierung des Objekts im Kommunikationspartner(z.B. Bereichslaengenfehler).";
	case 0x8702: return "Der angeforderte Dienst wird von der Baugruppe nicht unterstuetzt.";
	case 0x8703: return "Der Zugriff auf das Objekt wurde abgelehnt.";
	case 0x8704: return "Zugriffsfehler: Das Objekt ist zerstoert.";
	
	case 0xd001: return "Protokollfehler: Die Auftragsnummer ist unzulaessig.";
	case 0xd002: return "Parameterfehler: Die Auftragsvariante ist unzulaessig.";
	case 0xd003: return "Parameterfehler: Die Testfunktion wird von der Baugruppe nicht unterstuetzt.";
	case 0xd004: return "Parameterfehler: Der Auftragsstatus ist unzulaessig.";
	case 0xd005: return "Parameterfehler: Die Auftragsbeendung ist unzulaessig.";
	case 0xd006: return "Parameterfehler: Die Kennung fuer den Verbindungsabbruch ist unzulaessig.";
	case 0xd007: return "Parameterfehler: Die Anzahl der Pufferelemente ist unzulaessig.";
	case 0xd008: return "Parameterfehler: Der Untersetzungsfaktor ist unzulaessig.";
	case 0xd009: return "Parameterfehler: Die Ausfuehrungsanzahl ist unzulaessig.";
	case 0xd00a: return "Parameterfehler: Das Triggerereignis ist unzulaessig.Bitte pruefen Sie, ob der angegebene Trigger auf dieser Baugruppe zulaessig ist.";
	case 0xd00b: return "Parameterfehler: Das Triggerbedingung ist unzulaessig.Bitte pruefen Sie, ob der angegebene Trigger auf dieser Baugruppe zulaessig ist.";
	
	case 0xd011: return "Parameterfehler im Pfad der Aufrufumgebung: Der Baustein ist nicht vorhanden.";
	case 0xd012: return "Parameterfehler: Falsche Adresse im Baustein.";
	case 0xd014: return "Parameterfehler: Der Baustein wird gerade geloescht/ueberladen.";
	case 0xd015: return "Parameterfehler: Die Variablenadresse ist unzulaessig.";
	case 0xd016: return "Parameterfehler: Testauftraege nicht moeglich, da Anwenderprogramm fehlerhaft.";
	case 0xd017: return "Parameterfehler: Die Triggernummer ist unzulaessig.";
	
	case 0xd025: return "Parameterfehler: Der Pfad ist unzulaessig.";
	case 0xd026: return "Parameterfehler: Die Zugriffsart ist unzulaessig.";
	case 0xd027: return "Parameterfehler: Die Anzahl der DBs ist unzulaessig.";
	
	case 0xd031: return "interner Protokollfehler.";
	case 0xd032: return "Parameterfehler: Falsche Laenge des Ergebnispuffers.";
	case 0xd033: return "Parameterfehler: Die Auftragslaenge ist unzulaessig.";
	
	case 0xd03f: return "Kodierungsfehler: Fehler im Parameterteil (z.B. Reservebytes ungleich NULL).";

	case 0xd041: return "Datenfehler: Die Kennung der Statusliste ist unzulaessig.";
	case 0xd042: return "Datenfehler: Die Variablenadresse ist unzulaessig.";
	case 0xd044: return "Datenfehler: Der Variablenwert ist unzulaessig.Ueberpruefen Sie die Auftragsdaten.";
	case 0xd045: return "Datenfehler: Das Verlassen der ODIS-Steuerung bei HALT ist unzulaessig.";
	case 0xd046: return "Datenfehler: Die Messtufe bei Laufzeitmessung ist unzulaessig.";
	case 0xd047: return "Datenfehler: Die Hierarchie bei 'Auftragsliste lesen' ist unzulaessig.";
	case 0xd048: return "Datenfehler: Die Loeschkennung bei 'Auftrag loeschen' ist unzulaessig.";
	case 0xd049: return "Datenfehler: Die Ersetzkennung bei 'Auftrag ersetzen' ist unzulaessig.";
	case 0xd04a: return "Fehler bei der Ausfuehrung von 'Programmstatus'.";
	
	case 0xd05f: return "Kodierungsfehler: Fehler im Datenteil (z.B. Reservebytes ungleich NULL,...).";
	
	case 0xd061: return "Ressourcenfehler: Es ist kein Speicherplatz fuer den Auftrag vorhanden.";
	case 0xd062: return "Ressourcenfehler: Die Auftragsliste ist voll.";
	case 0xd063: return "Ressourcenfehler: Das Triggerereignis ist belegt.";
	case 0xd064: return "Ressourcenfehler: Der Speicherplatz fuer ein Element des Ereignispuffers ist zu klein.";
	case 0xd065: return "Ressourcenfehler: Der Speicherplatz fuer mehrere Elemente des Ereignispuffers ist zu klein.";
	case 0xd066: return "Ressourcenfehler: Der Timer fuer die Laufzeitmessung ist durch einen anderen Auftrag belegt.";
	case 0xd067: return "Ressourcenfehler: Es sind zu viele 'Steuern Variable'-Auftraege aktiv (insbesondere Mehrprozessorbetrieb).";
	
	case 0xd081: return "Die Funktion ist im aktuellen Betriebszustand unzulaessig.";
	case 0xd082: return "Betriebszustandsfehler: Betriebszustand HALT kann nicht verlassen werden.";

	case 0xd0a2: return "Funktion ist momentan nicht moeglich, da eine speicherveraendernde Funktion laeuft.";
	case 0xd0a3: return "Es sind zu viele 'Steuern Variable'-Auftraege auf die Peripherie aktiv (insbesondere Mehrprozessorbetrieb).";    	
	case 0xd0a4: return "'Forcen' ist schon eingerichtet.";    	
	case 0xd0a5: return "Der referenzierte Auftrag ist nicht vorhanden.";    	
	case 0xd0a6: return "Der Auftrag kann nicht gesperrt/freigegeben werden.";    	
	case 0xd0a7: return "Der Auftrag kann nicht geloescht werden, da dieser z.B. gerade gelesen wird. Versuchen Sie es spaeter noch einmal.";
	case 0xd0a8: return "Der Auftrag kann nicht ersetzt werden, da dieser z.B. gerade gelesen oder geloescht wird. Versuchen Sie es spaeter noch einmal.";
	case 0xd0a9: return "Der Auftrag kann nicht gelesen werden, da dieser z.B. gerade geloescht wird. Versuchen Sie es spaeter noch einmal.";
	case 0xd0aa: return "Das Zeitlimit im Prozessbetrieb ist ueberschritten.";
	case 0xd0ab: return "Die Auftragsparameter sind im Prozessbetrieb unzulaessig.";
	case 0xd0ac: return "Die Auftragsdaten sind im Prozessbetrieb unzulaessig.";
	case 0xd0ad: return "Der Betriebsmodus ist schon eingestellt.";
	case 0xd0ae: return "Der Auftrag wurde ueber eine andere Verbindung eingerichtet und kann nur ueber diese hantiert werden.";
	
	case 0xd0c1: return "Beim Zugriff auf die Variable(n) wurde mindestens ein Fehler erkannt.";
	case 0xd0c2: return "Betriebszustandsuebergang in STOP/HALT.";
	case 0xd0c3: return "Beim Zugriff auf die Variable(n) wurde mindestens ein Fehler erkannt und Betriebszustandsuebergang in STOP/HALT.";
	case 0xd0c4: return "Timerueberlauf bei der die Laufzeitmessung";
	case 0xd0c5: return "Die Ausgabe des Bausteinstacks ist inkonsistent, da Bausteine geloescht/nachgeladen wurden.";
	case 0xd0c6: return "Der Auftrag wurde automatisch geloescht, weil alle in ihm referenzierten Auftraege geloescht wurden.";
	case 0xd0c7: return "Der Auftrag wurde automatisch geloescht, wegen Verlassens des Betriebszustands STOP.";
	case 0xd0c8: return "'Status Baustein' wurde abgebrochen wegen Inkonsistenz zwischen Testauftrag und Programm.";
	case 0xd0c9: return "Verlassen des Statusbereichs durch Ruecksetzen des OB90.";
	case 0xd0ca: return "Verlassen des Statusbereichs durch Ruecksetzen des OB90 und Zugriffsfehler beim Lesen der Variablen vor dem Verlassen.";
	case 0xd0cb: return "Die Ausgabesperre der Peripherie ist wieder eingeschaltet.";
	case 0xd0cc: return "Der Datenumfang fuer die Testfunktion ist durch das Zeitlimit eingeschraenkt.";
	
	
	case 0xd231: return "Mindestens ein geladener OB kann nicht kopiert werden, da die zugehoerige Ablaufebene nicht existiert.";
	case 0xd232: return "Mindestens eine Bausteinnummer eines geladenen Bausteins ist unzulaessig.";
	case 0xd234: return "Der Baustein ist im angegebenen Speichermedium oder im Auftrag doppelt vorhanden.";
	case 0xd235: return "Der Baustein enthaelt eine fehlerhafte Pruefsumme.";
	case 0xd236: return "Der Baustein enthaelt keine Pruefsumme.";
	case 0xd237: return "Der Baustein soll doppelt geladen werde, d.h. es existiert schon derselbe Baustein mit gleichem Zeitstempel auf der CPU.";
	case 0xd238: return "Mindestens ein angegebener Baustein ist kein DB.";
	case 0xd239: return "Mindestens ein angegebener DB ist nicht als eingekettete Variante im Ladespeicher vorhanden.";
	case 0xd23a: return "Mindestens ein angegebener DB hat wesentliche Unterschiede zwischen kopierter und eingeketteter Variante.";
	
	case 0xd240: return "Die Koordinierungsregeln wurden verletzt.";
	case 0xd241: return "Die Funktion ist in der aktuellen Schutzstufe nicht erlaubt.";
	case 0xd242: return "Schutzverletzung bei der Bearbeitung von F-Bausteinen. F-Bausteine knnen nur nach Eingabe es Passwortes bearbeitet werden. Der F-Baustein SDB99 kann nicht geloescht werden.";

	case 0xd250: return "Die Update- und Baugruppenkennung oder der Ausgabestand stimmen nicht ueberein."; 
	case 0xd251: return "Falsche Reihenfolge der Betriebssystem-Komponenten."; 
	case 0xd252: return "Pruefsummenfehler.";
	case 0xd253: return "Es ist kein ablauffaehiger Lader vorhanden; eine Aktualisierung ist nur ueber die Memory Card moeglich.";
	case 0xd254: return "Speicherfehler im Betriebssystem.";
	
	case 0xd280: return "Fehler bei der Uebersetzung eines Bausteins in S7-300 CPU.";
	
	case 0xd401: return "Auskunftsfunktion nicht verfuegbar.";
	case 0xd402: return "Auskunftsfunktion nicht verfuegbar.";
	case 0xd403: return "Der Dienst ist bereits angemeldet/abgemeldet (Diagnose/PMC).";
	case 0xd404: return "Die maximale Teilnehmerzahl wurde erreicht. Keine weitere Anmeldung fuer Diagnose/PMC moeglich.";
	case 0xd405: return "Der Dienst wird nicht unterstuetzt oder Syntaxfehler bei den Funktionsparametern.";
	case 0xd406: return "Die gewnschte Information ist temporaer nicht vorhanden.";
	case 0xd407: return "Es ist ein Diagnose-Fehler aufgetreten.";	
	case 0xd408: return "Update abgebrochen.";	
	case 0xd409: return "Am DP-Bus ist ein Fehler aufgetreten.";	
	
	
	default: return daveStrerror(code);
	
    }
}
