using System;
using System.Globalization;
using QA.TestAutomation.Framework.Configuration;

namespace QA.TestAutomation.Framework.Helpers
{
    public class TradeHours
    {
        private readonly bool _empty;
        private readonly TimeSpan _tradeHoursBeginning;
        private readonly TimeSpan _tradeHoursEnd;
        private readonly DayOfWeek[] _tradeDaysOfWeek;

        public TradeHours(TradeHoursConfiguration config, TimeZoneInfo timeZone)
        {
            if (!config.ElementInformation.IsPresent)
                _empty = true;
            else
            {
                _tradeHoursBeginning = config.StartTime.TimeOfDay;
                _tradeHoursEnd = config.EndTime.TimeOfDay;
                string[] sa = config.WorkingDays.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (sa.Length > 0)
                {
                    _tradeDaysOfWeek = new DayOfWeek[sa.Length];
                    for (int i = 0; i < sa.Length; i++)
                        _tradeDaysOfWeek[i] = (DayOfWeek)Array.IndexOf(DateTimeFormatInfo.InvariantInfo.AbbreviatedDayNames, sa[i]);
                }
                _empty = _tradeHoursBeginning == TimeSpan.Zero && _tradeHoursEnd == TimeSpan.Zero && _tradeDaysOfWeek == null;
            }
            if (_tradeHoursEnd == TimeSpan.Zero)
                _tradeHoursEnd = TimeSpan.FromHours(24);
            if (_empty || timeZone == null) return;
            TimeSpan timeSpan = timeZone.GetUtcOffset(DateTime.Now);
            _tradeHoursBeginning -= timeSpan;
            _tradeHoursEnd -= timeSpan;
        }

        public TimeSpan Beginning { get { return _tradeHoursBeginning; } }

        public TimeSpan End { get { return _tradeHoursEnd; } }

        public bool IsEmpty()
        {
            return _empty;
        }

        public bool IsTradeHours(DateTime dateTime, bool dayOrMorePeriod = false)
        {
            return _empty ||
                IsTradeDay(dateTime) &&
                    (dayOrMorePeriod || _tradeHoursBeginning <= dateTime.TimeOfDay && dateTime.TimeOfDay < _tradeHoursEnd);
        }

        public bool CorrectInterval(ref DateTime beginning, ref DateTime end, bool dayOrMorePeriod = false)
        {
            while (!IsTradeHours(beginning, dayOrMorePeriod))
                beginning += TimeSpan.FromMinutes(1);
            while (!IsTradeHours(end, dayOrMorePeriod))
                end -= TimeSpan.FromMinutes(1);
            return beginning <= end;
        }

        public bool IsTradeDay(DateTime dateTime)
        {
            if (_tradeDaysOfWeek == null)
                return true;
            DayOfWeek dayOfWeek = dateTime.DayOfWeek;
            for (int i = 0; i < _tradeDaysOfWeek.Length; i++)
            {
                if (_tradeDaysOfWeek[i] == dayOfWeek)
                    return true;
            }
            return false;
        }
    }
}
