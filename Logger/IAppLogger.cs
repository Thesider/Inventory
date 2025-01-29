namespace Inventory.Logger
{
    public enum LogLevel
    {
        Debug,
        Information,
        Warning,
        Error,
        Critical
    }

    public interface IAppLogger
    {
        void Log(LogLevel level, string message);
        void Log(LogLevel level, string message, Exception exception);
        void LogDebug(string message);
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? exception = null);
        void LogCritical(string message, Exception? exception = null);

        // Specific to MAUI platform logging
        void LogToFile(string message);
        Task<string[]> GetLogFiles();
        Task<string> GetLatestLogs(int numberOfLines = 100);
    }
}