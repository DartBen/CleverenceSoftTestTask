using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ThirdTask.Service
{
    interface IPatternLogChecker
    {
        void SetInputPattern(Regex pattern);
        LogMetadata TryCheckString(string log);
    }
}
