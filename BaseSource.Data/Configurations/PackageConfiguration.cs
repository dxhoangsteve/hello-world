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
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.ToTable("Package");
            builder.HasKey(p => p.Id);

            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(255).UseCollation(SQL_CONST.Collation);

            builder.HasMany(p => p.Options)
                .WithOne(o => o.Package)
                .HasForeignKey(o => o.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Subscriptions)
                .WithOne(s => s.Package)
                .HasForeignKey(s => s.PackageId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class PackageOptionConfiguration : IEntityTypeConfiguration<PackageOption>
    {
        public void Configure(EntityTypeBuilder<PackageOption> builder)
        {
            builder.ToTable("PackageOption");
            builder.HasKey(o => o.Id);

            builder.Property(x => x.Id).UseIdentityColumn();
        }
    }

    public class PackageSubscriptionConfiguration : IEntityTypeConfiguration<PackageSubscription>
    {
        public void Configure(EntityTypeBuilder<PackageSubscription> builder)
        {
            builder.ToTable("PackageSubscription");
            builder.HasKey(s => s.Id);

            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(128);
            builder.Property(s => s.PaidAmount).HasColumnType("decimal(38,18)");
            builder.Property(s => s.RefundedAmount).HasColumnType("decimal(38,18)");

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
