using BaseSource.Data.Entities;
using BaseSource.Shared.Constants;
using BaseSource.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Data.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Project");
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.ApiKey).IsUnique();
            builder.HasIndex(p => p.SecretKey).IsUnique();

            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(255).UseCollation(SQL_CONST.Collation);
            builder.Property(p => p.ApiKey).IsRequired().HasMaxLength(128);
            builder.Property(p => p.SecretKey).IsRequired().HasMaxLength(128);
            builder.Property(p => p.WebhookUrl).HasMaxLength(500);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(128);

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Headers)
                .WithOne(h => h.Project)
                .HasForeignKey(h => h.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Wallets)
                .WithOne(w => w.Project)
                .HasForeignKey(w => w.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class WebhookHeaderConfiguration : IEntityTypeConfiguration<WebhookHeader>
    {
        public void Configure(EntityTypeBuilder<WebhookHeader> builder)
        {
            builder.ToTable("WebhookHeader");
            builder.HasKey(h => h.Id);

            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(h => h.Key).IsRequired().HasMaxLength(128);
            builder.Property(h => h.Value).IsRequired().HasMaxLength(500);
        }
    }

    public class CryptoWalletConfiguration : IEntityTypeConfiguration<CryptoWallet>
    {
        public void Configure(EntityTypeBuilder<CryptoWallet> builder)
        {
            builder.ToTable("CryptoWallet");
            builder.HasKey(w => w.Id);

            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(w => w.Address).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Chain).HasConversion(new EnumToStringConverter<EChain>()).HasMaxLength(128);

            builder.HasMany(w => w.Transactions)
                .WithOne(t => t.CryptoWallet)
                .HasForeignKey(t => t.CryptoWalletId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class BlockchainTransactionConfiguration : IEntityTypeConfiguration<BlockchainTransaction>
    {
        public void Configure(EntityTypeBuilder<BlockchainTransaction> builder)
        {
            builder.ToTable("BlockchainTransaction");
            builder.HasKey(t => t.Id);
            builder.HasIndex(t => t.Time);
            builder.HasIndex(t => t.TxHash).IsUnique();

            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(t => t.Amount).HasColumnType("decimal(38,18)");
            builder.Property(t => t.TxHash).IsRequired().HasMaxLength(128);
            builder.Property(t => t.TokenSymbol).HasMaxLength(20);
            builder.Property(t => t.FromAddress).HasMaxLength(128);
            builder.Property(t => t.ToAddress).HasMaxLength(128);

            builder.HasMany(t => t.WebhookHistories)
                .WithOne(w => w.BlockchainTransaction)
                .HasForeignKey(w => w.BlockchainTransactionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class WebhookHistoryConfiguration : IEntityTypeConfiguration<WebhookHistory>
    {
        public void Configure(EntityTypeBuilder<WebhookHistory> builder)
        {
            builder.ToTable("WebhookHistory");
            builder.HasKey(w => w.Id);

            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(w => w.ResponseBody).HasMaxLength(2000);
            builder.Property(w => w.SentAt).IsRequired();
        }
    }
}
