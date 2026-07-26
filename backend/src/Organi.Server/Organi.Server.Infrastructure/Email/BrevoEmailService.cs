using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Organi.Server.Application.Common.Interfaces;

namespace Organi.Server.Infrastructure.Email;

/// <summary>
/// Sends transactional email through Brevo's REST API (POST /v3/smtp/email).
///
/// Two deliberate guarantees, both load-bearing:
///   * No method throws. Callers invoke this after their SaveChangesAsync, so a provider
///     outage must never surface as a failed checkout or registration.
///   * The HttpClient timeout is pinned in DI (5s). The try/catch alone would NOT protect the
///     request — HttpClient defaults to 100 seconds, so a hung Brevo would stall the caller
///     for 100s and only then log politely.
/// </summary>
public sealed class BrevoEmailService(
    HttpClient httpClient,
    IOptions<BrevoOptions> options,
    IOptions<AppOptions> appOptions,
    ILogger<BrevoEmailService> logger) : IEmailService
{
    private const string SendPath = "v3/smtp/email";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly BrevoOptions _options = options.Value;
    private readonly AppOptions _appOptions = appOptions.Value;

    public Task SendEmailConfirmationAsync(string email, string name, string confirmationToken, CancellationToken cancellationToken = default)
    {
        var confirmUrl = EmailLinks.ConfirmEmail(_appOptions.FrontendBaseUrl, confirmationToken);
        var (subject, html) = EmailTemplates.EmailConfirmation(name, confirmUrl);

        return SendAsync(email, name, subject, html, cancellationToken);
    }

    public Task SendPasswordResetCodeAsync(string email, string name, string code, CancellationToken cancellationToken = default)
    {
        var (subject, html) = EmailTemplates.PasswordResetCode(name, code);

        return SendAsync(email, name, subject, html, cancellationToken);
    }

    public Task SendNewsletterWelcomeAsync(string email, CancellationToken cancellationToken = default)
    {
        var (subject, html) = EmailTemplates.NewsletterWelcome();

        return SendAsync(email, email, subject, html, cancellationToken);
    }

    public Task SendOrderConfirmationAsync(OrderEmailModel order, CancellationToken cancellationToken = default)
    {
        var (subject, html) = EmailTemplates.OrderConfirmation(order);

        return SendAsync(order.RecipientEmail, order.RecipientName, subject, html, cancellationToken);
    }

    public Task SendOrderStatusUpdateAsync(OrderEmailModel order, CancellationToken cancellationToken = default)
    {
        var (subject, html) = EmailTemplates.OrderStatusUpdate(order);

        return SendAsync(order.RecipientEmail, order.RecipientName, subject, html, cancellationToken);
    }

    public Task SendContactAcknowledgementAsync(string email, string name, string subject, CancellationToken cancellationToken = default)
    {
        var (mailSubject, html) = EmailTemplates.ContactAcknowledgement(name, subject);

        return SendAsync(email, name, mailSubject, html, cancellationToken);
    }

    private async Task SendAsync(string toEmail, string toName, string subject, string html, CancellationToken cancellationToken)
    {
        var payload = new
        {
            sender = new { name = _options.SenderName, email = _options.SenderEmail },
            to = new[] { new { email = toEmail, name = string.IsNullOrWhiteSpace(toName) ? toEmail : toName } },
            subject,
            htmlContent = html
        };

        try
        {
            using var content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");
            using var response = await httpClient.PostAsync(SendPath, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);

                logger.LogError(
                    "Brevo rejected email {Subject} to {Email}: {StatusCode} {Body}",
                    subject, toEmail, (int)response.StatusCode, body);

                return;
            }

            logger.LogInformation("Email {Subject} sent to {Email}", subject, toEmail);
        }
        catch (Exception ex)
        {
            // Swallowed on purpose — see the class summary. The business operation has already
            // committed; failing here would misreport it to the user.
            logger.LogError(ex, "Failed to send email {Subject} to {Email}", subject, toEmail);
        }
    }
}
