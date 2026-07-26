namespace Organi.Server.Application.Features.Auth;

/// <summary>
/// Lifetimes and limits for the email-confirmation and password-reset secrets, kept in one
/// place so the issuing and validating handlers can never disagree.
/// </summary>
public static class AuthVerificationPolicy
{
    public static readonly TimeSpan EmailConfirmationLifetime = TimeSpan.FromHours(24);

    public static readonly TimeSpan PasswordResetCodeLifetime = TimeSpan.FromMinutes(15);

    /// <summary>Wrong-code guesses allowed before the code is burned (anti-brute-force on 6 digits).</summary>
    public const int MaxPasswordResetAttempts = 5;
}
