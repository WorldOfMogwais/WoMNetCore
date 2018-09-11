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

    public enum LogType { Info, Damg, Heal, Evnt, Comb }

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
            return string.Format("{0} {1}", GetHEader(LogType), Message);
        }

        private string GetHEader(LogType logType)
        {
            switch (logType)
            {
                case LogType.Info:
                    return "[¬aINFO§]";
                case LogType.Damg:
                    return "[¬rDAMG§]";
                case LogType.Heal:
                    return "[¬gHEAL§]";
                case LogType.Evnt:
                    return "[¬YEVNT§]";
                case LogType.Comb:
                    return "[¬yCOMB§]";
                default:
                    return "[¬aINFO§]";
            }
        }

    }

}
