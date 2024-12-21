using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityProvider.Application.Helper
{
    public static class DateTimeHelper
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
        public static string GetPersianShortDate(this DateTime date)
        {
            PersianCalendar persianCalendar = new();
            var year = persianCalendar.GetYear(date).ToString();
            var month = persianCalendar.GetMonth(date).ToString().Length == 1 ? "0" + persianCalendar.GetMonth(date).ToString() : persianCalendar.GetMonth(date).ToString();
            var day = persianCalendar.GetDayOfMonth(date).ToString().Length == 1 ? "0" + persianCalendar.GetDayOfMonth(date).ToString() : persianCalendar.GetDayOfMonth(date).ToString();
            return $"{year}/{month}/{day}";
        }

    }
}
