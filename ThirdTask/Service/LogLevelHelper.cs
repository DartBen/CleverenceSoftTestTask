namespace ThirdTask.Service
{
    public class LogLevelHelper
    {
        public static LogLevel NormalizeLevel(string inputLevel)
        {
            return inputLevel switch
            {
                "INFORMATION" => LogLevel.INFO,
                "WARNING" => LogLevel.WARN,
                "ERROR" => LogLevel.ERROR,
                "DEBUG" => LogLevel.DEBUG,
                "INFO" => LogLevel.INFO,
                "WARN" => LogLevel.WARN,
                _ => LogLevel.Undefined
            };
        }
    }
}
