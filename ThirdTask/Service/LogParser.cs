using System.Text.RegularExpressions;

namespace ThirdTask.Service
{
    public class LogParser : IPatternLogChecker
    {
        static HashSet<string> groupNames = new HashSet<string>() { "date", "time", "loglevel", "message", "invoker" };

        private Regex _regex;

        public LogParser(Regex pattern)
        {
            if (CheckRegex(pattern))
                _regex = pattern;
        }

        private static bool CheckRegex(Regex pattern)
        {
            var groupNames = pattern.GetGroupNames();

            foreach (var group in groupNames)
            {
                if (!groupNames.Contains(group))
                    return false;
            }

            return true;
        }

        public LogMetadata TryCheckString(string log)
        {
            var match = _regex.Match(log);
            if (match.Success)
            {
                var date = DateOnly.Parse(match.Groups["date"].Value);
                var time = match.Groups["time"].Value;
                var level = LogLevelHelper.NormalizeLevel(match.Groups["loglevel"].Value);
                var message = match.Groups["message"].Value;
                var invoker = match.Groups["invoker"].Value;

                return new LogMetadata(match.Success, date, time, level, message, invoker);
            }

            // если ошибка парсинга
            return LogMetadata.BadResult(match.Success, "");
        }
    }
}
