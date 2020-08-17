using System;
using System.IO;

namespace ThePrinterSpyService.Core
{
    struct ErrorMessage
    {
        public string Message { get; }
        public string StackTrace { get; }

        public ErrorMessage(string message, string stackTrace)
        {
            Message = message;
            StackTrace = stackTrace;
        }
    }

    static class Log
    {
        private static readonly string CurrentPath;
        static Log()
        {
            CurrentPath = Path.Combine(Environment.CurrentDirectory, "Log");
            if (!Directory.Exists(CurrentPath)) Directory.CreateDirectory(CurrentPath);
        }

        public static void AddException(Exception exception)
        {
            File.AppendAllText($@"{CurrentPath}\error.txt", FormatMsg(new ErrorMessage(exception.Message, exception.StackTrace)));
        }
        
        public static void AddTextLine(string message)
        {
            File.AppendAllText($@"{CurrentPath}\log.txt", message);
        }

        private static string FormatMsg(ErrorMessage errorMessage)
        {
            return $@"[ERROR] {DateTime.Now:HH:mm:ss}: [Message] {errorMessage.Message} [Stack] {errorMessage.StackTrace}"+"\\r\\n";
        }
    }
}
