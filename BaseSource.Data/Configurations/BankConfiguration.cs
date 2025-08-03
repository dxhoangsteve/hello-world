using BaseSource.Data.Entities;
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
    public class BankConfiguration : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> builder)
        {
            builder.ToTable("Bank");
            builder.HasKey(x => x.id);

            builder.Property(x => x.id).ValueGeneratedNever();
            builder.Property(x => x.id).IsRequired().HasMaxLength(128);
        }
    }
}
