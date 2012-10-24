using System;
using System.Collections;
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
    internal class TimeTriggerWithCheckForChangesThread:IDisposable
    {
        private IDBInterface dbInterface;
        private DatasetConfig datasetConfig;
        private Dictionary<ConnectionConfig, Object> activConnections;

        public event ThreadExceptionEventHandler ThreadExceptionOccured;

        private bool StartedAsService;

        private TimeSpan TriggerTimeSpan = new TimeSpan(0, 0, 1);
        
        private Thread myThread = null;

        public TimeTriggerWithCheckForChangesThread(IDBInterface dbInterface, DatasetConfig datasetConfig, Dictionary<ConnectionConfig, Object> activConnections, bool StartedAsService)
        {
            this.StartedAsService = StartedAsService;
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

        private List<object> oldValues = new List<object>();
        private void WaitForTrigger()
        {
            try
            {
                while (true)
                {
                    IEnumerable<object> values = ReadData.ReadDataFromDataSources(datasetConfig, datasetConfig.DatasetConfigRows, activConnections, StartedAsService);


                    if (values != null)
                    {
                        bool write = false;
                        if (oldValues != null && oldValues.Count > 0)
                        {
                            int i = 0;
                            foreach (object val in values)
                            {
                                if (val is IStructuralEquatable)
                                { 
                                    if (!((IStructuralEquatable)val).Equals( oldValues[i], StructuralComparisons.StructuralEqualityComparer))
                                        write = true;
                                }
                                else if (!val.Equals(oldValues[i]))
                                    write = true;
                                i++;

                            }
                        }
                        else
                            write = true;

                        if (write)
                            dbInterface.Write(values);
                        
                        oldValues.Clear();
                        oldValues.AddRange(values);
                        
                    }

                    Thread.Sleep(TriggerTimeSpan);
                }
            }
            catch (ThreadAbortException ex)
            {
            }
            catch (Exception ex)
            {
                if (StartedAsService)
                    ThreadExceptionOccured.Invoke(this, new ThreadExceptionEventArgs(ex));
                else
                    throw;
            }
        }

        public void Dispose()
        {
            if (myThread != null)
                myThread.Abort();
        }
    }
}
