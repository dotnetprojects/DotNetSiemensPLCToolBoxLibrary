using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DotNetSimaticDatabaseProtokollerLibrary.Common
{
    using System.Net.Mail;
    using System.Threading;

    using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;

    public static class Logging
    {
        private static Thread myThread;

        private static List<string> _stringMessageList = new List<string>();
        private static List<DateTime> _DateTimesList = new List<DateTime>();

        private static volatile int _maxAdd = 0;

        private static volatile bool _firstEntry = false;

        public static LoggingTarget SelectedLoggingTarget = LoggingTarget.EventLog;

        public static LogLevel SelectedLogLevel = Logging.LogLevel.Information;

        private static EventLog eventlog = new EventLog(StaticServiceConfig.Company, ".", StaticServiceConfig.MyServiceName + "Service");

        public static EventLogEntryCollection LogEntries { get { return new EventLog(StaticServiceConfig.Company, ".", StaticServiceConfig.MyServiceName + "Service").Entries; } }

        public static void ClearLog()
        {            
            (new EventLog(StaticServiceConfig.Company, ".", StaticServiceConfig.MyServiceName + "Service")).Clear();
        }

        public static string LogText(string Message, Exception ex, Logging.LogLevel MessageLogLevel)
        {
            Message += "\r\n";
            Message += "Message: " + ex.Message + "\r\n";
            Message += "Stacktrace: " + ex.StackTrace + "\r\n";

            Exception inner = ex.InnerException;
            int i = 0;
            while (inner != null)
            {
                i++;
                Message += "Inner Exception (" + i.ToString() + ") :";
                Message += "Message:" + inner.Message + "\r\n";
                Message += "Stacktrace: " + ex.StackTrace + "\r\n";
                inner = inner.InnerException;
            }

            return LogText(Message, MessageLogLevel);
        }

        public static string LogText(string Message, LogLevel MessageLogLevel)
        {
            if (MessageLogLevel <= SelectedLogLevel)
            {
                if (SelectedLoggingTarget == LoggingTarget.EventLog)
                {
                    EventLogEntryType entrType = EventLogEntryType.Error;
                    if (MessageLogLevel == Logging.LogLevel.Warning)
                        entrType = EventLogEntryType.Warning;
                    else if (MessageLogLevel == Logging.LogLevel.Information)
                        entrType = EventLogEntryType.Information;

                    try
                    {
                        eventlog.WriteEntry(Message, entrType);
                    }
                    catch (Exception ex)
                    { }
                    try
                    {
                        if (ProtokollerConfiguration.ActualConfigInstance.UseMailInform)
                        {
                            LogErrorLevel tmpLogLevel = LogErrorLevel.Error;
                            switch (MessageLogLevel)
                            {
                                case LogLevel.None:
                                    tmpLogLevel = LogErrorLevel.None;
                                    break;
                                case LogLevel.Error:
                                    tmpLogLevel = LogErrorLevel.Error;
                                    break;

                                case LogLevel.Warning:
                                    tmpLogLevel = LogErrorLevel.Warning;
                                    break;

                                case LogLevel.Information:
                                    tmpLogLevel = LogErrorLevel.Information;
                                    break;
                            }
                            SendMail(Message, tmpLogLevel);
                        }
                    }
                    catch (Exception)
                    {}
                }
                else
                {
                    return Message;
                }
            }
            return null;
        }

        public enum LoggingTarget
        {
            EventLog,
        }

        public enum LogLevel
        {
            None = 0,
            Error = 1,
            Warning = 2,
            Information = 3
        }

        private static void SendMail(string Message, LogErrorLevel MessageLogLevel)
        {
            if (MessageLogLevel <= ProtokollerConfiguration.ActualConfigInstance.ErrorLevel)
            //if (MessageLogLevel <= Logging.LogLevel.Warning)
            {
                lock (_stringMessageList)
                {
                    _stringMessageList.Add(Message);
                    _DateTimesList.Add(DateTime.Now);
                }

                if (myThread == null)
                {
                    try
                    {
                        myThread = new Thread(new ThreadStart(ThreadProc));
                        myThread.Name = "Thread from Service: " + StaticServiceConfig.MyServiceName;
                        myThread.Start();
                        _firstEntry = true;
                    }
                    catch (Exception ex)
                    {
                        LogText("Error!!!!!!", ex, Logging.LogLevel.Information);
                    }
                }
            }
        }

        private static void ThreadProc()
        {
            try
            {
                while (true)
                {
                    bool ok = false;
                    Thread.Sleep(ProtokollerConfiguration.ActualConfigInstance.SendInterval);
                    if (_stringMessageList.Count > 0)
                    {
                        if (_firstEntry)
                        {
                            Thread.Sleep(ProtokollerConfiguration.ActualConfigInstance.SendInterval);
                            _firstEntry = false;
                        }
                        lock (_stringMessageList) _maxAdd = _stringMessageList.Count;

                        try
                        {
                            ok = _internalSend();
                        }
                        catch (ThreadAbortException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            LogText("Error Sending Mail to :" + ProtokollerConfiguration.ActualConfigInstance.Recipient, Logging.LogLevel.Error);
                        }

                        if (ok)
                            lock (_stringMessageList)
                            {
                                _stringMessageList.RemoveRange(0, _maxAdd);
                                _DateTimesList.RemoveRange(0, _maxAdd);
                            }
                    }
                }
            }
            catch (ThreadAbortException)
            {
                LogText("Start des Thread nicht möglich", Logging.LogLevel.Error);
                return;
            }
        }

        private static volatile string Body;

        private static bool _internalSend()
        {
            MailMessage Mail = new MailMessage();
            MailAddress Sender = new MailAddress(ProtokollerConfiguration.ActualConfigInstance.Sender);
            Mail.From = Sender;
            Mail.To.Add(ProtokollerConfiguration.ActualConfigInstance.Recipient);
            Mail.Subject = ProtokollerConfiguration.ActualConfigInstance.Subject + " (" + _maxAdd + " Logged Messages)";
            Body = null;
            lock (_stringMessageList)
            {
                for (int i = 0; i < _maxAdd; i++)
                {
                    Body += "\r\n ***** Timestamp: " + _DateTimesList[i].ToString() + " ***** Message " + (i + 1) + "/" + _maxAdd + " ***** ***** \r\n " + _stringMessageList[i].ToString();
                }
            }
            Mail.Body = Body;
            SmtpClient MailClient = new SmtpClient(ProtokollerConfiguration.ActualConfigInstance.SmtpServer, ProtokollerConfiguration.ActualConfigInstance.SmtpPort);
            bool LoginNecessary = false;
            if (LoginNecessary)
            {
                System.Net.NetworkCredential Credential = new System.Net.NetworkCredential(ProtokollerConfiguration.ActualConfigInstance.SmtpUsername, ProtokollerConfiguration.ActualConfigInstance.SmtpPassword);
                MailClient.Credentials = Credential;
            }
            try
            {
                MailClient.Send(Mail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
