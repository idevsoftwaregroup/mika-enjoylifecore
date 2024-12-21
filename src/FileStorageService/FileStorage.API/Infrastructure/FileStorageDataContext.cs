using FileStorage.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.API.Infrastructure;

public class FileStorageDataContext : DbContext
{
    public FileStorageDataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<MetaData> MetaDatas { get; set; }
    //public DbSet<OriginService> OriginServices { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<MetaData>().OwnsOne<OriginService>(nameof(MetaData.OriginService));

        base.OnModelCreating(modelBuilder);
    }
}
