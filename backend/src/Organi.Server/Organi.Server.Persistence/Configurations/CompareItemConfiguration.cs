using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Persistence.Configurations;

public sealed class CompareItemConfiguration : IEntityTypeConfiguration<CompareItem>
{
    public void Configure(EntityTypeBuilder<CompareItem> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => new { c.UserId, c.ProductId }).IsUnique();
    }
}
