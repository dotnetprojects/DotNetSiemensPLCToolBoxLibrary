// Lizenzsinformationen Protkoller Datenbank .Net
//
// Copyright: Jochen Kühner - Computerdienstleistungen (jochen.kuehner@gmx.de)
//
// Dieses Programm darf kostenlos für nicht gewerbliche Zwecke verwendet werden.
// Für nutzung in Gewerblichen Programmen, bitte Kontakt zu dem Autor aufnehmen.
//
// Dieses Programm ist Open Source, es darf nicht als Binary weitergegeben werden.
// Änderungen am Quelltext des Programmes sind dem Autor mitzuteilen!
//
// Dieses Programm nutzt folgende Externe Bibliotheken: 
//  - LibNoDave zur SPS Anbindung
//  - MySQL.Net Connector zur Kommunikation mit einer MySQL Datenbank
//  - Npgsql zur Kommunikation mit einer Postgres Datenbank
//  - Mono.Security für Npgsql 

//
// Erstellt mit SharpDevelop.
// Benutzer: Jochen Kuehner PC
// Datum: 16.01.2007
// Zeit: 10:31
// 
// Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
//

using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using DotNetSimaticDatabaseProtokollerLibrary;

namespace DotNetSimaticDatabaseProtokollerService
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {

        private ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
        private ServiceInstaller serviceInstaller = new ServiceInstaller();

        public bool postgres = false;
        public bool mysql = false;
        public bool mssql = false;

        public ProjectInstaller()
        {
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            serviceInstaller.ServiceName = StaticServiceConfig.MyServiceName;
            serviceInstaller.Description = "DotNetSimaticDatabaseProtokollerService for Logging PLC-Data to Databases";

            var DependsOn = new System.Collections.Generic.List<string>();

            if (postgres)
                DependsOn.Add("postgresql-8.3");
            if (mysql)
                DependsOn.Add("MySQL");
            if (mssql)
                DependsOn.Add("MSSQLSERVER");


            serviceInstaller.ServicesDependedOn = DependsOn.ToArray();

            this.Installers.AddRange(new Installer[] {serviceProcessInstaller, serviceInstaller});
        }
    }
}

