using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Persistence.Configurations;

public sealed class NewsletterSubscriberConfiguration : IEntityTypeConfiguration<NewsletterSubscriber>
{
    public void Configure(EntityTypeBuilder<NewsletterSubscriber> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(n => n.Email).IsUnique();

        builder.HasQueryFilter(n => !n.IsDeleted);
    }
}
