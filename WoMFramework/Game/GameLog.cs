namespace WoMFramework.Game
{
    using Enums;
    using Generator;
    using System.Collections.Generic;

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

    public enum LogType
    {
        Info,
        Damage,
        Heal,
        Event,
        Comb,
        AdventureLog
    }

    public class LogEntry
    {
        public LogType LogType { get; }
        public int Source { get; set; }
        public int Target { get; set; }
        public DamageType DamageType { get; set; }
        public HealType HealType { get; set; }
        public string Message { get; }

        public LogEntry(LogType logType, string message)
        {
            LogType = logType;
            Message = message;
        }

        private LogEntry(LogType logType, int source, params object[] list)
        {
            Source = source;
        }

        public override string ToString()
        {
            return $"{GetHeader(LogType)} {Message}";
        }

        private string GetHeader(LogType logType)
        {
            switch (logType)
            {
                case LogType.Info:
                    return "[INFO]";
                case LogType.Damage:
                    return "[DAMG]";
                case LogType.Heal:
                    return "[HEAL]";
                case LogType.Event:
                    return "[EVNT]";
                case LogType.Comb:
                    return "[COMB]";
                default:
                    return "[INFO]";
            }
        }

        public static LogEntry Damage(Combatant entity, int damageAmount, DamageType damageType)
        {
            return new LogEntry(LogType.Damage,
                entity.Adventure != null ? entity.AdventureEntityId : 0,
                damageAmount,
                damageType);
        }
    }
}
