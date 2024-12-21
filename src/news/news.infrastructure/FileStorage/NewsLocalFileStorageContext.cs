using Microsoft.Extensions.Configuration;
using news.application.Contracts.Interfaces;
using news.application.Exceptions;
using news.domain.Models;
//using news.domain.Models.Aggregates.NewsArticle.MultiMedia;
using news.infrastructure.Exceptions;

namespace news.infrastructure.FileStorage
{
    public class NewsLocalFileStorageContext : INewsLocalFileStorageContext
    {
        private readonly string _rootDirectoryPath;

        public NewsLocalFileStorageContext(IConfiguration configuration)
        {
            //_rootDirectoryPath = configuration.GetSection("FileStorage").GetSection("RootDirectoryPath").Value ?? throw new NewsInfrastructureException("local root directory is not provided");
            //if (!Directory.Exists(_rootDirectoryPath))
            //{

            //    try
            //    {
            //        Directory.CreateDirectory(_rootDirectoryPath);
            //    }
            //    catch (Exception ex)
            //    {

            //        throw new NewsInfrastructureException("provided local root directory was invalid", ex);
            //    }
            //}
        }
        public async Task<string> SaveNewFileToNewsArticleMultiMediaStorage(Guid newsArticleId, MemoryStream memoryStream, string fileName, CancellationToken cancellationToken)
        {
            if (memoryStream == null || memoryStream.Length == 0)
            {
                throw new ArgumentException("MemoryStream is empty or null.");
            }

            var filePath = Path.Combine(_rootDirectoryPath, newsArticleId.ToString(), fileName);
            try
            {
                if (File.Exists(filePath))
                {
                    var ex = new NewsInfrastructureFileAlreadyExistsException($"file with name {fileName} already exists for news article with id {newsArticleId}");
                    ex.Data["ExistedURL"] = filePath;
                    throw ex;
                }

                Directory.CreateDirectory(Path.Combine(_rootDirectoryPath, newsArticleId.ToString())); // maybe not needed

                using (var creationStream = new FileStream(filePath, FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(creationStream, cancellationToken);
                }

            }
            catch (NewsInfrastructureFileAlreadyExistsException ex)
            {
                throw new NewsApplicationFileAlreadyExistsException(ex.Message, ex);
            }


            return filePath;

            // FileInfo fileInfo = new FileInfo(filePath);

            //    return new MultiMediaContent(multiMediaContentId, filePath, formatType, fileInfo.Length);
        }



        //public async Task<MultiMediaContent> SaveFileStreamToNewsFolder(Guid newsArticleId, Guid multiMediaContentId, string formatType, MemoryStream fileStream, CancellationToken cancellationToken)
        //{

        //    //if (!MultiMediaContent.validFormatTypes.Contains(formatType)) throw new NewsInfrastructureException($"{formatType} format type is not supported");
        //    if (formatType.Equals("image/jpeg")) formatType = ".jpg";
        //    var filePath = Path.Combine(_rootDirectoryPath, newsArticleId.ToString(), multiMediaContentId.ToString() + formatType);

        //    if (File.Exists(filePath)) throw new NewsInfrastructureException("a file with this url already exists for this NewsArticle.");

        //    Directory.CreateDirectory(Path.Combine(_rootDirectoryPath, newsArticleId.ToString()));

        //    using (var creationStream = File.Create(filePath))
        //    {
        //        await fileStream.CopyToAsync(creationStream, cancellationToken);
        //    }

        //    FileInfo fileInfo = new FileInfo(filePath);

        //    return new MultiMediaContent(multiMediaContentId, filePath, formatType, fileInfo.Length);

        //}
    }
}
