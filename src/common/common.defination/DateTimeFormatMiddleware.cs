namespace core.api
{
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class DateTimeFormatMiddleware
    {
        private readonly RequestDelegate _next;

        public DateTimeFormatMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {            
            {
                var originalBodyStream = context.Response.Body;

                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;
                    await _next(context);
                    if (context.Response.ContentType?.Contains("application/json") == true && ShouldChangeDateTimeFormat(context.Request.Path))
                    {
                        responseBody.Seek(0, SeekOrigin.Begin);
                        var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = null, 
                        };
                        options.Converters.Add(new PersianDateTimeConverter());
                        var responseObject = JsonSerializer.Deserialize<object>(responseText, options);
                        var formattedJson = JsonSerializer.Serialize(responseObject, options);
                        var buffer = Encoding.UTF8.GetBytes(formattedJson);
                        await originalBodyStream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        responseBody.Seek(0, SeekOrigin.Begin);
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                }
            }

        }
        private bool ShouldChangeDateTimeFormat(PathString path)
        {
            bool check = false;

            check = path.StartsWithSegments("/api/Expense");
            return check;
        }
    }

}
