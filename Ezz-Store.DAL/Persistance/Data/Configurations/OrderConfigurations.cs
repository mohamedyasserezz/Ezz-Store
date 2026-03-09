using Ezz_Store.DAL.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ezz_Store.DAL.Persistance.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {

            builder.Property(o => o.OrderNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(o => o.OrderNumber).IsUnique();

            builder.Property(o => o.Status)
                   .IsRequired()
                   .HasConversion(
                       o => o.ToString(),
                       o => (Status)Enum.Parse(typeof(Status), o)
                       )
                   ;

            builder.Property(o => o.OrderDate)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(o => o.TotalAmount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.HasOne(o => o.User)
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(o => o.ShippingAddress)
               .WithMany()                          
               .HasForeignKey(o => o.ShippingAddressId)
               .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
