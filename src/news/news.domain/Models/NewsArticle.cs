using news.domain.Exceptions;

namespace news.domain.Models
{
    public class NewsArticle : BaseEntity
    {
        #region constants
        public static readonly int TITLE_MAXLENGTH = 20;
        public static readonly int DESCRIPTION_MAXLENGTH = 40;
        public static readonly int TEXTCONTENT_MAXLENGTH = 500;
        public static readonly int MAX_NUMBER_OF_PINNED_NEWS = 3;
        #endregion

        #region fields
        private string title;
        private string description;
        private string textContent;
        private NewsTagType _newsTag = NewsTagType.ETC;
        #endregion

        private NewsArticle()
        {
        }
        public NewsArticle(Guid Id) : base(Id)
        {

        }

        public Guid BuildingId { get; set; }
        public string Title
        {
            get => title; set //its throwing null pointer exception if input is null instead of domain exception
            {
                NewsDomainValidationException ex = null;
                if (string.IsNullOrWhiteSpace(value))
                {
                    ex = new NewsDomainValidationException($"news article title length cannot be more than {TITLE_MAXLENGTH} and it must not be empty ,null or whitespace");
                    ex.Data["Title IsNullOrWhiteSpace"] = $"you provided \"{value}\"";
                }
                if (value.Length > TITLE_MAXLENGTH)
                {
                    if (ex is null) ex = new NewsDomainValidationException($"news article title length cannot be more than {TITLE_MAXLENGTH} and it must not be empty ,null or whitespace");
                    ex.Data["Title length"] = value.Length;
                };
                if (ex is not null) throw ex;

                title = value;
            }
        }
        public string Description
        {
            get => description; set
            {
                NewsDomainValidationException? ex = null;
                if (string.IsNullOrWhiteSpace(value))
                {
                    ex = new NewsDomainValidationException($"news article description length cannot be more than {DESCRIPTION_MAXLENGTH} and it must not be empty ,null or whitespace");
                    ex.Data["Description IsNullOrWhiteSpace"] = $"you provided \"{value}\"";
                }
                if (value.Length > DESCRIPTION_MAXLENGTH)
                {
                    if (ex is null) ex = new NewsDomainValidationException($"news article description length cannot be more than {DESCRIPTION_MAXLENGTH} and it must not be empty ,null or whitespace");
                    ex.Data["Description Length"] = value.Length;
                }
                if (ex is not null) throw ex;

                description = value;
            }
        }
        public string TextContent
        {
            get => textContent; set
            {
                NewsDomainValidationException? ex = null;
                if (string.IsNullOrWhiteSpace(value))
                {
                    ex = new NewsDomainValidationException($"news article textContent length cannot be more than {TEXTCONTENT_MAXLENGTH} and it must not be empty ,null or whitespace");
                    ex.Data["TextContent IsNullOrWhiteSpace"] = $"you provided \"{value}\"";
                }
                if (value.Length > TEXTCONTENT_MAXLENGTH)
                {
                    if (ex is null) ex = new NewsDomainValidationException($"news article textContent length cannot be more than {TEXTCONTENT_MAXLENGTH} and it must not be empty ,null or whitespace");
                    ex.Data["TextContent Length"] = value.Length;
                }
                if (ex is not null) throw ex;

                textContent = value;
            }
        }
        public DateTime PublishDate { get; set; }
        public MultiMedia? Thumbnail { get; set; }
        public HashSet<MultiMedia> MultiMedias { get; set; }
        public bool Important { get; set; } = false;
        public NewsTagType NewsTag { get => _newsTag; set => _newsTag = Enum.IsDefined(typeof(NewsTagType), value) ? value : throw new NewsDomainValidationException($"tag must be an integer between 0 and {Enum.GetValues<NewsTagType>().Length - 1}"); }

        public bool Pinned { get; set; } = false; // this should be defined as a list of news article ids in a building model so we can control the number of pinned articles in our domain business

        public HashSet<Viewer> Viewers { get; set; }
    }


    public record Viewer(Guid ViewerId);
    //[Flags]
    public enum NewsTagType
    {
        ETC,
        EVENT,
        MANAGMENT,
        MAINTENANCE,
        CONCIERGE,
        PERFORMANCE_REPORTS
    }
    public record MultiMedia(string URL,MediaType MediaType);
    public enum MediaType
    {
        IMAGE,
        VIDEO,
        GIF
    }
}
