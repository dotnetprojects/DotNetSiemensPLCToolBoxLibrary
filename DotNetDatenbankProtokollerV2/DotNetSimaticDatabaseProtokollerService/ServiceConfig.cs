using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotNetSimaticDatabaseProtokollerLibrary;

namespace DotNetSimaticDatabaseProtokollerService
{
    public partial class ServiceConfig : Form
    {
        public ServiceConfig()
        {
            InitializeComponent();
        }

        private void cmdServiceStart_Click(object sender, EventArgs e)
        {
            try
            {
                System.ServiceProcess.ServiceController dienst = new System.ServiceProcess.ServiceController(StaticServiceConfig.MyServiceName);
                dienst.Start();
            }
            catch (Exception)
            { }
        }

        private void cmdServiceStop_Click(object sender, EventArgs e)
        {
            try
            {
                System.ServiceProcess.ServiceController dienst = new System.ServiceProcess.ServiceController(StaticServiceConfig.MyServiceName);
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
            if (chkPostgres.Checked)
                myProcess.StartInfo.Arguments += " /postgres";
            if (chkMysql.Checked)
                myProcess.StartInfo.Arguments += " /mysql";
            if (chkMssql.Checked)
                myProcess.StartInfo.Arguments += " /mssql";
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
            System.ServiceProcess.ServiceController dienst = new System.ServiceProcess.ServiceController(StaticServiceConfig.MyServiceName);
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
    }
}
