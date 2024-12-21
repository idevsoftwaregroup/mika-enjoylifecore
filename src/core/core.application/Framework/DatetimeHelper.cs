using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Framework
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
        public static string GetDayOfWeekTitle(this DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "دوشنبه",
                DayOfWeek.Tuesday => "سه شنبه",
                DayOfWeek.Wednesday => "چهارشنبه",
                DayOfWeek.Thursday => "پنجشنبه",
                DayOfWeek.Friday => "جمعه",
                DayOfWeek.Saturday => "شنبه",
                DayOfWeek.Sunday => "یکشنبه",
                _ => string.Empty,
            };
        }
        public static string GetDayOfWeekTitle(this DateTime date)
        {
            return GetDayOfWeekTitle(date.DayOfWeek);
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
        public static string GetTimeString(this TimeSpan time)
        {
            return $"{time.Hours}:{time.Minutes}";
        }
        public static string GetTimeString(this TimeSpan? time)
        {
            return $"{((TimeSpan)time).Hours}:{((TimeSpan)time).Minutes}";
        }
        public static string GetPersianDateString(this DateTime date)
        {
            PersianCalendar persianCalendar = new();
            return $"{persianCalendar.GetYear(date)}/{persianCalendar.GetMonth(date)}/{persianCalendar.GetDayOfMonth(date)}";
        }
        public static string GetPersianFullDate(this DateTime date)
        {
            PersianCalendar persianCalendar = new();
            return $"{((int)persianCalendar.GetDayOfWeek(date)).GetDayOfWeekTitle()} ، {persianCalendar.GetDayOfMonth(date)} {persianCalendar.GetMonth(date).GetMonthTitle()} ماه {persianCalendar.GetYear(date)}";

        }
        public static DateTime SetDateAndTime(DateTime date, TimeSpan time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0);
        }
        public static DateTime SetDateAndTime(DateTime date, TimeSpan? time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time == null ? 0 : ((TimeSpan)time).Hours, time == null ? 0 : ((TimeSpan)time).Minutes, 0);
        }
        public static DateTime SetDateAndTime(DateTime? date, TimeSpan? time)
        {
            return new DateTime(Convert.ToDateTime(date).Year, Convert.ToDateTime(date).Month, Convert.ToDateTime(date).Day, time == null ? 0 : ((TimeSpan)time).Hours, time == null ? 0 : ((TimeSpan)time).Minutes, 0);
        }
        public static DateTime ResetTime(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
        }
        public static DateTime ResetTime(this DateTime? dateTime)
        {
            return new DateTime(Convert.ToDateTime(dateTime).Year, Convert.ToDateTime(dateTime).Month, Convert.ToDateTime(dateTime).Day, 0, 0, 0, 0);
        }
        public static DateTime ResetTimeEnd(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0).AddDays(1).AddMinutes(-1);
        }
        public static DateTime ResetTimeEnd(this DateTime? dateTime)
        {
            return new DateTime(Convert.ToDateTime(dateTime).Year, Convert.ToDateTime(dateTime).Month, Convert.ToDateTime(dateTime).Day, 0, 0, 0, 0).AddDays(1).AddMinutes(-1);
        }
        public static DateTime ToGregorianDate(this string dateString)
        {
            var parts = dateString.Split("/", 3);
            System.Globalization.PersianCalendar persianCalender = new();
            return persianCalender.ToDateTime(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]), 0, 0, 0, 0);
        }
        public static string GetPersianShortDate(this DateTime date)
        {
            PersianCalendar persianCalendar = new();
            var year = persianCalendar.GetYear(date).ToString();
            var month = persianCalendar.GetMonth(date).ToString().Length == 1 ? "0" + persianCalendar.GetMonth(date).ToString() : persianCalendar.GetMonth(date).ToString();
            var day = persianCalendar.GetDayOfMonth(date).ToString().Length == 1 ? "0" + persianCalendar.GetDayOfMonth(date).ToString() : persianCalendar.GetDayOfMonth(date).ToString();
            return $"{year}/{month}/{day}";
        }
        public static int GetCurrentPersianYear()
        {
            PersianCalendar persianCalendar = new();
            return persianCalendar.GetYear(DateTime.Now);
        }
        public static int GetCurrentPersianMonth()
        {
            PersianCalendar persianCalendar = new();
            return persianCalendar.GetMonth(DateTime.Now);
        }
        public static int GetGregorianDayOfWeek(this int persianDay)
        {
            return persianDay switch
            {
                1 => (int)DayOfWeek.Saturday,
                2 => (int)DayOfWeek.Sunday,
                3 => (int)DayOfWeek.Monday,
                4 => (int)DayOfWeek.Tuesday,
                5 => (int)DayOfWeek.Wednesday,
                6 => (int)DayOfWeek.Thursday,
                7 => (int)DayOfWeek.Friday,
                _ => (int)DayOfWeek.Saturday,
            };
        }
        public static int GetPersianDayOfWeek(this int day)
        {
            return day switch
            {
                6 => 1,
                0 => 2,
                1 => 3,
                2 => 4,
                3 => 5,
                4 => 6,
                5 => 7,
                _ => 1,
            };
        }
        public static int GetGregorianYear(this int persianYear)
        {
            PersianCalendar persianCalendar = new();
            var convertedDate = persianCalendar.ToDateTime(persianYear, 1, 1, 0, 0, 0, 0);
            return convertedDate.Year;
        }
        public static int GetGregorianMonth(this int persianMonth)
        {
            PersianCalendar persianCalendar = new();
            var convertedDate = persianCalendar.ToDateTime(1400, persianMonth, 1, 0, 0, 0, 0);
            return convertedDate.Month;
        }
        public static List<DateTime> GetDatesOfMonthWithDayOfWeeks(int year, int month, List<int> days)
        {
            List<DateTime> dates = new();
            PersianCalendar persianCalendar = new();
            int daysInMonth = persianCalendar.GetDaysInMonth(year, month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime currentDate = persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
                if (days.Any(x => x == ((int)currentDate.DayOfWeek).GetPersianDayOfWeek()))
                {
                    dates.Add(currentDate.ResetTime());
                }
            }
            return dates;

        }
        public static TimeSpan ResetTimeSpan(this DateTime date)
        {
            var time = Convert.ToDateTime(date).TimeOfDay;
            return new TimeSpan(time.Hours, time.Minutes, 0);
        }
        public static TimeSpan ResetTimeSpan(this DateTime? date)
        {
            var time = Convert.ToDateTime(date).TimeOfDay;
            return new TimeSpan(time.Hours, time.Minutes, 0);
        }
        public static TimeSpan ResetTimeSpan(this TimeSpan? time)
        {
            return new TimeSpan(time == null ? 0 : ((TimeSpan)time).Hours, time == null ? 0 : ((TimeSpan)time).Minutes, 0);
        }
        public static TimeSpan ResetTimeSpan(this TimeSpan time)
        {
            return new TimeSpan(time.Hours, time.Minutes, 0);
        }
        public static TimeSpan SetTimeSpan(int h, int m)
        {
            return new TimeSpan(h, m, 0);
        }
        public static string GetHhMmFromTimeSpan(this TimeSpan time)
        {
            try
            {
                return $"{time.Hours}:{time.Minutes:00}";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static string GetHhMmFromDate(this DateTime date)
        {
            return GetHhMmFromTimeSpan(date.ResetTimeSpan());
        }
        public static (DateTime From, DateTime To) GetWeekOfDate(this DateTime date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Saturday => (date.ResetTime(), date.AddDays(6).ResetTimeEnd()),
                DayOfWeek.Sunday => (date.AddDays(-1).ResetTime(), date.AddDays(5).ResetTimeEnd()),
                DayOfWeek.Monday => (date.AddDays(-2).ResetTime(), date.AddDays(4).ResetTimeEnd()),
                DayOfWeek.Tuesday => (date.AddDays(-3).ResetTime(), date.AddDays(3).ResetTimeEnd()),
                DayOfWeek.Wednesday => (date.AddDays(-4).ResetTime(), date.AddDays(2).ResetTimeEnd()),
                DayOfWeek.Thursday => (date.AddDays(-5).ResetTime(), date.AddDays(1).ResetTimeEnd()),
                DayOfWeek.Friday => (date.AddDays(-6).ResetTime(), date.ResetTimeEnd()),
                _ => (date.ResetTime(), date.AddDays(6).ResetTimeEnd()),
            };
        }
        
        public static (string Key, int Value) GetTimeConcept(this int hours)
        {
            if (hours < 24)
            {
                return ("hourly", hours);
            }
            else if (hours >= 24 && hours < 7 * 24)
            {
                return hours._GetDailyTimeConcept();
            }
            else if (hours >= 7 * 24 && hours < 30 * 24)
            {
                return hours._GetWeeklyTimeConcept();
            }else
            {
                return hours._GetMonthlyTimeConcept();
            }
        }
        private static (string Key, int Value) _GetDailyTimeConcept(this int hours)
        {
            return hours % 24 == 0 ? ("daily", hours / 24) : ("hourly", hours);
        }
        private static (string Key, int Value) _GetWeeklyTimeConcept(this int hours)
        {
            return hours % (7 * 24) == 0 ? ("weekly", hours / (7 * 24)) : hours._GetDailyTimeConcept();
        }
        private static (string Key, int Value) _GetMonthlyTimeConcept(this int hours)
        {
            return hours % (30 * 24) == 0 ? ("monthly", hours / (30 * 24)) : hours._GetWeeklyTimeConcept();
        }
    }
}
