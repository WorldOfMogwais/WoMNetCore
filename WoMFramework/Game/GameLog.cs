namespace WoMFramework.Game
{
    using Enums;
    using Generator;
    using System.Collections.Generic;

    public class GameLog
    {
        public List<LogEntry> LogEntries;

        public GameLog()
        {
            LogEntries = new List<LogEntry>();
        }

        public void Add(LogType logType, ActivityLog activityLog)
        {
            LogEntries.Add(new LogEntry(logType, "",activityLog));
        }
    }

    public enum LogType
    {
        Info,
        Move,
        Attack,
        Died,
        Entity,
        Looted
    }

    public class LogEntry
    {
        public LogType Type { get; internal set; }
        public string Message { get; }
        public ActivityLog ActivityLog { get; internal set; }

        public LogEntry(LogType logType, string message, ActivityLog activityLog = null)
        {
            Type = logType;
            Message = message;
            ActivityLog = activityLog;
        }

        public override string ToString()
        {
            return $"{LogType.Info.ToString().ToUpper()} {Message}";
        }
    }
}
