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
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("UserProfile");
            builder.HasKey(x => x.UserId);
            builder.HasIndex(x => x.CustomId).IsUnique();
            builder.HasIndex(x => x.ApiKey).IsUnique();

            builder.Property(x => x.UserId).IsRequired().HasMaxLength(128);
            builder.Property(x => x.CustomId).IsRequired().HasMaxLength(128).UseCollation(SQL_CONST.Collation);
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(250).UseCollation(SQL_CONST.Collation);
            builder.Property(x => x.LastName).HasMaxLength(250).UseCollation(SQL_CONST.Collation);
            builder.Property(x => x.JoinedDate).IsRequired().HasDefaultValueSql("GetDate()");
            builder.Property(x => x.ApiKey).IsRequired().HasMaxLength(128);


            builder.HasOne(x => x.AppUser).WithOne(x => x.UserProfile).HasForeignKey<UserProfile>(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
