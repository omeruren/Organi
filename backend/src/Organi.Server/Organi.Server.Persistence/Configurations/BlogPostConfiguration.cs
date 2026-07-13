using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Persistence.Configurations;

public sealed class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Content)
            .IsRequired()
            .HasMaxLength(10000);

        builder.Property(b => b.Excerpt)
            .HasMaxLength(500);

        builder.Property(b => b.FeaturedImageUrl)
            .HasMaxLength(500);

        builder.HasIndex(b => b.Slug).IsUnique();
        builder.HasIndex(b => b.IsPublished);
        builder.HasIndex(b => b.AuthorId);

        builder.HasMany(b => b.BlogComments)
            .WithOne(c => c.BlogPost)
            .HasForeignKey(c => c.BlogPostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}
