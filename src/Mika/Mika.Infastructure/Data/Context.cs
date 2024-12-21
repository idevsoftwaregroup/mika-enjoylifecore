using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Mika.Domain.Entities;
using Mika.Domain.Entities.RelationalEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Infastructure.Data
{
    public class Context : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<SubModule> SubModules { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<BiDataEntry> BiDataEntries { get; set; }
        public DbSet<HistoryLog> HistoryLogs { get; set; }
        public DbSet<HistoryLogDetail> HistoryLogDetails { get; set; }
        public DbSet<F_UserAccount> F_UserAccounts { get; set; }
        public DbSet<F_UserSubModule> F_UserSubModules { get; set; }
        public Context()
        {
        }
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(role =>
            {
                role.Property(x => x.RoleName).IsRequired();
                role.Property(x => x.DisplayName).IsRequired();
                role.HasMany(x => x.Users).WithOne(x => x.Role).HasForeignKey(x => x.RoleId);
            });

            modelBuilder.Entity<Module>(module =>
            {
                module.Property(x => x.ModuleName).IsRequired();
                module.Property(x => x.DisplayName).IsRequired();
                module.HasMany(x => x.HistoryLogs).WithOne(x => x.Module).HasForeignKey(x => x.ModuleId);
                module.HasMany(x => x.SubModules).WithOne(x => x.Module).HasForeignKey(x => x.ModuleId);
            });

            modelBuilder.Entity<SubModule>(subModule =>
            {
                subModule.Property(x => x.SubModuleName).IsRequired();
                subModule.Property(x => x.SubDisplayName).IsRequired();
                subModule.HasMany(x => x.F_UserSubModules).WithOne(x => x.SubModule).HasForeignKey(x => x.SubModuleId);
            });


            modelBuilder.Entity<User>(user =>
            {
                user.Property(x => x.UserName).IsRequired();
                user.Property(x => x.Password).IsRequired();
                user.Property(x => x.Active).IsRequired();
                user.Property(x => x.Name).IsRequired();
                user.Property(x => x.LastName).IsRequired(false);
                user.Property(x => x.PhoneNumber).IsRequired(false);
                user.Property(x => x.Email).IsRequired(false);
                user.Property(x => x.Position).IsRequired(false);
                user.Property(x => x.PersonnelNumber).IsRequired(false);
                user.Property(x => x.Avatar).IsRequired(false);
                user.Property(x => x.CreationDate).IsRequired();
                user.Property(x => x.LastModificationDate).IsRequired(false);
                user.Property(x => x.LastModifier).IsRequired(false);
                user.Property(x => x.PrimaryParentId).IsRequired(false);
                user.Property(x => x.SecodaryParentIDs).IsRequired(false);
                user.HasMany(x => x.F_UserAccounts).WithOne(x => x.User).HasForeignKey(x => x.UserId);
                user.HasMany(x => x.F_UserSubModules).WithOne(x => x.User).HasForeignKey(x => x.UserId);
                user.HasMany(x => x.BiDataEntries).WithOne(x => x.Creator).HasForeignKey(x => x.CreatorId);
                user.HasMany(x => x.HistoryLogs).WithOne(x => x.Logger).HasForeignKey(x => x.LoggerId);
            });

            modelBuilder.Entity<Company>(company =>
            {
                company.Property(x => x.CompanyNumber).IsRequired();
                company.Property(x => x.CompanyCode).IsRequired();
                company.Property(x => x.Name).IsRequired();
                company.Property(x => x.CreationDate).IsRequired();
                company.Property(x => x.LastModificationDate).IsRequired(false);
                company.Property(x => x.LastModifier).IsRequired(false);
                company.HasMany(x => x.Accounts).WithOne(x => x.Company).HasForeignKey(x => x.CompanyId);
            });

            modelBuilder.Entity<Account>(account =>
            {
                account.Property(x => x.AccountNumber).IsRequired();
                account.Property(x => x.BankName).IsRequired();
                account.Property(x => x.AccountType).IsRequired(false);
                account.Property(x => x.InterestRatePercent).IsRequired();
                account.Property(x => x.ProjectName).IsRequired();
                account.Property(x => x.BelongsToCentralOffice).IsRequired(false);
                account.Property(x => x.Description).IsRequired(false);
                account.Property(x => x.CreationDate).IsRequired();
                account.Property(x => x.LastModificationDate).IsRequired(false);
                account.Property(x => x.LastModifier).IsRequired(false);
                account.HasMany(x => x.F_UserAccounts).WithOne(x => x.Account).HasForeignKey(x => x.AccountId);
                account.HasMany(x => x.BiDataEntries).WithOne(x => x.Account).HasForeignKey(x => x.AccountId);
            });

            modelBuilder.Entity<BiDataEntry>(biDataEntry =>
            {
                biDataEntry.Property(x => x.EntryDateTime).IsRequired();
                biDataEntry.Property(x => x.CreationDate).IsRequired();
                biDataEntry.Property(x => x.LastModificationDate).IsRequired(false);
            });

            modelBuilder.Entity<HistoryLog>(historyLog =>
            {
                historyLog.Property(x => x.EntityName).IsRequired();
                historyLog.Property(x => x.EntityId).IsRequired();
                historyLog.Property(x => x.LogDate).IsRequired();
                historyLog.Property(x => x.Key).IsRequired();
                historyLog.Property(x => x.Value).IsRequired(false);
                historyLog.HasMany(x => x.HistoryLogDetails).WithOne(x => x.HistoryLog).HasForeignKey(x => x.HistoryLogId);
            });

            modelBuilder.Entity<HistoryLogDetail>(historyLogDetail =>
            {
                historyLogDetail.Property(x => x.Key).IsRequired();
                historyLogDetail.Property(x => x.Value).IsRequired(false);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
