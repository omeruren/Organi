namespace Organi.Server.Infrastructure.Email;

/// <summary>
/// Builds storefront links embedded in emails. Lives in Infrastructure because the frontend
/// base address is deployment configuration, not application logic.
/// </summary>
public static class EmailLinks
{
    public static string ConfirmEmail(string frontendBaseUrl, string token) =>
        $"{frontendBaseUrl.TrimEnd('/')}/confirm-email?token={Uri.EscapeDataString(token)}";
}
