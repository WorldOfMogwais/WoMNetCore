using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoMFramework.Game
{
    public class GameLog
    {
        public int pointer;
        public List<LogEntry> logEntries;

        public GameLog()
        {

            logEntries = new List<LogEntry>();
            pointer = logEntries.Count - 1;
        }

        public void Add(LogType logType, string message)
        {
            Add(new LogEntry(logType, message));
        }

        public void Add(LogEntry entry)
        {
            logEntries.Add(entry);
            pointer = logEntries.Count - 1;
        }

    }

    public enum LogType { INFO, DAMG, HEAL, EVNT, COMB }

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
                case LogType.INFO:
                    return "[¬aINFO§]";
                case LogType.DAMG:
                    return "[¬rDAMG§]";
                case LogType.HEAL:
                    return "[¬gHEAL§]";
                case LogType.EVNT:
                    return "[¬YEVNT§]";
                case LogType.COMB:
                    return "[¬yCOMB§]";
                default:
                    return "[¬aINFO§]";
            }
        }

    }

}
