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

       
        public static string FilePath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), DefaultFileName);

        // Sets the file path used for logging operations.
        public static void ConfigureFilePath(string fileName)
        {
            FilePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        }

       
        // Appends a log entry with a timestamp, username and description to the log file and stack.
        public static void Log(string username, string description)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string line = $"{timestamp} | {username} | {description}";


                _stack.Push(line);

                File.AppendAllText(FilePath, line + Environment.NewLine);

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Logging failed: {ex.Message}");
            }
        }

       
        // Convenience overload that logs using the environment user name.
        public static void Log(string description)
        {
            Log(Environment.UserName, description);
           
        }

        
        // Attempts to pop the last log entry from the in-memory stack.
        public static bool TryPop(out string entry)
        {
            entry = null;
            if (_stack.IsEmpty)
                return false;

            entry = _stack.Pop();
            return entry != null;
        }

        
        // Attempts to peek at the most recent log entry without removing it.
        public static bool TryPeek(out string entry)
        {
            entry = null;
            if (_stack.IsEmpty)
                return false;

            entry = _stack.Peek();
            return entry != null;
        }

        
        // Returns all log entries currently stored in the in-memory stack.
        public static string[] GetAllEntries()
        {
            return _stack.GetAll().ToArray();
        }

        // Prints all in-memory log entries to the console with header/footer messages.
        public static void DisplayLogs()
        {
            Console.WriteLine(Messages.OperationHistoryTitle);
            string[] entries = GetAllEntries();
            if (entries.Length == 0)
            {
                Console.WriteLine(Messages.NoOperationLogs);
            }
            else
            {
                entries.ToList().ForEach(e => Console.WriteLine(e));
            }
            Console.WriteLine(Messages.OperationHistoryEnd);
        }

       
        
    }
}
