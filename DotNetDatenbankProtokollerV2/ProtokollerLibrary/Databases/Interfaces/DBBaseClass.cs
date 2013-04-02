using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces
{
    using System.Threading;

    using DotNetSimaticDatabaseProtokollerLibrary.Common;
    using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
    using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;

    public abstract class DBBaseClass : IDBInterface
    {
        protected DatasetConfig datasetConfig;

        protected StorageConfig storCfg = null;

        public virtual void Close()
        { }

        public event ThreadExceptionEventHandler ThreadExceptionOccured;

        private bool _initiated = false;
        public virtual void Initiate(DatasetConfig dsConfig)
        {
            datasetConfig = dsConfig;
            storCfg = datasetConfig.Storage;
            Initiate();
        }

        private void Initiate()
        {
            try
            {
                Connect_To_Database(storCfg);
                CreateOrModify_TablesAndFields(datasetConfig.Name, datasetConfig);
                _initiated = true;
                Logging.LogText(
                    "Database Initialised: " + datasetConfig.Storage.Name + ", " + datasetConfig.Name,
                    Logging.LogLevel.Information);
            }
            catch (Exception ex)
            {
                RaiseThreadExceptionOccured(this, ex);
            }
        }

        protected bool RaiseThreadExceptionOccured(object sender, Exception ex)
        {
            if (ThreadExceptionOccured != null) 
                ThreadExceptionOccured(this, new ThreadExceptionEventArgs(ex));
            else
                return false;
            return true;
        }

        #region Schreiben...

        public virtual void Write(IEnumerable<object> values)
        {
            lock (_intValueList)
            {
                _intValueList.Add(values);
                _intDateTimesList.Add(DateTime.Now);
            }

            if (myThread == null)
            {
                myThread = new Thread(new ThreadStart(ThreadProc));
                myThread.Name = "Thread from Storage: " + datasetConfig.Storage.Name + " for DataSet: "
                                + datasetConfig.Name;
                myThread.Start();
            }
        }

        protected Thread myThread;

        protected List<IEnumerable<object>> _intValueList = new List<IEnumerable<Object>>();
        protected List<DateTime> _intDateTimesList = new List<DateTime>();

        protected volatile int _maxAdd = 0;


        private void ThreadProc()
        {
            try
            {
                while (true)
                {
                    if (_initiated)
                    {
                        bool ok = false;
                        if (_intValueList.Count > 0)
                        {
                            lock (_intValueList) _maxAdd = _intValueList.Count;

                            try
                            {
                                ok = _internal_Write();
                            }
                            catch (ThreadAbortException)
                            {
                                throw;
                            }
                            catch (Exception ex)
                            {
                                if (ThreadExceptionOccured != null) ThreadExceptionOccured.Invoke(this, new ThreadExceptionEventArgs(ex));
                                else Logging.LogText("Exception: ", ex, Logging.LogLevel.Error);
                            }

                            if (ok)
                                lock (_intValueList)
                                {
                                    _intValueList.RemoveRange(0, _maxAdd);
                                    _intDateTimesList.RemoveRange(0, _maxAdd);
                                }
                        }
                        else Thread.Sleep(20);
                    }
                    else
                    {
                        this.Initiate();
                    }
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
        }

        #endregion

        protected abstract bool _internal_Write();

        public abstract void Connect_To_Database(StorageConfig config);

        public abstract void CreateOrModify_TablesAndFields(string dataTable, DatasetConfig datasetConfig);

        public abstract void Dispose();

    }
}
