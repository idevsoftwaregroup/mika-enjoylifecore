//using news.application.Contracts.DTOs.ParamsDTOs;
//using news.domain.Models.Aggregates.Building;
//using news.domain.Models.Aggregates.NewsArticle;
//using news.domain.Models.Aggregates.Party;

using news.application.Contracts.DTO;
using news.domain.Models;

namespace news.application.Contracts.Interfaces
{
    public interface INewsRepository
    {
        Task<NewsArticle?> GetNewsArticleByIdAsync(Guid newsArticleId, CancellationToken cancellationToken = default);
        Task<NewsArticle> CreateNewsArticleAsync(NewsArticle newsArticle, CancellationToken cancellationToken);
        Task DeleteNewsArticleAsync(NewsArticle newsArticle, CancellationToken cancellationToken);
        Task<List<NewsArticle>> GetNewsArticlesByQueryAsync(NewsArticlePreviewQueryParams queryParams, bool canSeeFuture, CancellationToken cancellationToken);
        Task<NewsArticle> UpdateNewsArticleAsync(NewsArticle newsArticle, CancellationToken cancellationToken = default);
        Task<List<NewsArticle>> GetNewsArticleListByIds(List<Guid> newsArticleIds, CancellationToken cancellationToken = default);
        
    }
}