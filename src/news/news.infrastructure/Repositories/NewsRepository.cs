using Microsoft.EntityFrameworkCore;
using news.application.Contracts.DTO;
using news.application.Contracts.Interfaces;
using news.domain.Models;
using news.infrastructure.Data;
using news.infrastructure.Exceptions;

namespace news.infrastructure.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly NewsContext _context;

        public NewsRepository(NewsContext context)
        {
            _context = context;
        }

        public async Task<List<NewsArticle>> GetNewsArticleListByIds(List<Guid> newsArticleIds,CancellationToken cancellationToken = default)
        {
            return await _context.NewsArticles.Where(na => newsArticleIds.Contains(na.Id)).ToListAsync(); // probably change to a for each loop and use .any
        }

        public async Task<NewsArticle> CreateNewsArticleAsync(NewsArticle newsArticle, CancellationToken cancellationToken)
        {


            if (newsArticle.Pinned && _context.NewsArticles.Where(na => na.Pinned)
                .Where(na => na.BuildingId == newsArticle.BuildingId)
                .Count(g => g.Pinned == true) > (NewsArticle.MAX_NUMBER_OF_PINNED_NEWS - 1))
            {
                var ex = new NewsInfrastructurePersistenceConstraintException($"buildings cant have more than {NewsArticle.MAX_NUMBER_OF_PINNED_NEWS} pinned news articles");
                ex.Data["newsArticleId"] = $"provided id is {newsArticle.Id}";
                ex.Data["buildingId"] = $"provided id is {newsArticle.BuildingId}";
                throw ex;
            }

            var tracker = await _context.NewsArticles.AddAsync(newsArticle, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            //return (await GetNewsArticleByIdAsync(newsArticle.Id, cancellationToken)) ?? throw new NewsInfrastructureException("could not get news article after creation");
            return tracker.Entity;
        }

        public async Task DeleteNewsArticleAsync(NewsArticle newsArticle, CancellationToken cancellationToken)
        {
            _context.NewsArticles.Remove(newsArticle);
            await _context.SaveChangesAsync(cancellationToken);
            if (await _context.NewsArticles.AnyAsync(na => na.Id.Equals(newsArticle.Id))) throw new NewsInfrastructureException($"the news article with id {newsArticle.Id} was not deleted");
        }

        public async Task<NewsArticle?> GetNewsArticleByIdAsync(Guid newsArticleId, CancellationToken cancellationToken = default)
        {
            return await _context.NewsArticles.Where(na => na.Id == newsArticleId).SingleOrDefaultAsync();
        }

        public async Task<List<NewsArticle>> GetNewsArticlesByQueryAsync(NewsArticlePreviewQueryParams queryParams, bool canSeeFuture, CancellationToken cancellationToken)
        {
            IQueryable<NewsArticle> query = _context.NewsArticles;

            if (queryParams.NewsTag is not null)
            {
                query = query.Where(na => na.NewsTag == queryParams.NewsTag);
            }
            //if (queryParams.BuildingId is not null || queryParams.BuildingId.Equals(Guid.Empty))
            //{
            query = query.Where(na => na.BuildingId == queryParams.BuildingId);
            //}
            if (queryParams.FromDate is not null)
            {
                query = query.Where(na => na.PublishDate >= queryParams.FromDate);
            }
            if (queryParams.Important is not null)
            {
                query = query.Where(na => na.Important == queryParams.Important);
            }

            if(!canSeeFuture)
            {
               query = query.Where(na => na.PublishDate <= DateTime.Now);
            }

            query = query.OrderByDescending(na => na.Pinned).ThenByDescending(na => na.Important).ThenByDescending(na => na.PublishDate);

            query = query.Skip(queryParams.PageSize * (queryParams.PageNumber - 1)).Take(queryParams.PageSize);
            List<NewsArticle>? result = null;
            try
            {
                result = await query.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {

                var message = ex.Message;
            }
            
            return result;

        }

        public async Task<NewsArticle> UpdateNewsArticleAsync(NewsArticle newsArticle, CancellationToken cancellationToken = default)
        {

            if (newsArticle.Pinned
                && _context.NewsArticles.Where(na => na.Id == newsArticle.Id).Any(na => !na.Pinned)
                && _context.NewsArticles.Where(na => na.Pinned)
                .Where(na => na.BuildingId == newsArticle.BuildingId)
                .Count(g => g.Pinned) > (NewsArticle.MAX_NUMBER_OF_PINNED_NEWS - 1))
            {
                var ex = new NewsInfrastructurePersistenceConstraintException($"buildings cant have more than {NewsArticle.MAX_NUMBER_OF_PINNED_NEWS} pinned news articles");
                ex.Data["newsArticleId"] = $"provided id is {newsArticle.Id}";
                ex.Data["buildingId"] = $"provided id is {newsArticle.BuildingId}";
                throw ex;
            }

            var tracker = _context.NewsArticles.Update(newsArticle);
            await _context.SaveChangesAsync(cancellationToken);
            return tracker.Entity;
        }

    }
}
