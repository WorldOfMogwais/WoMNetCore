using System.Collections.Generic;

namespace WoMFramework.Game
{
    public class GameLog
    {
        public int Pointer;
        public List<LogEntry> LogEntries;

        public GameLog()
        {
            LogEntries = new List<LogEntry>();
            Pointer = LogEntries.Count - 1;
        }

        public void Add(LogType logType, string message)
        {
            Add(new LogEntry(logType, message));
        }

        public void Add(LogEntry entry)
        {
            LogEntries.Add(entry);
            Pointer = LogEntries.Count - 1;
        }

    }

    public enum LogType { Info, Damg, Heal, Evnt, Comb,
        AdventureLog
    }

    public class LogEntry
    {
        public LogType LogType { get; }
        public string Message { get; }

        public LogEntry(LogType logType, string message)
        {
            LogType = logType;
            Message = message;
        }

        public override string ToString()
        {
            return string.Format("{0}{1} {2}", "", GetHeader(LogType), Message);
        }

        private string GetHeader(LogType logType)
        {
            switch (logType)
            {
                case LogType.Info:
                    return "[INFO]";
                case LogType.Damg:
                    return "[DAMG]";
                case LogType.Heal:
                    return "[HEAL]";
                case LogType.Evnt:
                    return "[EVNT]";
                case LogType.Comb:
                    return "[COMB]";
                default:
                    return "[INFO]";
            }
        }

    }

}
