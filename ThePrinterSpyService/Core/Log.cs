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
            this.Message = message;
            this.StackTrace = stackTrace;
        }
    }

    static class Log
    {
        public static void AddException(Exception exception)
        {
            File.AppendAllText(@"Log\error.txt", FormatMsg(new ErrorMessage(exception.Message, exception.StackTrace)));
        }
        
        public static void AddTextLine(string message)
        {
            File.AppendAllText(@"Log\error.txt", message);
        }

        private static string FormatMsg(ErrorMessage errorMessage)
        {
            return $@"[ERROR] {DateTime.Now:HH:mm:ss}: [Message] {errorMessage.Message} [Stack] {errorMessage.StackTrace}\r\n";
        }
    }
}
