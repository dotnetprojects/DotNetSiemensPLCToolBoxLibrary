using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;

namespace DotNetSimaticDatabaseProtokollerLibrary.Protocolling.Trigger
{
    internal class PLCTagTriggerThread : IDisposable
    {
        private PLCConnection triggerConn;
        private PLCTag readBit;
        private PLCTag quittBit;

        private IDBInterface dbInterface;
        private DatasetConfig datasetConfig;
        private Dictionary<ConnectionConfig, Object> activConnections;

        public event ThreadExceptionEventHandler ThreadExceptionOccured;

        private bool StartedAsService;

        // Leseintervall von der SPS wenn neue Daten vorhanden waren.        
        private int NewDataInterval = 5;

        // Anzahl der Lesezyklen ohne Daten bis auf das NoDataIntervall umgeschaltet wird.
        private int NoDataCycles = 10;

        // Leseintervall von der SPS wenn NoDataCycles lang keine neuen Daten vorhanden waren.
        private int NoDataInterval = 300;

        private Thread myThread = null;

        public PLCTagTriggerThread(IDBInterface dbInterface, DatasetConfig datasetConfig, Dictionary<ConnectionConfig, Object> activConnections, bool StartedAsService)
        {
            this.StartedAsService = StartedAsService;
            this.dbInterface = dbInterface;
            this.datasetConfig = datasetConfig;
            this.activConnections = activConnections;

            this.triggerConn = (PLCConnection) activConnections[datasetConfig.TriggerConnection];
            this.readBit = datasetConfig.TriggerReadBit;
            this.quittBit = datasetConfig.TriggerQuittBit;

            ak_interval = NoDataInterval;
        }

        public void StartTrigger()
        {            
            myThread = new Thread(new ThreadStart(WaitForTrigger)) {Name = "PLCTagTriggerThread, DataSetConfig:" + datasetConfig.Name};
            myThread.Start();
        }

        private int cycle_counter = 0;
        private int ak_interval;

        private void WaitForTrigger()
        {
            try
            {
                bool alreadyWritten = false;
                while (true)
                {
                    if (triggerConn.Connected)
                    {
                        try
                        {
                            //Read the Trigger Bit
                            triggerConn.ReadValue(readBit);
                        }
                        catch(Exception ex)
                        {
                            if (StartedAsService)
                                Logging.LogText("Error: Exception during ReadData, maybe Connection interupted?", ex, Logging.LogLevel.Error);
                            else
                                throw;
                        }
                        //If the cycle counter is 0, switch to the slower interval (it means that no new data was there for a long time! ;-)
                        if (cycle_counter > 0)
                        {
                            cycle_counter--;
                            ak_interval = NoDataInterval;
                        }

                        if ((bool) readBit.Value & !alreadyWritten)
                        {
                            alreadyWritten = true;
                            cycle_counter = NoDataCycles;
                            ak_interval = NewDataInterval;

                            IEnumerable<object> values = ReadData.ReadDataFromDataSources(datasetConfig, datasetConfig.DatasetConfigRows, activConnections, StartedAsService);
                            if (values != null)
                            {
                                dbInterface.Write(values);

                                quittBit.Value = true;
                                try
                                {
                                    triggerConn.WriteValue(quittBit);
                                }
                                catch (Exception ex)
                                {
                                    if (StartedAsService)
                                        Logging.LogText("Error: Exception during WriteValue, maybe Connection interupted?", ex, Logging.LogLevel.Error);
                                    else
                                        throw;
                                }
                            }
                        }
                        else if (!(bool) readBit.Value)
                        {
                            if (alreadyWritten)
                            {
                                alreadyWritten = false;
                                quittBit.Value = false;
                                try
                                {
                                    triggerConn.WriteValue(quittBit);
                                }
                                catch (Exception ex)
                                {
                                    if (StartedAsService)
                                        Logging.LogText("Error: Exception during WriteValue, maybe Connection interupted?", ex, Logging.LogLevel.Error);
                                    else
                                        throw;
                                }
                            }
                        }
                    }

                    Thread.Sleep(ak_interval);
                }
            }
            catch (ThreadAbortException ex)
            {
                //ThreadExceptionOccured.Invoke(this, new ThreadExceptionEventArgs(ex));
            }
            catch (Exception ex)
            {
                ThreadExceptionOccured.Invoke(this, new ThreadExceptionEventArgs(ex));
            }
        }

        public void Dispose()
        {
            if (myThread != null)
                myThread.Abort();
        }
    }
}
