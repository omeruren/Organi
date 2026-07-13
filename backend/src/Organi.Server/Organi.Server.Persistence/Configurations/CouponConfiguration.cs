using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Persistence.Configurations;

public sealed class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Description)
            .HasMaxLength(256);

        builder.Property(c => c.DiscountType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.DiscountValue).HasPrecision(18, 2);
        builder.Property(c => c.MinimumOrderAmount).HasPrecision(18, 2);

        builder.HasIndex(c => c.Code).IsUnique();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Coupon_DiscountValue", "[DiscountValue] > 0");
            t.HasCheckConstraint("CK_Coupon_Dates", "[EndDate] > [StartDate]");
        });

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
