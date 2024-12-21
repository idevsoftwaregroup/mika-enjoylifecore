using Messaging.Infrastructure.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Infrastructure.Services.Persistence
{
    public class MessagingDBContext : DbContext
    {
        public MessagingDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<BulkInstanceMessage> BulkInstances { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>().OwnsMany(x => x.Recipients);

            base.OnModelCreating(modelBuilder);
        }
    }
}
