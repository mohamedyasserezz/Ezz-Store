using Ezz_Store.DAL.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ezz_Store.DAL.Persistance.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {

            builder.Property(a => a.Country).IsRequired().HasMaxLength(100);
            builder.Property(a => a.City).IsRequired().HasMaxLength(100);

            builder.Property(a => a.Street).IsRequired().HasMaxLength(250);
            builder.Property(a => a.Zip).IsRequired().HasMaxLength(20);

            builder.Property(a => a.IsDefault)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.HasOne(a => a.User)
                   .WithMany(u => u.Addresses)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
