using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using Quartz;
using Quartz.Impl;

namespace DotNetSimaticDatabaseProtokollerLibrary.Protocolling.Trigger
{    
    internal class QuartzTriggerThread : IDisposable
    {
        private IDBInterface dbInterface;
        private DatasetConfig datasetConfig;
        private Dictionary<ConnectionConfig, Object> activConnections;

        public event ThreadExceptionEventHandler ThreadExceptionOccured;

        private bool StartedAsService;

        private IScheduler _sched;

        internal class QuartzJob : IJob
        {
            public virtual Task Execute(IJobExecutionContext context)
            {
                ((Action) context.JobDetail.JobDataMap["CallbackMethod"]).Invoke();

                return Task.CompletedTask;
            }
        }

        public QuartzTriggerThread(IDBInterface dbInterface, DatasetConfig datasetConfig, Dictionary<ConnectionConfig, Object> activConnections, bool StartedAsService)
        {
            this.StartedAsService = StartedAsService;
            this.dbInterface = dbInterface;
            this.datasetConfig = datasetConfig;
            this.activConnections = activConnections;
        }

        public void StartTrigger()
        {
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            _sched = schedFact.GetScheduler().Result;

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<QuartzJob>()
                .WithIdentity("job_" + datasetConfig.Name, "group1")
                .Build();


            DateTimeOffset runTime = DateBuilder.EvenMinuteDate(DateTimeOffset.UtcNow);

            // Trigger the job to run on the next round minute
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger_" + datasetConfig.Name, "group1")
                .WithCronSchedule(datasetConfig.CronTab)
                //.StartAt(runTime)
                .Build();

            job.JobDataMap["CallbackMethod"] = new Action(TriggerCallback);

            _sched.ScheduleJob(job, trigger);
            _sched.Start();
        }


        private void TriggerCallback()
        {
            try
            {
                IEnumerable<object> values = ReadData.ReadDataFromDataSources(datasetConfig, datasetConfig.DatasetConfigRows, activConnections, StartedAsService);
                if (values != null)
                    dbInterface.Write(values);
            }
            catch (ThreadAbortException ex)
            {
                //ThreadExceptionOccured.Invoke(this, new ThreadExceptionEventArgs(ex));
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
            _sched.Shutdown(true);
        }
        
    }
}
