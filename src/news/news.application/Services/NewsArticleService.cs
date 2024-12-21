using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using news.application.Contracts.DTO;
using news.application.Contracts.Interfaces;
using news.application.Exceptions;
using news.application.Settings;
using news.application.Utilities;
using news.domain.Models;
using System;


namespace news.application.Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsRepository _newsRepository;
        //private readonly IViewingService _viewingService;
        private readonly INewsLocalFileStorageContext _newsLocalFileStorageContext;
        private readonly ILogger<NewsArticleService> _logger;
        private readonly FileStorageSettings _settings;
        private readonly IMessagingService _messagingService;
        private readonly ICoreService _coreService;

        public NewsArticleService(INewsRepository newsRepository, INewsLocalFileStorageContext newsLocalFileStorageContext,
            ILogger<NewsArticleService> logger, IOptions<FileStorageSettings> options, IMessagingService messagingService, ICoreService coreService)
        {
            _newsRepository = newsRepository;
            _newsLocalFileStorageContext = newsLocalFileStorageContext;
            _logger = logger;
            _settings = options.Value;
            _messagingService = messagingService;
            _coreService = coreService;
        }

        public async Task<NewsArticleDTO> CreateNewsArticleAsync(NewsArticleCreationDTO newsArticleCreationDTO, CancellationToken cancellationToken)
        {
            NewsArticle newsArticle = newsArticleCreationDTO;
            var result = (NewsArticleDTO)await _newsRepository.CreateNewsArticleAsync(newsArticle, cancellationToken);

            if (newsArticleCreationDTO.Important)
            {
                List<string> PhoneNumbers = await _coreService.GetPhoneNumbers(5, 1, cancellationToken);
                MessageNewsRequestDTO dto = new MessageNewsRequestDTO();
                dto.NewsArticleTitle = newsArticleCreationDTO.Title;
                //dto.NewsArticleId = result.Id.ToString();
                dto.PhoneNumbers = PhoneNumbers;
                _messagingService.SendNewsViaSMS(dto, cancellationToken);
            }
            return result;
        }

        public async Task DeleteNewsArticleAsync(Guid newsArticleId, CancellationToken cancellationToken)
        {
            NewsArticle newsArticle = await _newsRepository.GetNewsArticleByIdAsync(newsArticleId, cancellationToken) ?? throw new NewsApplicationResourceNotFoundException($"news article with id {newsArticleId} was not found");

            await _newsRepository.DeleteNewsArticleAsync(newsArticle, cancellationToken);
        }

        public async Task<NewsArticleDTO> GetNewsArticleByIdAsync(Guid newsArticleId, bool canSeeFuture, CancellationToken cancellationToken)
        {
            NewsArticle newsArticle = await _newsRepository.GetNewsArticleByIdAsync(newsArticleId, cancellationToken)
                ?? throw new NewsApplicationResourceNotFoundException($"news article with id {newsArticleId} was not found");

            if (!canSeeFuture)
            {
                if (newsArticle.PublishDate >= DateTime.Now) throw new NewsApplicationAccessToFutureArticleDeniedException("you cannot see future news articles");
            }

            return (NewsArticleDTO)newsArticle;
        }

        public async Task<List<NewsArticlePreviewDTO>> GetPreviewOfAllNewsByQueryAsync(NewsArticlePreviewQueryParams queryParams, bool canSeeFuture, CancellationToken cancellationToken)
        {
            var result = await _newsRepository.GetNewsArticlesByQueryAsync(queryParams, canSeeFuture, cancellationToken);
            if (result == null)
            {
                return new List<NewsArticlePreviewDTO>();
            }
            List<NewsArticlePreviewDTO> output = new();
            foreach (var item in result)
            {
                output.Add((NewsArticlePreviewDTO)item);
            }
            return output;

        }

        public async Task<NewsArticleDTO> UpdateNewsArticleAsync(Guid newsArticleId, NewsArticleCreationDTO updateDTO, CancellationToken cancellationToken)
        {
            NewsArticle newsArticle = await _newsRepository.GetNewsArticleByIdAsync(newsArticleId, cancellationToken) ?? throw new NewsApplicationResourceNotFoundException($"news article with id {newsArticleId} was not found");
            /*newsArticle =*/
            NewsArticleCreationDTO.ApplyUpdate(ref newsArticle, updateDTO);
            return (NewsArticleDTO)await _newsRepository.UpdateNewsArticleAsync(newsArticle, cancellationToken);
        }
        public async Task<NewsArticleDTO> AddMultiMediaToNewsArticleAsync(Guid newsArticleId, MemoryStream memoryStream, string fileName, string contentType, CancellationToken cancellationToken, bool isThumbnail)
        // make all the persitances in one go to not have a file in storage without corresponding metadata in database // this was for the other domain .
        {
            // generate proper id for file
            MediaType mediaType = Convertors.ConvertContentTypeToMediaType(contentType, _settings);
            //do this before streaming the file inside
            NewsArticle newsArticle = await _newsRepository.GetNewsArticleByIdAsync(newsArticleId, cancellationToken) ?? throw new NewsApplicationResourceNotFoundException($"news article with id {newsArticleId} was not found");


            MultiMedia multiMedia;
            try
            {
                string filePath = await _newsLocalFileStorageContext.SaveNewFileToNewsArticleMultiMediaStorage(newsArticleId, memoryStream, fileName, cancellationToken);
                multiMedia = new MultiMedia(filePath, mediaType);
            }
            catch (NewsApplicationFileAlreadyExistsException ex)
            {
                string exsitedUrl = (string)ex.InnerException.Data["ExistedURL"];

                multiMedia = newsArticle.MultiMedias.Where(mm => mm.URL == exsitedUrl).SingleOrDefault();

                if (multiMedia is null && newsArticle.Thumbnail is not null && newsArticle.Thumbnail?.URL == exsitedUrl)
                {
                    multiMedia = newsArticle.Thumbnail;
                }
                else if (multiMedia is null && newsArticle.Thumbnail is null)
                {
                    _logger.LogCritical($"infrastructure has a file for news article with id: {newsArticleId} but the news article entity does not have multi media with url: {newsArticleId}/{fileName}");
                    multiMedia = new MultiMedia(fileName, mediaType);
                }

                await AddMultiMediaToNewsArticleAsync(newsArticle, multiMedia, isThumbnail, cancellationToken);

                ex.Data["NewsArticleDTO"] = (NewsArticleDTO)newsArticle;
                throw ex;
            }

            await AddMultiMediaToNewsArticleAsync(newsArticle, multiMedia, isThumbnail, cancellationToken);

            return newsArticle;

        }
        private async Task AddMultiMediaToNewsArticleAsync(NewsArticle newsArticle, MultiMedia multiMedia, bool isThumbnail, CancellationToken cancellationToken)
        {
            if (isThumbnail)
            {
                newsArticle.Thumbnail = multiMedia;
            }
            else newsArticle.MultiMedias.Add(multiMedia);

            await _newsRepository.UpdateNewsArticleAsync(newsArticle, cancellationToken);
        }
        public async Task<NewsArticleDTO> AddViewerToNewsArticleAsync(Guid newsArticleId, Guid viewerId, CancellationToken cancellationToken)
        {
            NewsArticle newsArticle = await _newsRepository.GetNewsArticleByIdAsync(newsArticleId, cancellationToken) ?? throw new NewsApplicationResourceNotFoundException($"news article with id {newsArticleId} was not found"); ;
            if (newsArticle.Viewers.Any(v => v.ViewerId == viewerId))
            {
                var ex = new NewsApplicationResourceAlreadyExistsException($"viewer with id {viewerId} has already viewed new article with id {newsArticleId}", null);
                ex.Data["NewsArticleDTO"] = (NewsArticleDTO)newsArticle;
                throw ex;
            }
            else newsArticle.Viewers.Add(new Viewer(viewerId));
            return await _newsRepository.UpdateNewsArticleAsync(newsArticle, cancellationToken);
        }

        public async Task MessageNewsArticles(List<Guid> newsArticles, List<string> phoneNumbers, CancellationToken cancellationToken)
        {
            List<NewsArticle> articles = await _newsRepository.GetNewsArticleListByIds(newsArticles, cancellationToken);
            List<(string, string)> texts = articles.Select(na => (na.Description, na.Title)).ToList();
            //foreach (var tuple in texts) {
            //    await _messagingService.SendNewsViaSMS(tuple.Item1,tuple.Item2 , phoneNumbers ,cancellationToken);
            //}
        }

        public async Task<NewsArticleDTO> AddMultiMediaToNewsArticleAsyncByUrl(Guid newsArticleId, bool isThumbnail, string url, string type)
        {
            var model = await _newsRepository.GetNewsArticleByIdAsync(newsArticleId) ?? throw new NewsApplicationResourceNotFoundException();
            if (isThumbnail)
            {
                model.Thumbnail = new MultiMedia(url, Convertors.ConvertContentTypeToMediaType(type, _settings));
            }
            else
            {
                model.MultiMedias.Add(new MultiMedia(url, Convertors.ConvertContentTypeToMediaType(type, _settings)));
            }



            await _newsRepository.UpdateNewsArticleAsync(model);

            return model;

        }
    }
}
