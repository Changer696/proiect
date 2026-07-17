using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Lightweight file repository utilities: read, parse, append, and remove-last-line.
public static class FileRepository
{
    // Read all non-empty trimmed lines from a file. Returns empty list if file missing.
    public static List<string> ReadAllLines(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return new List<string>();
        if (!File.Exists(path)) return new List<string>();
        try
        {
            return File.ReadAllLines(path)
                       .Select(l => l?.Trim())
                       .Where(l => !string.IsNullOrWhiteSpace(l))
                       .ToList();
        }
        catch
        {
            return new List<string>();
        }
    }

    // Parse all lines using provided parser function. Skips lines where parser throws or returns null.
    public static List<T> LoadParsed<T>(string path, Func<string, T> parser)
        where T : class
    {
        var result = new List<T>();
        if (parser == null) return result;
        foreach (var line in ReadAllLines(path))
        {
            try
            {
                var obj = parser(line);
                if (obj != null) result.Add(obj);
            }
            catch
            {
                // skip malformed lines
            }
        }
        return result;
    }

    // Append a single line to the file. Creates file if missing. Returns true on success.
    public static bool AppendLine(string path, string line)
    {
        if (string.IsNullOrWhiteSpace(path) || line == null) return false;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? AppDomain.CurrentDomain.BaseDirectory);
            using (var sw = File.AppendText(path))
            {
                sw.WriteLine(line);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Remove the last non-empty line from the file. Returns true when a line was removed.
    public static bool RemoveLastLine(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return false;
        try
        {
            var lines = File.ReadAllLines(path).ToList();
            // find last non-empty line index
            int idx = -1;
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                if (!string.IsNullOrWhiteSpace(lines[i])) { idx = i; break; }
            }
            if (idx == -1) return false;
            lines.RemoveAt(idx);
            File.WriteAllLines(path, lines);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Remove the last non-empty line only if the line matches the provided predicate.
    public static bool RemoveLastLineIf(string path, Func<string, bool> predicate)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path) || predicate == null) return false;
        try
        {
            var lines = File.ReadAllLines(path).ToList();
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var trimmed = lines[i]?.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;
                if (predicate(trimmed))
                {
                    lines.RemoveAt(i);
                    File.WriteAllLines(path, lines);
                    return true;
                }
                return false;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    // Get the last non-empty line or null if none.
    public static string GetLastLine(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return null;
        try
        {
            var lines = File.ReadAllLines(path);
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                var t = lines[i]?.Trim();
                if (!string.IsNullOrWhiteSpace(t)) return t;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}
