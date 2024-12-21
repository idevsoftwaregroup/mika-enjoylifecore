using Microsoft.AspNetCore.Mvc;
using news.application.Contracts.DTO;
using news.application.Exceptions;
//using news.application.Contracts.DTOs.NewsArticleDTOs;
//using news.application.Contracts.DTOs.ParamsDTOs;
//using news.application.Contracts.DTOs.UserDTOs;
using news.application.Services;
using System.ComponentModel.DataAnnotations;
//using news.domain.Models.Aggregates.NewsArticle;

namespace news.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsArticlesController : ControllerBase
    {
        INewsArticleService _newsArticleServices;

        public NewsArticlesController(INewsArticleService newsArticleServices)
        {
            _newsArticleServices = newsArticleServices;
        }


        [HttpGet]
        public async Task<ActionResult<List<NewsArticlePreviewDTO>>> GetPreviewOfAllNewsByQueryAsync([FromQuery] NewsArticlePreviewQueryParams QueryParams, CancellationToken cancellationToken)
        {
            bool canSeeFuture = false;

            return Ok(await _newsArticleServices.GetPreviewOfAllNewsByQueryAsync(QueryParams, canSeeFuture, cancellationToken));
        }
        [HttpGet("Admin")]
        public async Task<ActionResult<List<NewsArticlePreviewDTO>>> GetPreviewOfAllNewsByQueryAdminAsync([FromQuery] NewsArticlePreviewQueryParams QueryParams, CancellationToken cancellationToken)
        {
            bool canSeeFuture = true;

            return Ok(await _newsArticleServices.GetPreviewOfAllNewsByQueryAsync(QueryParams, canSeeFuture, cancellationToken));
        }

        [HttpGet("{newsArticleId}", Name = nameof(GetNewsArticleByIdAsync))]
        public async Task<ActionResult<NewsArticleDTO>> GetNewsArticleByIdAsync(Guid newsArticleId, CancellationToken cancellationToken)
        {
            bool canSeeFuture = User.IsInRole("Admin");

            return Ok(await _newsArticleServices.GetNewsArticleByIdAsync(newsArticleId, canSeeFuture, cancellationToken));

        }

        [HttpPost]
        public async Task<ActionResult<NewsArticleDTO>> CreateNewsArticleAsync(NewsArticleCreationDTO newsArticleDTO, CancellationToken cancellationToken)
        {
            var result = await _newsArticleServices.CreateNewsArticleAsync(newsArticleDTO, cancellationToken);

            return CreatedAtRoute(nameof(GetNewsArticleByIdAsync), new { newsArticleId = result.Id, cancellationToken }, result);
        }
        [HttpPut("{newsArticleId}")]
        public async Task<ActionResult<NewsArticleDTO>> UpdateNewsArticleAsync(Guid newsArticleId, NewsArticleCreationDTO updateDTO, CancellationToken cancellationToken)
        {
            NewsArticleDTO newsArticleDTO = await _newsArticleServices.UpdateNewsArticleAsync(newsArticleId, updateDTO, cancellationToken);
            return CreatedAtRoute(nameof(GetNewsArticleByIdAsync), new { newsArticleId = newsArticleDTO.Id, cancellationToken }, newsArticleDTO);
        }

        [HttpDelete("{newsArticleId}")]
        public async Task<IActionResult> DeleteNewsArticleAsync(Guid newsArticleId, CancellationToken cancellationToken)
        {
            await _newsArticleServices.DeleteNewsArticleAsync(newsArticleId, cancellationToken);

            return NoContent();
        }

        [HttpPost("{newsArticleId}/MultiMedia")]
        public async Task<ActionResult<NewsArticleDTO>> AddMultiMediaToNewsArticleAsync(Guid newsArticleId, /*[FromForm]*/ IFormFile formFile, CancellationToken cancellationToken, bool isThumbnail = false)
        {
            // we could make this multi threaded so all the checkings are done while the streaming is happening and if checks fail i send a cancellation token to streaming and if not i wait for it 
            /* 
             * probably needs thread safe adn disposal checks and
             * maybe should be done by a new threaad and return
             * a success result with and event or sth
            */
            //todo: check the format here to stop the rest of the process if the format is not valid
            //todo: check the filename here to stop the rest of the process if the format is not valid
            //todo: check the newsexistence id here to stop the rest of the process if the format is not valid
            if (formFile == null)
            {
                return BadRequest("No file uploaded.");
            }


            using (var memoryStream = new MemoryStream())
            {

                await formFile.CopyToAsync(memoryStream);

                string fileName = formFile.FileName;
                NewsArticleDTO result;
                try
                {
                    result = await _newsArticleServices.AddMultiMediaToNewsArticleAsync(newsArticleId, memoryStream, fileName, formFile.ContentType, cancellationToken, isThumbnail);
                    HttpContext.Response.Headers.Add("X-file-Uploading", $"file with name {fileName} was uploaded for news article with id {newsArticleId}");
                }
                catch (NewsApplicationFileAlreadyExistsException ex)
                {
                    HttpContext.Response.Headers.Add("X-file-Uploading", $"file with name {fileName} already existed for news article with id {newsArticleId} and uploading was not needed");
                    result = (NewsArticleDTO)ex.Data["NewsArticleDTO"];
                }

                return CreatedAtRoute(nameof(GetNewsArticleByIdAsync), new { newsArticleId, cancellationToken }, result);

            }

        }

        [HttpPost("{newsArticleId}/MultiMediaByUrl")]
        public async Task<IActionResult> AddMultiMediaToNewsArticleAsyncByUrl(Guid newsArticleId, string url, bool isThumbnail,string type)
        {
            await _newsArticleServices.AddMultiMediaToNewsArticleAsyncByUrl(newsArticleId, isThumbnail, url, type);
            return NoContent();
        }
        [HttpPost("{newsArticleId}/Viewers")]
        public async Task<ActionResult<NewsArticleDTO>> AddViewerToNewsArticleAsync(Guid newsArticleId, [FromQuery] Guid viewerId, CancellationToken cancellationToken)
        {
            NewsArticleDTO result;
            try
            {
                result = await _newsArticleServices.AddViewerToNewsArticleAsync(newsArticleId, viewerId, cancellationToken);
                HttpContext.Response.Headers.Add("X-Resource-Creation", $"viewer with id {viewerId} was successfully added to news article with id {newsArticleId}");
            }
            catch (NewsApplicationResourceAlreadyExistsException ex)
            {
                result = (NewsArticleDTO)ex.Data["NewsArticleDTO"];
                HttpContext.Response.Headers.Add("X-Resource-Creation", $"viewer with id {viewerId} was already added to news article with id {newsArticleId} before and new resource was not created");
            }

            return CreatedAtRoute(nameof(GetNewsArticleByIdAsync), new { newsArticleId, cancellationToken }, result);
        }

        //[HttpPost("Message")]
        //public async Task<IActionResult> MessageNewsArticles(MessageNewsRequestDTO requestDTO, CancellationToken cancellationToken = default)
        //{
        //    await _newsArticleServices.MessageNewsArticles(requestDTO.NewsArticleIds, requestDTO.PhoneNumbers, cancellationToken);
        //    return NoContent();
        //}

    }
}
