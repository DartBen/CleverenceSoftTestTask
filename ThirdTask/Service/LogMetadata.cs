using System.Reflection.Emit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ThirdTask.Service
{
    public record LogMetadata
    {
        public bool IsSuccess { get; }
        public LogType LogType { get; }
        public DateOnly Date { get; }
        public string Time { get; }
        public LogLevel LogLevel { get; }
        public string InvokerMethod { get; } = "DEFAULT";
        public string Message { get; }

        public LogMetadata(bool succes, DateOnly date, string time, LogLevel level, string message, string invoker = "DEFAULT")
        {
            IsSuccess = succes;
            Date = date;
            Time = time;
            LogLevel = level;

            if (invoker != string.Empty)
                InvokerMethod = invoker;

            Message = message;
        }

        private LogMetadata(bool succes, string message = "")
        {
            IsSuccess = succes;
            Message = message;

        }

        public static LogMetadata BadResult(bool succes, string message = "")
        {
            return new LogMetadata(succes, message);
        }

    }
}
