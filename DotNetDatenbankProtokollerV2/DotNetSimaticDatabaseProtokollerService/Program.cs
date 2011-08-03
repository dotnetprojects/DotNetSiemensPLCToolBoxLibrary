using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace DotNetSimaticDatabaseProtokollerService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] CommandLineArgs)
        {
            //Komandozeilenargumente eingegeben???		
            if (CommandLineArgs.Length > 0)
            {

                if (CommandLineArgs[0] == "/install")
                {
                    //Service hinzufügen

                    TransactedInstaller serviceInstaller;
                    Hashtable stateSaver;
                    InstallContext installContext;
                    ProjectInstaller mi;

                    serviceInstaller = new TransactedInstaller();
                    stateSaver = new Hashtable();

                    mi = new ProjectInstaller();
                    serviceInstaller.Installers.Add(mi);

                    string prgPfad = "\"" + Assembly.GetExecutingAssembly().Location + "\" " + "/service";
                    installContext = new InstallContext("ServiceInstall.log", null);
                    installContext.Parameters.Add("assemblyPath", prgPfad);

                    foreach (string myStr in CommandLineArgs)
                    {
                        if (myStr == "/postgres")
                            mi.postgres = true;
                        else if (myStr == "/mysql")
                            mi.mysql = true;
                        else if (myStr == "/mssql")
                            mi.mssql = true;
                    }

                    serviceInstaller.Context = installContext;
                    try
                    {
                        serviceInstaller.Install(stateSaver);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fehler bein installieren des Service: " + ex.ToString());
                    }
                }
                //serviceInstaller.Commit(stateSaver)

                else if (CommandLineArgs[0] == "/uninstall")
                {
                    //Service löschen...

                    TransactedInstaller serviceInstaller;
                    Hashtable stateSaver;
                    InstallContext installContext;
                    ProjectInstaller mi;

                    serviceInstaller = new TransactedInstaller();
                    stateSaver = new Hashtable();

                    mi = new ProjectInstaller();
                    serviceInstaller.Installers.Add(mi);

                    string prgPfad = "\"" + Assembly.GetExecutingAssembly().Location + "\" \"" + "/service\"";
                    installContext = new InstallContext("ServiceInstall.log", null);
                    installContext.Parameters.Add("assemblyPath", prgPfad);

                    serviceInstaller.Context = installContext;
                    try
                    {
                        serviceInstaller.Uninstall(null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fehler bein deinstallieren des Service: " + ex.ToString());
                    }

                }
                else if (CommandLineArgs[0] == "/service")
                {
                    ProtokollerDatenbankService myProt = new ProtokollerDatenbankService();
                    ServiceBase.Run(new ServiceBase[] { myProt });
                }
            }

            //Keine Komandozeilenargumente....			
            else
            {

                //Zuerst versuchen den Service zu starten....
                //ProtkollerDatenbank myProt = new ProtkollerDatenbank();
                //ServiceBase.Run(new ServiceBase[] {myProt});

                //Wenn starten nicht möglich war, dann war es doppelklick auf die Datei
                //daher Config anzeigen....
                //if (myProt.ServiceRunning == false)
                {
                    Application.Run(new ServiceConfig());
                }

            }
        }
    }
}
