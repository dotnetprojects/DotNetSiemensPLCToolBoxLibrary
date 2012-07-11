using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace TCPForwarder
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /*static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new Service() 
			};
            ServiceBase.Run(ServicesToRun);
        }*/

        [STAThread]
        private static void Main(String[] CommandLineArgs)
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
                    Service service = new Service();
                    ServiceBase.Run(new ServiceBase[] {service});
                }
            }

                //Keine Komandozeilenargumente....			
            else
            {
                {
                    Application.Run(new ServiceConfig());
                }

            }

        }
    }
}
