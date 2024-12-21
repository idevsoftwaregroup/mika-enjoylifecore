using Microsoft.EntityFrameworkCore;
using news.application.Contracts.DTO;
using news.domain.Models;
using news.infrastructure.Data;
using news.infrastructure.Repositories;

namespace news.test.Infrastructure.Respositories
{

    public class NewsRepositoryTests
    {
        private readonly DbContextOptions<NewsContext> _options;
        string validTitle = "titl0";
        string validDescription = "description0";
        string validTextcontent = "text content 0 usually is long but maybe not too long to be a document then i will have to put it with multimedia logic";

        public NewsRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<NewsContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryTestDB")
                //.UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=NewsLaptopTestDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False")
                .Options;


        }

        //[Fact]
        //public async Task BasicInfoPersistence_ValidInfo_ReturnsRequierdInfoAsync()
        //{
        //    //Arrange

        //    Guid buildingid = Guid.NewGuid();

        //    NewsArticle newsArticle = new NewsArticle(Guid.NewGuid())
        //    {
        //        Title = validTitle,
        //        Description = validDescription,
        //        TextContent = validTextcontent,
        //        PublishDate = DateTime.Now,
        //        BuildingId = buildingid

        //    };

        //    //_dbContext.Buildings.Add(building);
        //    NewsContext dbContext = new NewsContext(_options);
        //    dbContext.NewsArticles.Add(newsArticle);
        //    dbContext.SaveChanges();
        //    NewsRepository repository = new NewsRepository(dbContext);

        //    //Act
        //    NewsArticle result = await repository.GetNewsArticleByIdAsync(newsArticle.Id, CancellationToken.None);

        //    //Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(result.Id, newsArticle.Id);
        //    Assert.Equal(result.Title, newsArticle.Title);
        //    Assert.Equal(result.Description, newsArticle.Description);
        //    Assert.Equal(result.PublishDate, newsArticle.PublishDate);
        //    Assert.Equal(result.BuildingId, buildingid);

        //    dbContext.Dispose();
        //}

        //[Fact]
        //public async Task GetNewsArticlesByQueryAsync_ValidQueries_ReturnsList()
        //{
        //    //Arrange
        //    NewsContext dbContext = new NewsContext(_options);
        //    Guid buildingid0 = Guid.NewGuid();
        //    Guid buildingid1 = Guid.NewGuid();
        //    DateTime dateTime = DateTime.Now;
        //    NewsArticle[] articles = new NewsArticle[15];
        //    for (int i = 0; i < 10; i++)
        //    {
        //        articles[i] = new NewsArticle(Guid.NewGuid())
        //        {
        //            Title = $"title{i}",
        //            Description = validDescription,
        //            TextContent = validTextcontent,
        //            PublishDate = dateTime.AddDays(i),
        //            BuildingId = buildingid0,
        //            Important = i % 3 == 0 ? true : false,
        //            NewsTag = (NewsTagType)(i % 5),

        //        };
        //    }


        //    for (int i = 10; i < 15; i++)
        //    {
        //        articles[i] = new NewsArticle(Guid.NewGuid())
        //        {
        //            Title = $"title{i}",
        //            Description = validDescription,
        //            TextContent = validTextcontent,
        //            PublishDate = dateTime.AddDays(i),
        //            BuildingId = buildingid1,
        //            Important = i % 3 == 0 ? true : false,
        //            NewsTag = (NewsTagType)(i % 5),

        //        };
        //    }
        //    dbContext.AddRange(articles);
        //    dbContext.SaveChanges();
        //    NewsRepository newsRepository = new NewsRepository(dbContext);
        //    NewsArticlePreviewQueryParams queryParams0 = new NewsArticlePreviewQueryParams()
        //    {
        //        BuildingId = buildingid0,
        //        PageSize = NewsArticlePreviewQueryParams.MAX_PAGE_SIZE
        //    };
        //    NewsArticlePreviewQueryParams queryParams1 = new NewsArticlePreviewQueryParams()
        //    {
        //        BuildingId = buildingid0,
        //        FromDate = dateTime.AddDays(1),
        //        PageSize = NewsArticlePreviewQueryParams.MAX_PAGE_SIZE
        //    };
        //    NewsArticlePreviewQueryParams queryParams2 = new NewsArticlePreviewQueryParams()
        //    {
        //        BuildingId = buildingid0,
        //        FromDate = dateTime.AddDays(1),
        //        PageSize = NewsArticlePreviewQueryParams.MAX_PAGE_SIZE,
        //        Important = true
        //    };
        //    //Act
        //    var result0 = await newsRepository.GetNewsArticlesByQueryAsync(queryParams0,true, CancellationToken.None);
        //    var result1 = await newsRepository.GetNewsArticlesByQueryAsync(queryParams1,true, CancellationToken.None);
        //    var result2 = await newsRepository.GetNewsArticlesByQueryAsync(queryParams2,true, CancellationToken.None);

        //    //Assert
        //    Assert.NotNull(result0);
        //    Assert.NotNull(result1);
        //    Assert.NotNull(result2);
        //    Assert.Equal(10, result0.Count);
        //    Assert.Equal(9, result1.Count);

        //    for (int i = 0; i < 10; i++)
        //        Assert.Contains(articles[i], result0);

        //    for (int i = 1; i < 10; i++)
        //        Assert.Contains(articles[i], result1);
        //    Assert.DoesNotContain(articles[0], result1);

        //    foreach (var article in articles)
        //    {
        //        if (article.Important && article.PublishDate >= dateTime.AddDays(1) && article.BuildingId.Equals(buildingid0))
        //            Assert.Contains(article, result2);
        //        else
        //            Assert.DoesNotContain(article, result2);
        //    }
        //}

        //private void SeedNewsArticles(Guid buildingId0, Guid buildingId1, NewsContext DbContext)

        #region old
        //{

        //    DateTime dateTime = DateTime.Now;
        //    for(int i = 0; i < 10; i++)
        //    {
        //        DbContext.Add(new NewsArticle(Guid.NewGuid())
        //        {
        //            Title = $"title{i}",
        //            Description = validDescription,
        //            TextContent = validTextcontent,
        //            PublishDate = dateTime.AddDays(i),
        //            BuildingId = buildingId0,
        //            Important = i%3==0 ? true : false,
        //            NewsTag = (NewsTagType) (i%4),

        //        }  );;
        //    }


        //    for (int i = 10; i < 15; i++)
        //    {
        //        DbContext.Add(new NewsArticle(Guid.NewGuid())
        //        {
        //            Title = $"title{i}",
        //            Description = validDescription,
        //            TextContent = validTextcontent,
        //            PublishDate = dateTime.AddDays(i),
        //            BuildingId = buildingId1,
        //            Important = i % 3 == 0 ? true : false,
        //            NewsTag = (NewsTagType)(i % 4),

        //        });
        //    }

        //    DbContext.SaveChanges();
        //}
        //[Fact]
        //public async void GetNewsArticleByIdAsync_ExistingId_RetunrsViewerInfo()
        //{
        //    //Arrange
        //    Viewer viewer0 = new Viewer(Guid.NewGuid(), "viewr0");
        //    Viewer viewer1 = new Viewer(Guid.NewGuid(), "viewr1");

        //    Building building = new Building(Guid.NewGuid(), "buildingTitle0");

        //    NewsArticle newsArticle = new NewsArticle(Guid.NewGuid(),
        //                                                    "title0",
        //                                                    "description0",
        //                                                    "textcontent0",
        //                                                    DateTime.Now,
        //                                                    building,
        //                                                    building.Id,
        //                                                    NewsTagType.NONE);
        //    newsArticle.AddViewer(viewer0);
        //    newsArticle.AddViewer(viewer1);

        //    _dbContext.NewsArticles.Add(newsArticle);
        //    _dbContext.SaveChanges();

        //    NewsRepository repository = new NewsRepository(_dbContext);

        //    //Act
        //    NewsArticle result = await repository.GetNewsArticleByIdAsync(newsArticle.Id, CancellationToken.None);

        //    //Assert

        //    Assert.NotNull(result);
        //    Assert.Equal(result.Id, newsArticle.Id);
        //    Assert.NotEmpty(result.Viewers);
        //    Assert.Contains(viewer0,result.Viewers);
        //    Assert.Contains(viewer1,result.Viewers);
        //    Assert.Contains(viewer0.Id, result.Viewers.Select(v => v.Id).ToList());
        //    Assert.Contains(viewer1.Id, result.Viewers.Select(v => v.Id).ToList());
        //    Assert.Contains(viewer0.Name, result.Viewers.Select(v => v.Name).ToList());
        //    Assert.Contains(viewer1.Name, result.Viewers.Select(v => v.Name).ToList());


        //}

        //[Fact]
        //public async Task GetNewsArticleByIdAsync_ExistingId_RetunrsMultiMediaInfoAsync()
        //{
        //    //Arrange
        //    MultiMediaContent multiMediaContent0 = new MultiMediaContent(Guid.NewGuid(), "url0",
        //                                                                MultiMediaContent.validFormatTypes[0],
        //                                                                200);
        //     MultiMediaContent multiMediaContent1 = new MultiMediaContent(Guid.NewGuid(), "url1",
        //                                                                MultiMediaContent.validFormatTypes[0],
        //                                                                200);
        //     MultiMediaContent thumbnail = new MultiMediaContent(Guid.NewGuid(), "url2",
        //                                                                MultiMediaContent.validFormatTypes[0],
        //                                                                200);

        //    Building building = new Building(Guid.NewGuid(), "buildingTitle0");

        //    NewsArticle newsArticle = new NewsArticle(Guid.NewGuid(),
        //                                                    "title0",
        //                                                    "description0",
        //                                                    "textcontent0",
        //                                                    DateTime.Now,
        //                                                    building,
        //                                                    building.Id,
        //                                                    NewsTagType.NONE);

        //    newsArticle.AddMultiMediaContent(multiMediaContent0);
        //    newsArticle.AddMultiMediaContent(multiMediaContent1);

        //    newsArticle.Thumbnail = thumbnail;


        //    _dbContext.NewsArticles.Add(newsArticle);
        //    _dbContext.SaveChanges();

        //    NewsRepository repository = new NewsRepository(_dbContext);

        //    //Act
        //    NewsArticle result = await repository.GetNewsArticleByIdAsync(newsArticle.Id, CancellationToken.None);

        //    //Assert

        //    Assert.NotNull(result);
        //    Assert.Equal(result.Id, newsArticle.Id);
        //    Assert.NotEmpty(result.MultiMediaContents);
        //    Assert.Contains(multiMediaContent0, result.MultiMediaContents);
        //    Assert.Contains(multiMediaContent1, result.MultiMediaContents);
        //    Assert.Contains(multiMediaContent0.Id, result.MultiMediaContents.Select(v => v.Id).ToList());
        //    Assert.Contains(multiMediaContent1.Id, result.MultiMediaContents.Select(v => v.Id).ToList());
        //    Assert.Contains(multiMediaContent0.URL, result.MultiMediaContents.Select(v => v.URL).ToList());
        //    Assert.Contains(multiMediaContent1.URL, result.MultiMediaContents.Select(v => v.URL).ToList());
        //    Assert.NotNull(result.Thumbnail);
        //    Assert.Equal(result.Thumbnail, thumbnail);
        //    Assert.Equal(result.Thumbnail.Id, thumbnail.Id);
        //    Assert.Equal(result.Thumbnail.URL, thumbnail.URL);

        //}

        //[Fact]
        //public async void CreateNewsArticleAsync_ValidCreation_PersistsRequiredInfo()
        //{
        //    //Arrange
        //    Building building = new Building(Guid.NewGuid(), "buildingTitle0");

        //    NewsArticle newsArticle = new NewsArticle(Guid.NewGuid(),
        //                                                    "title0",
        //                                                    "description0",
        //                                                    "textcontent0",
        //                                                    DateTime.Now,
        //                                                    building,
        //                                                    building.Id,
        //                                                    NewsTagType.NONE);

        //    NewsRepository repository = new NewsRepository(_dbContext);

        //    //Act
        //    var resultOfCreation = await repository.CreateNewsArticleAsync(newsArticle,CancellationToken.None);

        //    var inDBArticle = _dbContext.NewsArticles.Where(x=>x.Id==newsArticle.Id).SingleOrDefault();

        //    //Assert
        //    Assert.NotNull(resultOfCreation);
        //    Assert.NotNull(inDBArticle);
        //    Assert.Equal(newsArticle, inDBArticle);
        //}
        #endregion

    }
}
