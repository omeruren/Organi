using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
        builder.Property(oi => oi.TotalPrice).HasPrecision(18, 2);

        builder.Property(oi => oi.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(oi => oi.ProductSKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(oi => oi.OrderId);
        builder.HasIndex(oi => oi.VendorId);
        builder.HasIndex(oi => oi.ProductId);

        builder.HasOne(oi => oi.Vendor)
            .WithMany()
            .HasForeignKey(oi => oi.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_OrderItem_Quantity", "[Quantity] > 0");
            t.HasCheckConstraint("CK_OrderItem_UnitPrice", "[UnitPrice] >= 0");
        });
    }
}
