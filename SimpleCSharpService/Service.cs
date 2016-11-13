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
using DotNetSiemensPLCToolBoxLibrary.DataTypes;

namespace SimpleCSharpService
{
    //Installieren mit:
    //C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil /i SimpleCSharpService.exe
    public partial class Service : ServiceBase
    {

        private string triggerBit = "M0.0";
        private int writeDB = 20;
        private int writeCharArraySize = 100;
        private string csvFile = "c:\\aa.csv";

        public Service()
        {
            ServiceName = "SimpleCSharpService";

            InitializeComponent();
        }

        private Thread myThread;

        private volatile bool threadShouldRun;

        private PLCConnection myConn;

        public void Test(string[] args)
        {
            OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            var cfg = new PLCConnectionConfiguration("myConnection", LibNodaveConnectionConfigurationType.ObjectSavedConfiguration);
            cfg.ConnectionType = LibNodaveConnectionTypes.ISO_over_TCP;
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
            PLCTag tag = new PLCTag(triggerBit);

            while (threadShouldRun)
            {
                myConn.ReadValue(tag);

                if ((bool)tag.Value == true)
                {
                    PLCTag writeData = new PLCTag();
                    writeData.TagDataType = TagDataType.CharArray;
                    writeData.DataBlockNumber = writeDB;
                    writeData.ArraySize = writeCharArraySize;

                    string writeDataString = "";
                    using (StreamReader sr = new StreamReader(csvFile))
                    {
                        writeDataString = sr.ReadToEnd();                        
                    }
                    writeData.Controlvalue = writeDataString;

                    //Reset the Bit
                    tag.Controlvalue = false;

                    myConn.WriteValues(new[] { writeData, tag });
                }

                Thread.Sleep(20);
            }
        }


        protected override void OnStop()
        {
            base.OnStop();

            threadShouldRun = false;
        }
    }
}
