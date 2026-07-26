using Organi.Server.Domain.Common;

namespace Organi.Server.Domain.Entities;

public sealed class User : AuditableEntity
{
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int FailedLoginCount { get; set; }
    public DateTime? LockoutEnd { get; set; }

    // Email verification / password reset. Only SHA-256 hashes are stored — the raw token and
    // code exist solely in the email that was sent, so a database leak cannot be used to take
    // over an account.
    public string? EmailConfirmationTokenHash { get; set; }
    public DateTime? EmailConfirmationTokenExpiresAt { get; set; }
    public string? PasswordResetCodeHash { get; set; }
    public DateTime? PasswordResetCodeExpiresAt { get; set; }
    public int PasswordResetAttemptCount { get; set; }

    public ICollection<Role> Roles { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
    public Vendor? Vendor { get; set; }
    public Cart? Cart { get; set; }
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<WishlistItem> WishlistItems { get; set; } = [];
    public ICollection<CompareItem> CompareItems { get; set; } = [];
    public ICollection<BlogPost> BlogPosts { get; set; } = [];
    public ICollection<BlogComment> BlogComments { get; set; } = [];
}
