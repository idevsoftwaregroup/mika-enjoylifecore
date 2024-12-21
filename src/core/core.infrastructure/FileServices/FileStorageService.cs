using core.application.Contract.infrastructure;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;


namespace core.infrastructure.FileServices;

public class FileStorageService : IFileStorageService
{
    private readonly HttpClient _httpClient;
    private readonly FileStorageSettings _fileStorageSettings;

    public FileStorageService(HttpClient httpClient, IOptions<FileStorageSettings> options)
    {
        _httpClient = httpClient;
        _fileStorageSettings = options.Value;
    }


    public async Task<string> UploadTicketingAttachment(Stream stream, string fileName, string contentType, int ticketId, long messageId, CancellationToken cancellationToken = default)
    {
        using MultipartFormDataContent form = new MultipartFormDataContent();
        //stream.Seek(0, SeekOrigin.Begin);
        var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "formFile",
            FileName = fileName
        };

        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

        form.Add(fileContent, $"ticket_{ticketId}_{messageId}_attachment", fileName);

        //form.Add(new StringContent("Some additional data"), "otherField");

        HttpResponseMessage responseMessage = await _httpClient.PostAsync(_fileStorageSettings.FileStorageUrl + $"?ServiceType={_fileStorageSettings.TicketingServiceType}", form, cancellationToken);


        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new Exception("attachment could not be uploaded", new Exception(await responseMessage.Content.ReadAsStringAsync(cancellationToken)));
        }

        //FileStorageResponseDTO responseJson = JsonSerializer.Deserialize<FileStorageResponseDTO>(result) ?? throw new Exception("response body is null or not deserializable");
        var result = await responseMessage.Content.ReadFromJsonAsync<FileStorageResponseDTO>() ?? throw new Exception("response body is null or not deserializable"); //.ReadAsStringAsync(cancellationToken);
        result.Url = System.Web.HttpUtility.UrlDecode(result.Url);
        return result.Url;

    }

    private record FileStorageResponseDTO
    {
        public string Url { get; set; }
    }
}
