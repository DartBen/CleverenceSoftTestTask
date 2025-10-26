using System.Text.RegularExpressions;

namespace ThirdTask.Service
{
    public interface ILogParserFactory
    {
        IPatternLogChecker CreateParser(Regex regex);
    }
}
