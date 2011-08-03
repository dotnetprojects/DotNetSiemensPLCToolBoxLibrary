using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;

namespace DotNetSimaticDatabaseProtokollerLibrary.Protocolling.Trigger
{
    class TimeTriggerThread:IDisposable
    {
        private IDBInterface dbInterface;
        private DatasetConfig datasetConfig;
        private Dictionary<ConnectionConfig, Object> activConnections;


        private TimeSpan TriggerTimeSpan = new TimeSpan(0, 0, 1);
        
        private Thread myThread = null;

        public TimeTriggerThread(IDBInterface dbInterface, DatasetConfig datasetConfig, Dictionary<ConnectionConfig, Object> activConnections)
        {
            this.dbInterface = dbInterface;
            this.datasetConfig = datasetConfig;
            this.activConnections = activConnections;

            this.TriggerTimeSpan = datasetConfig.TriggerTimeSpan;
        }

        public void StartTrigger()
        {
            myThread = new Thread(new ThreadStart(WaitForTrigger)) {Name = "TimeTriggerThread, DataSetConfig:" + datasetConfig.Name};
            myThread.Start();
        }


        private void WaitForTrigger()
        {
            try
            {
                while (true)
                {
                    IEnumerable<object> values = ReadData.ReadDataFromPLCs(datasetConfig.DatasetConfigRows, activConnections);
                    if (values != null)
                        dbInterface.Write(values);

                    Thread.Sleep(TriggerTimeSpan);
                }
            }
            catch (ThreadAbortException ex)
            {
            }
        }

        public void Dispose()
        {
            if (myThread != null)
                myThread.Abort();
        }
    }
}
