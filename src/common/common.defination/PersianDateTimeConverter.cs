using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace core.api
{
    public class PersianDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && DateTime.TryParse(reader.GetString(), new CultureInfo("fa-IR"), DateTimeStyles.None, out var date))
            {
                return date;
            }

            return reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            try
            {
                string formattedDate = value.ToString("yyyy/MM/dd", new CultureInfo("fa-IR"));
                writer.WriteStringValue(formattedDate);
            }
            catch (Exception)
            {

                writer.WriteStringValue(value);
            }

        }
    }

}
