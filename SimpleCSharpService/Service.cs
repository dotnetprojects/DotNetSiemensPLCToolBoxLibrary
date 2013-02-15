using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

using DotNetSiemensPLCToolBoxLibrary.Communication;

namespace SimpleCSharpService
{
    //Installieren mit:
    //C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil /i SimpleCSharpService.exe
    public partial class Service : ServiceBase
    {

        public Service()
        {
            ServiceName = "SimpleCSharpService";

            InitializeComponent();
        }

        private Thread myThread;

        private volatile bool threadShouldRun;

        private PLCConnection myConn;

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            var cfg = new PLCConnectionConfiguration("myConnection", LibNodaveConnectionConfigurationType.ObjectSavedConfiguration);
            cfg.ConnectionType = (int) LibNodaveConnectionTypes.ISO_over_TCP;
            cfg.CpuIP = "192.168.1.185";
            cfg.CpuSlot = 2;

            myConn = new PLCConnection(cfg);
            myConn.Connect();
            
            threadShouldRun = true;

            myThread = new Thread(new ThreadStart(this.ThreadProc));
            myThread.Start();
            
                    
        }

        private void ThreadProc()
        {
            PLCTag tag = new PLCTag("MD0");
            object oldValue = null;

            while (threadShouldRun)
            {
                myConn.ReadValue(tag);

                if (oldValue != tag.Value)
                {
                    //Hier Code was bei SPS Wertänderung passieren soll!

                    StreamWriter myFile = new StreamWriter("c:\\test.txt", true);
                    myFile.Write(tag.Value + Environment.NewLine);
                    myFile.Close();

                }

                oldValue = tag.Value;
                Thread.Sleep(100);
            }            
        }

        protected override void OnStop()
        {
            base.OnStop();

            threadShouldRun = false;
        }
    }
}
