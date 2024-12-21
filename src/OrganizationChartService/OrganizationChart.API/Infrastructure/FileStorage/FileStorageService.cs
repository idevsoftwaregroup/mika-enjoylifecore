using Microsoft.Extensions.Options;
using OrganizationChart.API.Settings;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;


namespace OrganizationChart.API.Infrastructure.FileStorage;

public class FileStorageService : IFileStorageService
{
    private readonly HttpClient _httpClient;
    private readonly FileStorageSettings _fileStorageSettings;

    public FileStorageService(HttpClient httpClient, IOptions<FileStorageSettings> options)
    {
        _httpClient = httpClient;
        _fileStorageSettings = options.Value;
    }


    public async Task<string> SendProfilePictureToFileStorage(Stream stream, string fileName, string contentType, int employeeId, CancellationToken cancellationToken = default)
    {
        using MultipartFormDataContent form = new MultipartFormDataContent();

        var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "formFile",//$"user_{employeeId}_profile_picture",
            FileName = fileName
        };

        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);//MediaTypeHeaderValue.Parse("multipart/form-data");
        
        form.Add(fileContent, $"user_{employeeId}_profile_picture",fileName);
        
        //form.Add(new StringContent("Some additional data"), "otherField");
        
        HttpResponseMessage responseMessage = await _httpClient.PostAsync(_fileStorageSettings.FileStorageUrl + $"?ServiceType={_fileStorageSettings.ServiceType}", form, cancellationToken);

        
        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new Exception("picture could not be uploaded",new Exception(await responseMessage.Content.ReadAsStringAsync(cancellationToken)));
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
