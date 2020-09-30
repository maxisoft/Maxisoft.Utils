using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Maxisoft.Utils.TimeRanges
{
    public static class Parser
    {
        private const string TimeRangeRegexString =
            "^(\\d{1,2}(:\\d{1,2})?(:\\d{1,2})?)\\s*(->)\\s*(\\d{1,2}(:\\d{1,2})?(:\\d{1,2})?)$";

        private const string TimePointRegexString =
            "^(\\d{1,2})(:\\d{1,2})?(:\\d{1,2})?$";

        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<Regex> _timeRangeRegex = new Lazy<Regex>(() => new Regex(TimeRangeRegexString));

        public static Regex TimeRangeRegex => _timeRangeRegex.Value;

        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<Regex> _timePointRegex = new Lazy<Regex>(() => new Regex(TimePointRegexString));

        public static Regex TimePointRegex => _timePointRegex.Value;

        internal static bool TryParse(string playload, out TimeRange timeRange)
        {
            timeRange = default(TimeRange);
            var match = TimeRangeRegex.Match(playload);
            if (!match.Success)
            {
                return false;
            }

            var tp1 = match.Groups[1];
            if (!tp1.Success) return false;

            TimePoint start, end;
            if (!TryParse(tp1.Value.Trim(), out start)) return false;
            var tp2 = match.Groups[5];
            if (!tp2.Success) return false;
            if (!TryParse(tp2.Value.Trim(), out end)) return false;
            timeRange = new TimeRange(start, end);
            return true;
        }

        internal static bool TryParse(string playload, out TimePoint timePoint)
        {
            timePoint = default(TimePoint);
            var match = TimePointRegex.Match(playload);
            if (!match.Success)
            {
                return false;
            }
            int h;
            var m = 0;
            var s = 0;
            var hg = match.Groups[1];
            if (!hg.Success) return false;
            if (!int.TryParse(hg.Value, out h))
            {
                return false;
            }

            var mg = match.Groups[2];
            if (mg.Success)
            {
                if (!int.TryParse(mg.Value.TrimStart(':', ' '), out m)) return false;
            }

            var sg = match.Groups[3];
            if (sg.Success)
            {
                if (!int.TryParse(sg.Value.TrimStart(':', ' '), out s)) return false;
            }

            timePoint = new TimePoint(h, m, s);
            return true;
        }
    }
}