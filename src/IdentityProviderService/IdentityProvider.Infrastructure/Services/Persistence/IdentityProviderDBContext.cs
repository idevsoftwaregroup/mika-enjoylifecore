using IdentityProvider.Domain.Models.EnjoyLifeRole;
using IdentityProvider.Domain.Models.EnjoyLifeUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityProvider.Infrastructure.Services.Persistence;

public class EnjoyLifeIdentityDbContext : IdentityDbContext<EnjoyLifeUser, EnjoyLifeRole, int>
{
    public EnjoyLifeIdentityDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        
        builder.Entity<EnjoyLifeUser>().HasIndex(u => u.PhoneNumber).IsUnique();
        
        base.OnModelCreating(builder);
    }
}
