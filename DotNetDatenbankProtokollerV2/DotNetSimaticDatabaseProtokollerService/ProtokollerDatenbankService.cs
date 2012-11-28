using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using DotNetSimaticDatabaseProtokollerLibrary;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Protocolling;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;

namespace DotNetSimaticDatabaseProtokollerService
{
    public partial class ProtokollerDatenbankService : ServiceBase
    {
        private ProtokollerInstance myInstance = null;

        public ProtokollerDatenbankService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                ProtokollerConfiguration.Load();
                myInstance = new ProtokollerInstance(ProtokollerConfiguration.ActualConfigInstance);
                myInstance.Start(true);
            }
            catch (Exception ex)
            {
                Logging.LogText("Exception occured! ",ex, Logging.LogLevel.Error);
            }
        }

        protected override void OnStop()
        {
            myInstance.Dispose();
            myInstance = null;
        }
    }
}
