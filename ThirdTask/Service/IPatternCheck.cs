using System.Text.RegularExpressions;

namespace ThirdTask.Service
{
    public interface IPatternLogChecker
    {
        LogMetadata TryCheckString(string log);
    }
}
