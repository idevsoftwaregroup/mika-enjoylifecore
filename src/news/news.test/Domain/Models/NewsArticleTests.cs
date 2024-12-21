using news.domain.Exceptions;
using news.domain.Models;

namespace news.test.Domain.Models
{
    public class NewsArticleTests
    {
        string validTitle = "titl0";
        string validDescription = "description0";
        string validTextcontent = "text content 0 usually is long but maybe not too long to be a document then i will have to put it with multimedia logic";
        Guid buildingId = Guid.NewGuid();


        [Theory]
        [InlineData("", "", "")]
        [InlineData("   ", "   ", "   ")]
        [InlineData(null, null, null)]
        [InlineData("too long test too long test too long test too long test too long test",
            "too long test too long test too long test too long test too long test long test long test long test",
            "too long test too long test too long test too long test too long test long " +
            "test long test long test too long test too long test too long test too long test too long " +
            "test long test long test long test too long test too long test too long test too long test " +
            "too long test long test long test long test too long test too long test too long test too long " +
            "test too long test long test long test long test too long test too long test too long test too long test too long test long test long test long test too long test too long test too long test too long test too long test long test long test long test too long test too long test too long test too long test too long test long test long test long test too long test too long test too long test too long test too long test long test long test long test too long test too long test too long test too long test too long test long test long test long test too long test too long test too long test too long test too long test long test long test long test")]

        public void ValidationCheck_TextPropertiesAndDefaultValuesValidation_ThrowsException(string title, string description, string textContent)
        {
            //Arrange
            NewsArticle newsArticle;
            DateTime dateTime = DateTime.Now;
            Guid id = Guid.NewGuid();

            //Act
            newsArticle = new NewsArticle(id) // i know its runnning this every time :(
            {
                Title = validTitle,
                Description = validDescription,
                BuildingId = buildingId,
                TextContent = validTextcontent,
                PublishDate = dateTime
            };

            //Act and Assert

            Assert.NotNull(newsArticle);
            Assert.Equal(NewsTagType.ETC, newsArticle.NewsTag);
            Assert.False(newsArticle.Important);
            if (title is null)
                Assert.Throws<NullReferenceException>(() =>
                { // it is ok but i would likeit to change so i can send 422 status code respond
                    newsArticle = new NewsArticle(id)
                    {
                        Title = title,
                        Description = validDescription,
                        BuildingId = buildingId,
                        TextContent = validTextcontent,
                        PublishDate = dateTime
                    };
                });
            else
                Assert.Throws<NewsDomainValidationException>(() =>
                {
                    newsArticle = new NewsArticle(id)
                    {
                        Title = title,
                        Description = validDescription,
                        BuildingId = buildingId,
                        TextContent = validTextcontent,
                        PublishDate = dateTime
                    };
                });

            if (description is null)
                Assert.Throws<NullReferenceException>(() =>
                { // it is ok but i would likeit to change so i can send 422 status code respond
                    newsArticle = new NewsArticle(id)
                    {
                        Title = validTitle,
                        Description = description,
                        BuildingId = buildingId,
                        TextContent = validTextcontent,
                        PublishDate = dateTime
                    };
                });
            else
                Assert.Throws<NewsDomainValidationException>(() =>
                {
                    newsArticle = new NewsArticle(id)
                    {
                        Title = validTitle,
                        Description = description,
                        BuildingId = buildingId,
                        TextContent = validTextcontent,
                        PublishDate = dateTime
                    };
                });

            if (textContent is null)
                Assert.Throws<NullReferenceException>(() =>
                { // it is ok but i would likeit to change so i can send 422 status code respond
                    newsArticle = new NewsArticle(id)
                    {
                        Title = validTitle,
                        Description = validDescription,
                        BuildingId = buildingId,
                        TextContent = textContent,
                        PublishDate = dateTime
                    };
                });
            else
                Assert.Throws<NewsDomainValidationException>(() =>
                {
                    newsArticle = new NewsArticle(id)
                    {
                        Title = validTitle,
                        Description = validDescription,
                        BuildingId = buildingId,
                        TextContent = textContent,
                        PublishDate = dateTime
                    };
                });
        }

        public void EqulityCheck_EntityEqualityById_ReturnsTrueOrFalse()
        {
            //Arrange
            Guid id0 = Guid.NewGuid();
            Guid id1 = Guid.NewGuid();
            while (id0.Equals(id1)) id1 = Guid.NewGuid();
            DateTime dateTime = DateTime.Now;

            NewsArticle newsArticle0 = new NewsArticle(id0) // i know its runnning this every time :(
            {
                Title = "title0",
                Description = "description0",
                BuildingId = buildingId,
                TextContent = validTextcontent,
                PublishDate = dateTime
            };
            NewsArticle newsArticle1 = new NewsArticle(id1) // i know its runnning this every time :(
            {
                Title = "title1",
                Description = "description1",
                BuildingId = buildingId,
                TextContent = validTextcontent,
                PublishDate = dateTime
            };
            NewsArticle newsArticle2 = new NewsArticle(id0) // i know its runnning this every time :(
            {
                Title = "title2",
                Description = "description2",
                BuildingId = buildingId,
                TextContent = validTextcontent,
                PublishDate = dateTime
            };

            //Act
            var resultFalse = newsArticle0.Equals(newsArticle1);
            var resultTrue = newsArticle0.Equals(newsArticle2);

            //Assert
            Assert.True(resultTrue);
            Assert.False(newsArticle0.Id.Equals(newsArticle1.Id));
            Assert.True(newsArticle0.Id.Equals(newsArticle2.Id));
            Assert.False(resultFalse);


        }
        [Fact]
        public void NewsTagType_Validation_ThrowsExceptionOrCreates()
        {
            NewsArticle newsArticle0 = new NewsArticle(Guid.NewGuid()) // i know its runnning this every time :(
            {
                Title = "title0",
                Description = "description0",
                BuildingId = buildingId,
                TextContent = validTextcontent,
                PublishDate = DateTime.Now
            };

            Assert.Throws<NewsDomainValidationException>(() =>
            {
                newsArticle0.NewsTag = (NewsTagType)15;
            });
            Assert.Throws<NewsDomainValidationException>(() =>
            {
                newsArticle0.NewsTag = (NewsTagType)(-1);
            });
            Assert.Throws<NewsDomainValidationException>(() =>
            {
                new NewsArticle(Guid.NewGuid()) // i know its runnning this every time :(
                {
                    Title = "title0",
                    Description = "description0",
                    BuildingId = buildingId,
                    TextContent = validTextcontent,
                    PublishDate = DateTime.Now,
                    NewsTag = (NewsTagType)15
                };
            });

            newsArticle0.NewsTag = 0;
            Assert.Equal(NewsTagType.ETC, newsArticle0.NewsTag);

        }
    }
}
