using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Organi.Server.Application.Common.Interfaces;

namespace Organi.Server.Infrastructure.Email;

/// <summary>
/// Fallback used when Brevo:ApiKey is not configured, so the app is fully runnable — and every
/// email flow verifiable — without a Brevo account. Logs what WOULD have been sent, including
/// the confirmation link and reset code so they can be used from the console during development.
/// Never throws, same contract as the real sender.
/// </summary>
public sealed class LogOnlyEmailService(
    IOptions<AppOptions> appOptions,
    ILogger<LogOnlyEmailService> logger) : IEmailService
{
    private readonly AppOptions _appOptions = appOptions.Value;

    public Task SendEmailConfirmationAsync(string email, string name, string confirmationToken, CancellationToken cancellationToken = default)
    {
        var confirmUrl = EmailLinks.ConfirmEmail(_appOptions.FrontendBaseUrl, confirmationToken);

        logger.LogInformation("[EMAIL:log-only] Confirmation to {Email} ({Name}) — link: {ConfirmUrl}", email, name, confirmUrl);

        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(string email, string name, string code, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[EMAIL:log-only] Password reset for {Email} ({Name}) — code: {Code}", email, name, code);

        return Task.CompletedTask;
    }

    public Task SendNewsletterWelcomeAsync(string email, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[EMAIL:log-only] Newsletter welcome to {Email}", email);

        return Task.CompletedTask;
    }

    public Task SendOrderConfirmationAsync(OrderEmailModel order, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[EMAIL:log-only] Order confirmation {OrderNumber} to {Email} — total {Total}",
            order.OrderNumber, order.RecipientEmail, order.TotalAmount);

        return Task.CompletedTask;
    }

    public Task SendOrderStatusUpdateAsync(OrderEmailModel order, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[EMAIL:log-only] Order {OrderNumber} now {Status} — notifying {Email}",
            order.OrderNumber, order.Status, order.RecipientEmail);

        return Task.CompletedTask;
    }

    public Task SendContactAcknowledgementAsync(string email, string name, string subject, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[EMAIL:log-only] Contact acknowledgement to {Email} ({Name}) — subject: {Subject}", email, name, subject);

        return Task.CompletedTask;
    }
}
