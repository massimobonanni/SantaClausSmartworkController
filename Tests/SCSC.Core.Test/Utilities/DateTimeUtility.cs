using System;
using System.Collections.Generic;
using System.Text;

namespace SCSC.Core.Test.Utilities
{
    internal static class DateTimeUtility
    {
        public static DateTimeOffset Create(int hour, int minute, int second)
        {
            var now = DateTime.Now;
            return new DateTimeOffset(now.Year, now.Month, now.Day, hour, minute, second, new TimeSpan(0, 0, 0));
        }
    }
}
