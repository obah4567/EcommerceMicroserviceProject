using Serilog;

namespace ECommerce.SharedLibrary.Logs
{
    public static class LogException
    {
        public static void LogExceptions(Exception ex)
        {
            LogToFile(ex.Message);
            LogToConsole(ex.Message);
            LogToDebugger(ex.Message);
        }

        public static void LogToConsole(string message) => Log.Information(message);

        public static void LogToDebugger(string message) => Log.Warning(message);

        private static void LogToFile(string message) => Log.Debug(message);
    }
}
