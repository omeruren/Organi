using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Persistence.Configurations;

public sealed class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.StoreName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.Description)
            .HasMaxLength(2000);

        builder.Property(v => v.LogoUrl)
            .HasMaxLength(500);

        builder.Property(v => v.BannerUrl)
            .HasMaxLength(500);

        builder.Property(v => v.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(v => v.Address)
            .HasMaxLength(500);

        builder.Property(v => v.City)
            .HasMaxLength(100);

        builder.Property(v => v.Rating)
            .HasPrecision(3, 2);

        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(v => v.Slug).IsUnique();
        builder.HasIndex(v => v.Status);

        builder.HasQueryFilter(v => !v.IsDeleted);
    }
}
