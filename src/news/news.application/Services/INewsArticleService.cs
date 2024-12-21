//using news.application.Contracts.DTOs.NewsArticleDTOs;
//using news.application.Contracts.DTOs.ParamsDTOs;
//using news.application.Contracts.DTOs.UserDTOs;

using news.application.Contracts.DTO;

namespace news.application.Services
{
    public interface INewsArticleService
    {
        Task<NewsArticleDTO> GetNewsArticleByIdAsync(Guid newsArticleId, bool canSeeFuture, CancellationToken cancellationToken);

        //Task<NewsArticleDTO> AddNewsArticleToBuildingAsync(NewsArticleCreationDTO newsArticleCreationDTO, CancellationToken cancellationToken);
        //void DeleteNewsArticleFromBuilding();
        //Task<NewsArticleDTO> GetNewsArticleOfBuilding(Guid newsArticleId, Guid buildingId, CancellationToken cancellationToken);
        //Task<IEnumerable<NewsArticlePreviewDTO>> GetPreviewOfNewsArticlesOfBuildingAsync(NewsArticlesQueryParams newsArticlesQueryParams, CancellationToken cancellationToken);
        //void UpdateNewsArticleOfBuilding();

        ////Task SetNewsArticleViewedByUser(Guid userId, Guid newsArticleId, CancellationToken cancellationToken);
        ////Task<IEnumerable<ViewerDTO>> GetViewersOfNewsArticleAsync(Guid newsArticleId, CancellationToken cancellationToken);
        //Task<MultiMediaContentDTO> AddMultiMediaContentToNewsArticleAsync(Guid newsArticleId, string formatType, MemoryStream memoryStream,  CancellationToken cancellationToken);
        Task<List<NewsArticlePreviewDTO>> GetPreviewOfAllNewsByQueryAsync(NewsArticlePreviewQueryParams queryParams, bool canSeeFuture, CancellationToken cancellationToken);
        Task<NewsArticleDTO> CreateNewsArticleAsync(NewsArticleCreationDTO newsArticleDTO, CancellationToken cancellationToken);
        Task<NewsArticleDTO> UpdateNewsArticleAsync(Guid newsArticleId, NewsArticleCreationDTO updateDTO, CancellationToken cancellationToken);
        Task DeleteNewsArticleAsync(Guid newsArticleId, CancellationToken cancellationToken);
        Task<NewsArticleDTO> AddMultiMediaToNewsArticleAsync(Guid newsArticleId, MemoryStream memoryStream, string fileName,string contentType, CancellationToken cancellationToken, bool isThumbnail);
        Task<NewsArticleDTO> AddViewerToNewsArticleAsync(Guid newsArticleId, Guid viewerId, CancellationToken cancellationToken);
        Task MessageNewsArticles(List<Guid> newsArticles, List<string> phoneNumbers, CancellationToken cancellationToken);
        Task<NewsArticleDTO> AddMultiMediaToNewsArticleAsyncByUrl(Guid newsArticleId, bool isThumbnail, string url, string type);
    }
}