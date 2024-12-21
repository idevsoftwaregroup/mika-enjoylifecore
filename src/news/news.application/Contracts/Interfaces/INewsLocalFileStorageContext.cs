//using news.domain.Models.Aggregates.NewsArticle.MultiMedia;


namespace news.application.Contracts.Interfaces
{
    public interface INewsLocalFileStorageContext
    {
        Task<string> SaveNewFileToNewsArticleMultiMediaStorage(Guid newsArticleId, MemoryStream memoryStream, string fileName, CancellationToken cancellationToken);
    }
}