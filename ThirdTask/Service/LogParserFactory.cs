using System.Text.RegularExpressions;

namespace ThirdTask.Service
{
    public class LogParserFactory : ILogParserFactory
    {
        public IPatternLogChecker CreateParser(Regex regex)
        {
            return new LogParser(regex);
        }
    }
}
