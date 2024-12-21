using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Framework.Utilities
{
    public static class DateTimeHelper
    {
        public static string ToTimeStringNullable(this TimeSpan? timeSpan)
        {
            if (timeSpan == null)
            {
                return null;
            }
            if (!TimeSpan.TryParse(timeSpan.ToString(), out TimeSpan time))
            {
                return null;

            }
            return TimeSpan.Parse(timeSpan.ToString()).ToString(@"hh\:mm");
        }
        public static TimeSpan? ToTimeSpanNullable(this string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
            {
                return null;
            }
            if (!TimeSpan.TryParse(timeString, out TimeSpan time))
            {
                return null;
            }
            return TimeSpan.Parse(timeString);

        }
        public static TimeSpan ToTimeSpan(this string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
            {
                return TimeSpan.Parse("00:00");
            }
            if (!TimeSpan.TryParse(timeString, out TimeSpan time))
            {
                return TimeSpan.Parse("00:00");
            }
            return TimeSpan.Parse(timeString);

        }
        public static DateTime ToGregorianDate(this string dateString)
        {
            var parts = dateString.Split("/", 3);
            System.Globalization.PersianCalendar persianCalender = new();
            return persianCalender.ToDateTime(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]), 0, 0, 0, 0);
        }
        public static string ToPersianDate(this DateTime dateTime)
        {
            System.Globalization.PersianCalendar persianCalendar = new();
            return persianCalendar.GetYear(dateTime) + "/" + persianCalendar.GetMonth(dateTime) + "/" + persianCalendar.GetDayOfMonth(dateTime);
        }
        public static string ToPersianDateTime(this DateTime dateTime)
        {
            System.Globalization.PersianCalendar persianCalendar = new();
            var hour = persianCalendar.GetHour(dateTime).ToString();
            var minute = persianCalendar.GetMinute(dateTime).ToString();
            if (persianCalendar.GetHour(dateTime) < 10)
            {
                hour = "0" + persianCalendar.GetHour(dateTime).ToString();
            }
            if (persianCalendar.GetMinute(dateTime) < 10)
            {
                minute = "0" + persianCalendar.GetMinute(dateTime).ToString();
            }
            return persianCalendar.GetYear(dateTime) + "/" + persianCalendar.GetMonth(dateTime) + "/" + persianCalendar.GetDayOfMonth(dateTime) + "  " + hour + ":" + minute;
        }
        public static DateTime ResetTime(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
        }
        public static DateTime ResetTime(this DateTime? dateTime)
        {
            return new DateTime(Convert.ToDateTime(dateTime).Year, Convert.ToDateTime(dateTime).Month, Convert.ToDateTime(dateTime).Day, 0, 0, 0, 0);
        }

    }
}
