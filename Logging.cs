using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartFactorySimple
{
    public static class Logging
    {
        private static readonly object _lock = new object();
        private static readonly StackRepository<string> _stack = new StackRepository<string>();
        private const string DefaultFileName = "operations.txt";

        // FilePath can be overridden by the application if needed.
        public static string FilePath { get; set; } = Path.Combine(AppContext.BaseDirectory, DefaultFileName);

        // Log entry with explicit username
        public static void Log(string username, string description)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string line = $"{timestamp} | {username} | {description}";

                // push to in-memory stack (LIFO)
                _stack.Push(line);

                // persist to file
                lock (_lock)
                {
                    File.AppendAllText(FilePath, line + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Logging failed: {ex.Message}");
            }
        }

        // Log entry using current OS user as username
        public static void Log(string description)
        {
            Log(Environment.UserName, description);
        }

        // Try to pop the most recent log entry from the in-memory stack
        public static bool TryPop(out string entry)
        {
            entry = null;
            if (_stack.IsEmpty)
                return false;

            entry = _stack.Pop();
            return entry != null;
        }

        // Peek the most recent log entry without removing it
        public static bool TryPeek(out string entry)
        {
            entry = null;
            if (_stack.IsEmpty)
                return false;

            entry = _stack.Peek();
            return entry != null;
        }

        // Get a snapshot of all entries in LIFO order (most recent first)
        public static string[] GetAllEntries()
        {
            // StackRepository does not expose random access; snapshot by popping then restoring.
            List<string> snapshot = new List<string>();
            string entry;

            while (TryPop(out entry))
            {
                snapshot.Add(entry);
            }

            // restore stack contents in original order
            for (int i = snapshot.Count - 1; i >= 0; i--)
            {
                _stack.Push(snapshot[i]);
            }

            return snapshot.ToArray();
        }

        // Persist the current in-memory stack to the operations file. By default appends entries
        // in stack order (most recent first). Set overwrite=true to replace the file.
        public static void SaveStackToFile(bool overwrite = false)
        {
            try
            {
                lock (_lock)
                {
                    if (overwrite)
                        File.WriteAllText(FilePath, string.Empty);

                    var entries = GetAllEntries().Reverse().ToArray();
                    if (entries.Length > 0)
                        File.AppendAllLines(FilePath, entries);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Saving stack failed: {ex.Message}");
            }
        }
    }
}
