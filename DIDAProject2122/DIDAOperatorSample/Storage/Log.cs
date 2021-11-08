using System.Collections.Generic;

namespace Storage
{
    class Log
    {
        private readonly List<LogEntry> logEntries;

        internal List<LogEntry> LogEntries => logEntries;

        public Log()
        {
            logEntries = new List<LogEntry>();
        }

        public void AddWriteEntry(string id, string val)
        {
            var entry = new LogEntry { Id = id, NewValue = val, Type = LogEntryType.Write };
            LogEntries.Add(entry);
        }

        public void AddUpdateIfEntry(string id, string newVal, string oldVal)
        {
            var entry = new LogEntry { Id = id, NewValue = newVal, OldValue = oldVal ,Type = LogEntryType.UpdateIfValue };
            LogEntries.Add(entry);
        }

        public List<LogEntry> ToList()
        {
            return new List<LogEntry>(LogEntries);
        }

        public void Clear()
        {
            LogEntries.Clear();
        }
    }
}
