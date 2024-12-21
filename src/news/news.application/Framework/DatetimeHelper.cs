using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.application.Framework
{
    public static class DatetimeHelper
    {
        public static string GetDayOfWeekTitle(this int day)
        {
            return day switch
            {
                1 => "دوشنبه",
                2 => "سه شنبه",
                3 => "چهارشنبه",
                4 => "پنجشنبه",
                5 => "جمعه",
                6 => "شنبه",
                7 => "یکشنبه",
                _ => string.Empty,
            };
        }
        public static string GetMonthTitle(this int month)
        {
            return month switch
            {
                1 => "فروردین",
                2 => "اردیبهشت",
                3 => "خرداد",
                4 => "تیر",
                5 => "مرداد",
                6 => "شهریور",
                7 => "مهر",
                8 => "آبان",
                9 => "آذر",
                10 => "دی",
                11 => "بهمن",
                12 => "اسفند",
                _ => string.Empty,
            };
        }
        public static string GetPersianFullDate(this DateTime date)
        {
            PersianCalendar persianCalendar = new();
            return $"{((int)persianCalendar.GetDayOfWeek(date)).GetDayOfWeekTitle()} ، {persianCalendar.GetDayOfMonth(date)} {persianCalendar.GetMonth(date).GetMonthTitle()} ماه {persianCalendar.GetYear(date)}";

        }
        public static DateTime ResetTime(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
        }
        public static DateTime ResetTime(this DateTime? dateTime)
        {
            return new DateTime(Convert.ToDateTime(dateTime).Year, Convert.ToDateTime(dateTime).Month, Convert.ToDateTime(dateTime).Day, 0, 0, 0, 0);
        }
        public static DateTime ToGregorianDate(this string dateString)
        {
            var parts = dateString.Split("/", 3);
            System.Globalization.PersianCalendar persianCalender = new();
            return persianCalender.ToDateTime(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]), 0, 0, 0, 0);
        }

    }
}
