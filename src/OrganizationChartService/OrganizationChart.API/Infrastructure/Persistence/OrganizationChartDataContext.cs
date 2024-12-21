using Microsoft.EntityFrameworkCore;
using OrganizationChart.API.Models;

namespace OrganizationChart.API.Infrastructure.Persistence;

public class OrganizationChartDataContext : DbContext
{
    public OrganizationChartDataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }

    public DbSet<SystemType> SystemTypes { get; set; }
    public DbSet<Systems> Systems { get; set; }
    public DbSet<SystemGraphic> SystemGraphics { get; set; }
    public DbSet<SystemHard> SystemHards { get; set; }

    public DbSet<SystemProject> SystemProjects { get; set; }
    public DbSet<SystemUPS> SystemUPSes { get; set; }
    public DbSet<SystemUpsService> SystemUpsServices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().HasKey(e => e.Id);
        modelBuilder.Entity<Department>().HasKey(d => d.Id);
        modelBuilder.Entity<Employee>().HasOne(e => e.Department);//.Property(e => e.Department)
                                                                  //modelBuilder.Entity<Department>().HasIndex(d => d.Name).IsUnique();


        #region System

        modelBuilder.Entity<SystemType>().Property(x => x.Name).IsRequired();
        modelBuilder.Entity<SystemType>().HasMany(x => x.Systems).WithOne(x => x.Type).HasForeignKey(x => x.SystemTypeId);

        modelBuilder.Entity<Systems>().Property(x => x.ComputerName).IsRequired();
        modelBuilder.Entity<Systems>().Property(x => x.User).IsRequired();
        modelBuilder.Entity<Systems>().Property(x => x.OS).IsRequired();
        modelBuilder.Entity<Systems>().Property(x => x.RAM).IsRequired();
        modelBuilder.Entity<Systems>().Property(x => x.MainBoardName).IsRequired();
        modelBuilder.Entity<Systems>().Property(x => x.MainBoardModel).IsRequired();
        modelBuilder.Entity<Systems>().Property(x => x.CPU).IsRequired();
        modelBuilder.Entity<Systems>().Property(x => x.Description).IsRequired(false);
        modelBuilder.Entity<Systems>().Property(x => x.Location).IsRequired(false);
        modelBuilder.Entity<Systems>().Property(x => x.SystemPicUrl).IsRequired(false);
        modelBuilder.Entity<Systems>().Property(x => x.UserPicUrl).IsRequired(false);

        modelBuilder.Entity<Systems>().HasMany(x => x.Graphics).WithOne(x => x.System).HasForeignKey(x => x.SystemId);
        modelBuilder.Entity<Systems>().HasMany(x => x.HardDisks).WithOne(x => x.System).HasForeignKey(x => x.SystemId);

        modelBuilder.Entity<SystemProject>(project =>
        {
            project.Property(x => x.Name).IsRequired();
            project.HasMany(x => x.UPSes).WithOne(x => x.Project).HasForeignKey(x => x.ProjectId);
        });

        modelBuilder.Entity<SystemUPS>(ups =>
        {
            ups.Property(x => x.Location).IsRequired();
            ups.Property(x => x.Model).IsRequired();
            ups.Property(x => x.PowerInKVA).IsRequired();
            ups.Property(x => x.Devices).IsRequired(false);
            ups.Property(x => x.BatteryCapacityInAH).IsRequired(false);
            ups.Property(x => x.BatteryCount).IsRequired(false);
            ups.Property(x => x.MaintenanceEnvironment).IsRequired(false);
            ups.Property(x => x.PurchaseDate).IsRequired();
            ups.Property(x => x.BatteryPurchaseDate).IsRequired(false);
            ups.Property(x => x.BatteryUsageDate).IsRequired(false);
            ups.Property(x => x.LatestServiceDate).IsRequired(false);
            ups.Property(x => x.Description).IsRequired(false);

            ups.HasMany(x => x.UpsServices).WithOne(x => x.UPS).HasForeignKey(x => x.UpsId);
        });

        modelBuilder.Entity<SystemUpsService>(service =>
        {
            service.Property(x => x.Date).IsRequired();
            service.Property(x => x.Description).IsRequired(false);
            service.Property(x => x.AttachmentUrl).IsRequired(false);
        });

        #endregion

        base.OnModelCreating(modelBuilder);
    }
}
