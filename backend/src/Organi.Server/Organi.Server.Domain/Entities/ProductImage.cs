using Organi.Server.Domain.Common;

namespace Organi.Server.Domain.Entities;

public sealed class ProductImage : BaseEntity
{
    public string ImageUrl { get; set; } = default!;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
    public Guid ProductId { get; set; }

    public Product Product { get; set; } = default!;
}
