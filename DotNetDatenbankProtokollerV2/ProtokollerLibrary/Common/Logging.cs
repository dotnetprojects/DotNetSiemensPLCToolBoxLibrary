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

                    eventlog.WriteEntry(Message, entrType);                    
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
