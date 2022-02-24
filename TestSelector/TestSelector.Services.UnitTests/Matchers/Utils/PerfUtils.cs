using System;
using System.Diagnostics;

namespace TestSelector.Services.UnitTests.Matchers.Utils
{
    public static class PerfUtils
    {
        public static TimeSpan Time(Action toTime)
        {
            var timer = Stopwatch.StartNew();
            toTime();
            timer.Stop();
            return timer.Elapsed;
        }
    }
}