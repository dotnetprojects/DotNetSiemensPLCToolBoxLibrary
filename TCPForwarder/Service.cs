using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using JFKCommonLibrary.Networking;

namespace TCPForwarder
{
    public partial class Service : ServiceBase
    {
        
        private TCPFunctionsAsync tcp1 = null;
        private TCPFunctionsAsync tcp2 = null;

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            tcp1 = new TCPFunctionsAsync(null, IPAddress.Parse(Settings.Default.SourceIP), Settings.Default.SourcePort,
                                         Settings.Default.SourceActive);
            tcp2 = new TCPFunctionsAsync(null, IPAddress.Parse(Settings.Default.DestinationIP), Settings.Default.DestinationPort,
                                         Settings.Default.DestinationActive);

            tcp1.DataRecieved += (data, tcpClient) => tcp2.SendData(data);
            tcp2.DataRecieved += (data, tcpClient) => tcp1.SendData(data);

            tcp1.Start();
            tcp2.Start();
        }

        protected override void OnStop()
        {
            if (tcp1 != null)
                tcp1.Stop();
            if (tcp2 != null)
                tcp2.Stop();
        }
    }
}
