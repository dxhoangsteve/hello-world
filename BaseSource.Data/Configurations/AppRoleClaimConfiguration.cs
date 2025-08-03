//using BaseSource.Data.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BaseSource.Data.Configurations
//{
//    public class AppRoleClaimConfiguration : IEntityTypeConfiguration<AppRoleClaim>
//    {
//        public void Configure(EntityTypeBuilder<AppRoleClaim> builder)
//        {
//            builder.Property(x => x.RoleId).HasMaxLength(128);
//            builder.Property(x => x.Name).HasMaxLength(256);
//            builder.Property(x => x.Description).HasMaxLength(256);
//        }
//    }
//}
