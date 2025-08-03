using BaseSource.Data.Configurations;
using BaseSource.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Data.EF
{
    public class BaseSourceDbContextFactory : IDesignTimeDbContextFactory<BaseSourceDbContext>
    {
        public BaseSourceDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("BaseSourceDbConnection");

            var optionsBuilder = new DbContextOptionsBuilder<BaseSourceDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new BaseSourceDbContext(optionsBuilder.Options);
        }
    }
}
