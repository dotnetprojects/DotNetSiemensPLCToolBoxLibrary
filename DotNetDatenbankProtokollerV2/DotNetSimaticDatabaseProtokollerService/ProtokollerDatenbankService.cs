using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
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
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


                ProtokollerConfiguration.Load();
                myInstance = new ProtokollerInstance(ProtokollerConfiguration.ActualConfigInstance);
                myInstance.Start(true);
            }
            catch (Exception ex)
            {
                Logging.LogText("Exception occured! ",ex, Logging.LogLevel.Error);
            }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logging.LogText("Exception occured! ", e.ExceptionObject as Exception, Logging.LogLevel.Error);
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logging.LogText("Exception occured! ", e.Exception, Logging.LogLevel.Error);
        }

        protected override void OnStop()
        {
            myInstance.Dispose();
            myInstance = null;
        }
    }
}
