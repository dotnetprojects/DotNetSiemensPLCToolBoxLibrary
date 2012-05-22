using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DotNetSimaticDatabaseProtokollerLibrary.Common
{
    public static class Logging
    {
        public static LoggingTarget SelectedLoggingTarget = LoggingTarget.EventLog;

        public static LogLevel SelectedLogLevel = LogLevel.Information;

        private static EventLog eventlog = new EventLog(StaticServiceConfig.Company, ".", StaticServiceConfig.MyServiceName + "Service");

        public static EventLogEntryCollection LogEntries { get { return new EventLog(StaticServiceConfig.Company, ".", StaticServiceConfig.MyServiceName + "Service").Entries; } }

        public static void ClearLog()
        {            
            (new EventLog(StaticServiceConfig.Company, ".", StaticServiceConfig.MyServiceName + "Service")).Clear();
        }

        public static string LogText(string Message, Exception ex, LogLevel MessageLogLevel)
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
                    if (MessageLogLevel == LogLevel.Warning)
                        entrType = EventLogEntryType.Warning;
                    else if (MessageLogLevel == LogLevel.Information)
                        entrType = EventLogEntryType.Information;

                    try
                    {
                        eventlog.WriteEntry(Message, entrType);
                    }
                    catch (Exception ex)
                    { }
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
    }
}
