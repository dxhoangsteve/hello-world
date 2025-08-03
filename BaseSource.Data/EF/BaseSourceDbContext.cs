using BaseSource.Data.Configurations;
using BaseSource.Data.Entities;
using BaseSource.Data.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Data.EF
{
    public class BaseSourceDbContext : IdentityDbContext<AppUser, AppRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public BaseSourceDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure using Fluent API

            #region Identity
            modelBuilder.ApplyConfiguration(new AppUserConfiguration());
            modelBuilder.ApplyConfiguration(new AppRoleConfiguration());
            //modelBuilder.ApplyConfiguration(new AppRoleClaimConfiguration());

            //modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AppUserClaims").HasKey(x => x.Id);
            modelBuilder.Entity<IdentityUserClaim<string>>().Property(x => x.UserId).HasMaxLength(128);

            //modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AppUserRoles").HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<IdentityUserRole<string>>().Property(x => x.UserId).HasMaxLength(128);
            modelBuilder.Entity<IdentityUserRole<string>>().Property(x => x.RoleId).HasMaxLength(128);

            //modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AppUserLogins").HasKey(x => new { x.LoginProvider, x.ProviderKey });
            modelBuilder.Entity<IdentityUserLogin<string>>().Property(x => x.UserId).HasMaxLength(128);
            //modelBuilder.Entity<IdentityUserLogin<string>>().Property(x => x.LoginProvider).HasMaxLength(128);
            //modelBuilder.Entity<IdentityUserLogin<string>>().Property(x => x.ProviderKey).HasMaxLength(128);

            //modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AppRoleClaims").HasKey(x => x.Id);
            modelBuilder.Entity<IdentityRoleClaim<string>>().Property(x => x.RoleId).HasMaxLength(128);

            //modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AppUserTokens").HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
            modelBuilder.Entity<IdentityUserToken<string>>().Property(x => x.UserId).HasMaxLength(128);
            //modelBuilder.Entity<IdentityUserToken<string>>().Property(x => x.LoginProvider).HasMaxLength(128);

            #endregion
            
            modelBuilder.ApplyConfiguration(new SettingConfiguration());
            modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
            modelBuilder.ApplyConfiguration(new BankConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new WebhookHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new CryptoWalletConfiguration());
            modelBuilder.ApplyConfiguration(new BlockchainTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new WebhookHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new PackageConfiguration());
            modelBuilder.ApplyConfiguration(new PackageOptionConfiguration());
            modelBuilder.ApplyConfiguration(new PackageSubscriptionConfiguration());

            //Data seeding
            modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Setting> Settings { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<WebhookHeader> WebhookHeaders { get; set; }
        public DbSet<CryptoWallet> CryptoWallets { get; set; }
        public DbSet<BlockchainTransaction> BlockchainTransactions { get; set; }
        public DbSet<WebhookHistory> WebhookHistories { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<PackageOption> PackageOptions { get; set; }
        public DbSet<PackageSubscription> PackageSubscriptions { get; set; }
    }
}
