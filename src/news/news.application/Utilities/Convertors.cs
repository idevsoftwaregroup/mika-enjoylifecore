using news.application.Exceptions;
using news.application.Settings;
using news.domain.Models;

namespace news.application.Utilities;

public static class Convertors
{
    public static MediaType ConvertContentTypeToMediaType(string contentType, FileStorageSettings settings)
    {
        if (settings.ImageContentTypes.Contains(contentType)) { return MediaType.IMAGE; }
        else if (settings.VideoContentTypes.Contains(contentType)) { return MediaType.VIDEO; }
        else if (settings.GifContentTypes.Contains(contentType)) { return MediaType.GIF; }
        else throw new NewsApplicationInvalidContentTypeException($"invalid Content type {contentType}");
    }
}
