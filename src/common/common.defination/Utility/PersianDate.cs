using System.Globalization;

namespace common.defination.Utility;

public static class PersianDate
{
    public static string ConvertGeoToJalaiSimple(this DateTime date)
    {
        PersianCalendar p = new PersianCalendar();
        CultureInfo persianCulture = new CultureInfo("fa-IR");
        string persianDate = date.ToString("yy/MM/dd", persianCulture);
        return persianDate;
    }

    public static string ConvertGregToJalaiMonthName(this DateTime gregorianDate)
    {


        // Set the Persian culture
        CultureInfo persianCulture = new CultureInfo("fa-IR");

        // Create a DateTimeFormatInfo for the Persian culture
        DateTimeFormatInfo persianDateTimeFormat = persianCulture.DateTimeFormat;

        // Format the date in the desired format
        string persianDate = gregorianDate.ToString("dd MMMM , yyyy", persianDateTimeFormat);

        return persianDate;

        //return $"{d:D2} {monthNames[m - 1]} , {y % 100:D2}";   // this is innverted in front when i tested locally idk if server os will affect it or not pls check if possible

    }

    public static string ConvertGeoToJalaiDayName(this DateTime date)
    {
        PersianCalendar p = new PersianCalendar();
        CultureInfo persianCulture = new CultureInfo("fa-IR");
        string persianDate = date.ToString("dddd d MMMM yyyy", persianCulture);

        return persianDate;

    }

    public static string? ConvertToSimpleDateTimePersian(this DateTime? date)
    {
        if (date is null) return null;
        DateTime date1 = (DateTime)date; 
        CultureInfo persianCulture = new CultureInfo("fa-IR");
        string persianDate = date1.ToString("yy/MM/d HH:mm", persianCulture);

        return persianDate;
    }
}
