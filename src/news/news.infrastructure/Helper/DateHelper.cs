using System.Globalization;

namespace news.infrastructure.Helper;

public static class DateHelper
{
    public static string ConvertGeoToJalaiSimple(this DateTime date)
    {
        PersianCalendar p = new PersianCalendar();
        CultureInfo persianCulture = new CultureInfo("fa-IR");
        string persianDate = date.ToString("yyyy-MM-dd", persianCulture);
        return persianDate;
    }

    public static string ConvertGeoToJalaiDayName(this DateTime date)
    {
        PersianCalendar p = new PersianCalendar();
        CultureInfo persianCulture = new CultureInfo("fa-IR");
        string persianDate = date.ToString("dddd d MMMM yyyy", persianCulture);

        return persianDate;

    }
}
