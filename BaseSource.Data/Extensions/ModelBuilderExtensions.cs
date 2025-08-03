using BaseSource.Data.Entities;
using BaseSource.Shared.Constants;
using BaseSource.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Identity data
            var roleAdminId = RoleConstants.Id.Admin;

            modelBuilder.Entity<AppRole>().HasData(
                new AppRole
                {
                    Id = roleAdminId,
                    Name = "Admin",
                    NormalizedName = "Admin",
                    Description = "Administrator role",
                    ConcurrencyStamp = ""
                });

            var userAdminId = "3423d55f-e6e9-41dc-8e3e-a9efb36e3a69";
            //var hasher = new PasswordHasher<AppUser>();
            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = userAdminId,
                UserName = "superadmin",
                NormalizedUserName = "superadmin",
                Email = "superadmin@gmail.com",
                NormalizedEmail = "superadmin@gmail.com",
                EmailConfirmed = true,
                //PasswordHash = hasher.HashPassword(null, "superadmin"),
                PasswordHash = "AQAAAAIAAYagAAAAEDNAq8crIjCNxgwzvvydKjdf0FGcfDahXI9wMSSAQhPFlyXCZ4kYWngYR/v6GGvLVA==",
                SecurityStamp = string.Empty,
                ConcurrencyStamp = ""
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = roleAdminId,
                UserId = userAdminId
            });

            modelBuilder.Entity<UserProfile>().HasData(
               new UserProfile
               {
                   UserId = userAdminId,
                   CustomId = userAdminId,
                   FirstName = "Admin",
                   JoinedDate = new DateTime(2025, 1, 1),
                   ApiKey = userAdminId
               });
        }
    }
}
