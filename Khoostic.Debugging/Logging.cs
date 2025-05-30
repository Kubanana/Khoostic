using System.Runtime.CompilerServices;

namespace BiggyTools.Debugging
{
    public static class Logger
    {
        private enum LogType
        {
            Log,
            Warning,
            Error
        }

        public static void Log(string text, [CallerFilePath] string? callerFilePath = null)
        {
            PrintLog(text, LogType.Log, callerFilePath);
        }

        public static void LogWarning(string text, [CallerFilePath] string? callerFilePath = null)
        {
            PrintLog(text, LogType.Warning, callerFilePath);
        }

        public static void LogError(string text, [CallerFilePath] string? callerFilePath = null)
        {
            PrintLog(text, LogType.Error, callerFilePath);   
        }

        private static void PrintLog(string text, LogType logType, [CallerFilePath] string? callerFilePath = null)
        {
            string className = "UnknownClass";

            if (!string.IsNullOrEmpty(callerFilePath))
            {
                string fileName = Path.GetFileNameWithoutExtension(callerFilePath);

                className = fileName;
            }

            Console.WriteLine($"{className}::{logType}::{text}");
        }
    }
}