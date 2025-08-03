using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Utilities.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime AsiaZone(this DateTime dateTime)
        {
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, cstZone);
        }

        public static long MillisecondsTimestamp(this DateTime date)
        {
            DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)(date.ToUniversalTime() - baseDate).TotalMilliseconds;
        }

        public static DateTime MillisecondsTimestampToDateTime(this long timestamp)
        {
            DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var datetime = baseDate.AddMilliseconds(timestamp).ToLocalTime();

            return datetime;
        }
        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.DaysInMonth());
        }
    }
}
