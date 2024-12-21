using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace core.application.Framework
{
    public static class FormatHelper
    {
        public static bool IsNumeric(this string str, bool nullable)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
            {
                if (nullable)
                {
                    return true;
                }
                return false;
            }
            return Regex.IsMatch(str.ToEnglishNumber(), "^[0-9]*$", RegexOptions.IgnoreCase);
        }
        public static bool IsNotNullAndZero(this int? num)
        {
            if (num == null || num <= 0)
            {
                return false;
            }
            return true;
        }
        public static bool IsNotNullAndZero(this long? num)
        {
            if (num == null || num <= 0)
            {
                return false;
            }
            return true;
        }
        public static bool IsNotNull(this string? str)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
            {
                return false;
            }
            return true;
        }
        public static bool IsNotNullAndZero(this decimal? dec)
        {
            if (dec == null || dec <= 0)
            {
                return false;
            }
            return true;
        }
        public static bool IsNotNull(this decimal? dec)
        {
            if (dec == null || dec < 0)
            {
                return false;
            }
            return true;
        }
        public static bool IsNationalCode(this string str, bool nullable)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(str))
            {
                if (nullable)
                {
                    return true;
                }
                return false;
            }
            if (!str.IsNumeric(nullable))
            {
                return false;
            }
            if (str.Length < 10 || str.Length > 11)
            {
                return false;
            }
            return true;
        }
        public static bool IsEmail(this string str, bool nullable)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
            {
                if (nullable)
                {
                    return true;
                }
                return false;
            }
            return Regex.IsMatch(str, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }
        public static bool IsMobile(this string str)
        {
            return str.IsNumeric(false) && str.Length == 11 && str.StartsWith("09");
        }
        public static bool IsUrl(this string str)
        {
            string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex reg = new(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(str);
        }
        public static string AllSpaceRemover(this string str)
        {
            try
            {
                return str.Replace(" ", "");
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string MultipleSpaceRemover(this string str)
        {
            Regex regex = new("[ ]{2,}", RegexOptions.None);
            return regex.Replace(str, " ");
        }
        public static string MultipleSpaceRemoverTrim(this string str)
        {
            Regex regex = new("[ ]{2,}", RegexOptions.None);
            return regex.Replace(str, " ").Trim();
        }
        public static string PersianToArabic(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Replace("ی", "ي").Replace("ک", "ك");
            }
            return str;
        }
        public static string ArabicToPersian(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Replace("ي", "ی").Replace("ك", "ک");
            }
            return str;
        }
        public static string ToPersianNumber(this string data)
        {
            return data.Replace("0", "۰").Replace("1", "۱").Replace("2", "۲").Replace("3", "۳").Replace("4", "۴").Replace("5", "۵").Replace("6", "۶").Replace("7", "۷").Replace("8", "۸").Replace("9", "۹");
        }
        public static string ToPersianNumber(this int data)
        {
            return data.ToString().Replace("0", "۰").Replace("1", "۱").Replace("2", "۲").Replace("3", "۳").Replace("4", "۴").Replace("5", "۵").Replace("6", "۶").Replace("7", "۷").Replace("8", "۸").Replace("9", "۹");
        }
        public static string ToPersianNumber(this long data)
        {
            return data.ToString().Replace("0", "۰").Replace("1", "۱").Replace("2", "۲").Replace("3", "۳").Replace("4", "۴").Replace("5", "۵").Replace("6", "۶").Replace("7", "۷").Replace("8", "۸").Replace("9", "۹");
        }
        public static string ToEnglishNumber(this string data)
        {
            return data.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("۷", "7").Replace("۸", "8").Replace("۹", "9");
        }
        public static string ToEnglishNumber(this int data)
        {
            return data.ToString().Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("۷", "7").Replace("۸", "8").Replace("۹", "9");
        }
    }
}
