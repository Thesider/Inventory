using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace Inventory.Logger
{
    public class AppLogger : IAppLogger
    {
        private readonly string _logFolder;
        private readonly string _currentLogFile;
        private readonly string _bugLogFile;
        private readonly ConcurrentQueue<string> _memoryLogs;
        private const int MaxMemoryLogs = 1000;

        public AppLogger()
        {
            _logFolder = Path.Combine(FileSystem.AppDataDirectory, "Logs");
            _currentLogFile = Path.Combine(_logFolder, $"log_{DateTime.Now:yyyyMMdd}.txt");
            _bugLogFile = Path.Combine(_logFolder, "buglog.txt");
            _memoryLogs = new ConcurrentQueue<string>();

            if (!Directory.Exists(_logFolder))
            {
                Directory.CreateDirectory(_logFolder);
            }
        }

        public void Log(LogLevel level, string message)
        {
            var logEntry = FormatLogEntry(level, message);
            WriteLogEntry(logEntry);
        }

        public void Log(LogLevel level, string message, Exception exception)
        {
            var logEntry = FormatLogEntry(level, message, exception);
            WriteLogEntry(logEntry);
        }

        public void LogDebug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        public void LogInformation(string message)
        {
            Log(LogLevel.Information, message);
        }

        public void LogWarning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        public void LogError(string message, Exception? exception = null)
        {
            if (exception != null)
                Log(LogLevel.Error, message, exception);
            else
                Log(LogLevel.Error, message);
        }

        public void LogCritical(string message, Exception? exception = null)
        {
            if (exception != null)
                Log(LogLevel.Critical, message, exception);
            else
                Log(LogLevel.Critical, message);
        }

        public void LogToFile(string message)
        {
            try
            {
                File.AppendAllText(_currentLogFile, $"{message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                // If we can't write to file, at least keep it in memory
                LogToMemory($"Failed to write to log file: {ex.Message}");
            }
        }

        public string[] GetLogFiles()
        {
            try
            {
                return Directory.GetFiles(_logFolder, "log_*.txt")
                               .OrderByDescending(f => f)
                               .ToArray();
            }
            catch (Exception)
            {
                return Array.Empty<string>();
            }
        }

        public string GetLatestLogs(int numberOfLines = 100)
        {
            try
            {
                if (File.Exists(_currentLogFile))
                {
                    var lines = File.ReadLines(_currentLogFile).Reverse().Take(numberOfLines).Reverse();
                    return string.Join(Environment.NewLine, lines);
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string FormatLogEntry(LogLevel level, string message, Exception? exception = null)
        {
            var sb = new StringBuilder();
            sb.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ");
            sb.Append($"[{level}] ");
            sb.Append(message);

            if (exception != null)
            {
                sb.AppendLine();
                sb.Append("Exception: ");
                sb.AppendLine(exception.Message);
                sb.AppendLine("Stack Trace:");
                sb.Append(exception.StackTrace);
            }

            return sb.ToString();
        }

        private void WriteLogEntry(string logEntry)
        {
            LogToMemory(logEntry);
            LogToFile(logEntry);

            // In debug mode, also write to debug output
#if DEBUG
            System.Diagnostics.Debug.WriteLine(logEntry);
#endif
        }

        private void LogToMemory(string logEntry)
        {
            _memoryLogs.Enqueue(logEntry);

            // Keep memory logs under control
            while (_memoryLogs.Count > MaxMemoryLogs)
            {
                _memoryLogs.TryDequeue(out _);
            }
        }

        public string[] GetMemoryLogs()
        {
            return _memoryLogs.ToArray();
        }

        public async Task SaveBugLog(BugLog bugLog)
        {
            try
            {
                var logEntry = FormatBugLogEntry(bugLog);
                await File.AppendAllTextAsync(_bugLogFile, $"{logEntry}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                LogError("Failed to save bug report", ex);
            }
        }

        private string FormatBugLogEntry(BugLog bugLog)
        {
            var sb = new StringBuilder();
            sb.Append($"[{bugLog.DateCreated:yyyy-MM-dd HH:mm:ss.fff}] ");
            sb.Append($"Message: {bugLog.Message}");
            if (!string.IsNullOrEmpty(bugLog.StackTrace))
            {
                sb.AppendLine();
                sb.Append("Stack Trace: ");
                sb.Append(bugLog.StackTrace);
            }
            return sb.ToString();
        }

        public async Task<BugLog[]> GetBugList()
        {
            try
            {
                if (File.Exists(_bugLogFile))
                {
                    var lines = await File.ReadAllLinesAsync(_bugLogFile);
                    var bugLogs = lines.Select(line => JsonSerializer.Deserialize<BugLog>(line)!)
                                       .Where(bugLog => bugLog != null)
                                       .ToArray();
                    return bugLogs;
                }

                return Array.Empty<BugLog>();
            }
            catch (Exception ex)
            {
                LogError("Failed to retrieve bug reports", ex);
                return Array.Empty<BugLog>();
            }
        }

        Task<string[]> IAppLogger.GetLogFiles()
        {
            throw new NotImplementedException();
        }

        Task<string> IAppLogger.GetLatestLogs(int numberOfLines)
        {
            throw new NotImplementedException();
        }
    }
}
