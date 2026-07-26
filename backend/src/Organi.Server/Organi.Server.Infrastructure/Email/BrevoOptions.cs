namespace Organi.Server.Infrastructure.Email;

public sealed class BrevoOptions
{
    public const string SectionName = "Brevo";

    /// <summary>
    /// Secret — set via user-secrets or environment, never committed. When blank the app
    /// falls back to <see cref="LogOnlyEmailService"/> so it stays runnable without Brevo.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    public string SenderName { get; set; } = "Organi";
    public string SenderEmail { get; set; } = "no-reply@organi.dev";
}

public sealed class AppOptions
{
    public const string SectionName = "App";

    /// <summary>Base URL of the storefront, used to build links inside emails.</summary>
    public string FrontendBaseUrl { get; set; } = "http://localhost:3000";
}
