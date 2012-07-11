using System;
using System.Windows.Forms;

namespace TCPForwarder
{
    public partial class ServiceConfig : Form
    {
        public ServiceConfig()
        {
            InitializeComponent();

            propertyGrid.SelectedObject = Settings.Default;
        }

        private void cmdServiceStart_Click(object sender, EventArgs e)
        {
            try
            {
                System.ServiceProcess.ServiceController dienst = new System.ServiceProcess.ServiceController(ProjectInstaller.ServiceName);
                dienst.Start();
            }
            catch (Exception)
            { }
        }

        private void cmdServiceStop_Click(object sender, EventArgs e)
        {
            try
            {
                System.ServiceProcess.ServiceController dienst = new System.ServiceProcess.ServiceController(ProjectInstaller.ServiceName);
                dienst.Stop();
            }
            catch (Exception)
            { }
        }

        private void cmdServiceInstall_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            myProcess.StartInfo.Arguments = "/install";
            myProcess.Start();
        }

        private void cmdServiceUninstall_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            myProcess.StartInfo.Arguments = "/uninstall";
            myProcess.Start();
        }

        private void servicezustand_Tick(object sender, EventArgs e)
        {
            System.ServiceProcess.ServiceController dienst = new System.ServiceProcess.ServiceController(ProjectInstaller.ServiceName);
            try
            {
                switch (dienst.Status)
                {
                    case System.ServiceProcess.ServiceControllerStatus.ContinuePending:
                        serviceState.Text = "Continue Pending";
                        break;
                    case System.ServiceProcess.ServiceControllerStatus.Paused:
                        serviceState.Text = "Paused";
                        break;
                    case System.ServiceProcess.ServiceControllerStatus.PausePending:
                        serviceState.Text = "Pause Pending";
                        break;
                    case System.ServiceProcess.ServiceControllerStatus.Running:
                        serviceState.Text = "Running";

                        break;
                    case System.ServiceProcess.ServiceControllerStatus.StartPending:
                        serviceState.Text = "Start Pending";
                        break;
                    case System.ServiceProcess.ServiceControllerStatus.Stopped:
                        serviceState.Text = "Stopped";
                        break;
                    case System.ServiceProcess.ServiceControllerStatus.StopPending:
                        serviceState.Text = "Stop Pending";
                        break;

                }
            }
            catch (InvalidOperationException)
            {
                serviceState.Text = "Dienst nicht installiert!";
            }
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ServiceConfig_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
