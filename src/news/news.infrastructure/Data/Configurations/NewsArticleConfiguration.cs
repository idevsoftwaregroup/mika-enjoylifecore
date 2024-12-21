using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using news.domain.Models;
//using news.domain.Models.Aggregates.NewsArticle;

namespace news.infrastructure.Data.Configurations
{
    public class NewsArticleConfiguration : IEntityTypeConfiguration<NewsArticle>
    {
        public void Configure(EntityTypeBuilder<NewsArticle> builder)
        {
            builder.ToTable("NewsArticles");
            builder.HasKey(x => x.Id);


            builder.OwnsOne(x => x.Thumbnail);
            builder.OwnsMany(x => x.MultiMedias);
            builder.OwnsMany(x => x.Viewers);



            //builder.Property(na => na.Title).HasMaxLength(20).IsRequired();
            //builder.Property(na => na.Description).HasMaxLength(200).IsRequired();

            //builder.OwnsOne(na => na.Thumbnail, mb =>
            //{
            //    mb.ToTable("MultiMediaContents");
            //    mb.HasKey(x => x.Id);
            //    mb.Property(x => x.Size).HasMaxLength(1000);
            //    mb.OwnsOne(m => m.BaseEntityReports, berb =>
            //    {
            //        berb.Property(ber => ber.IsDeleted).HasColumnName("MultiMedia_IsDeleted");
            //        berb.Property(ber => ber.CreatedDate).HasColumnName("MultiMedia_CreatedDate");
            //        berb.Property(ber => ber.CreatedBy).HasColumnName("MultiMedia_CreatedBy");
            //        berb.Property(ber => ber.ModifyDate).HasColumnName("MultiMedia_ModifyDate");
            //        berb.Property(ber => ber.ModifyBy).HasColumnName("MultiMedia_ModifyBy");
            //    });
            //    //mb.WithOwner().HasForeignKey("Thumbnail_NewsArticleId");

            //});


            //builder.OwnsMany(na => na.MultiMediaContents, mb =>
            //{
            //    mb.OwnsOne(m => m.BaseEntityReports, berb =>
            //    {
            //        berb.Property(ber => ber.IsDeleted).HasColumnName("MultiMedia_IsDeleted");
            //        berb.Property(ber => ber.CreatedDate).HasColumnName("MultiMedia_CreatedDate");
            //        berb.Property(ber => ber.CreatedBy).HasColumnName("MultiMedia_CreatedBy");
            //        berb.Property(ber => ber.ModifyDate).HasColumnName("MultiMedia_ModifyDate");
            //        berb.Property(ber => ber.ModifyBy).HasColumnName("MultiMedia_ModifyBy");
            //    });
            //    //mb.WithOwner().HasForeignKey()
            //});

            //builder.OwnsMany(na => na.Viewers);

            //builder.HasOne(na => na.Thumbnail).WithMany();
            //builder.HasMany(na => na.MultiMediaContents).WithMany();
            //builder.HasOne(na => na.Building).WithMany().HasForeignKey(na => na.BuildingId);
        }
    }
}
