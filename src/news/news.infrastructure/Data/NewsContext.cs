using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using news.domain.Models;
using news.infrastructure.Data.Configurations;
//using news.domain.Models.Aggregates.Building;
//using news.domain.Models.Aggregates.NewsArticle;
//using news.domain.Models.Aggregates.NewsArticle.MultiMedia;
//using news.domain.Models.Aggregates.Party;


namespace news.infrastructure.Data
{
    public class NewsContext : DbContext // extra indexes are added (why is thumbnail indexed in NewsArticles) and multimedia is not manytomany with newsarticle
    {
        public DbSet<NewsArticle> NewsArticles { get; set; }
        public DbSet<TempNewsArticle> TempNewsArticles { get; set; }
        public DbSet<Media> Medias { get; set; }

        //public DbSet<Building> Buildings { get; set; }
        //public DbSet<User> Users { get; set; }

        public NewsContext()
        {
        }
        public NewsContext(DbContextOptions<NewsContext> options) : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    // You can use a configuration file or a hard-coded connection string here
        //    optionsBuilder.UseSqlServer("Server=46.245.66.25,1433;Database=EnjoyLifeNews;User Id=sa;Password=!@#qwe123;Encrypt=False;");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder) // how to put fk constraint on owned Many Ids ? should i even ?
        {
            modelBuilder.ApplyConfiguration(new NewsArticleConfiguration());
            //modelBuilder.ApplyConfiguration(new BuildingConfiguration());
            //modelBuilder.ApplyConfiguration(new UserConfiguration());

            #region Media

            modelBuilder.Entity<Media>().Property(x => x.Url).IsRequired();

            #endregion


            #region TempNewsArticle

            modelBuilder.Entity<TempNewsArticle>().Property(x => x.Title).IsRequired();
            modelBuilder.Entity<TempNewsArticle>().Property(x => x.Description).IsRequired(false);
            modelBuilder.Entity<TempNewsArticle>().Property(x => x.ThumbnailUrl).IsRequired(false);
            modelBuilder.Entity<TempNewsArticle>().Property(x => x.ThumbnailMediaType).IsRequired(false);
            modelBuilder.Entity<TempNewsArticle>().Property(x => x.ModificationDate).IsRequired(false);
            modelBuilder.Entity<TempNewsArticle>().Property(x => x.Modifier).IsRequired(false);
            modelBuilder.Entity<TempNewsArticle>().HasMany(x => x.Medias).WithOne(x => x.TempNewsArticle).HasForeignKey(x => x.TempNewsArticleId);

            #endregion


            base.OnModelCreating(modelBuilder);
        }

    }
}
