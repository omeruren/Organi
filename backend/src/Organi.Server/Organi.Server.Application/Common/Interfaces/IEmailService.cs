namespace Organi.Server.Application.Common.Interfaces;

/// <summary>
/// Line item on an order-related email.
/// </summary>
public sealed record OrderEmailLine(string ProductName, int Quantity, decimal UnitPrice, decimal TotalPrice);

/// <summary>
/// Everything the order emails need, snapshotted by the caller so the email layer
/// never touches the DbContext.
/// </summary>
public sealed record OrderEmailModel(
    string OrderNumber,
    string Status,
    string RecipientEmail,
    string RecipientName,
    decimal SubTotal,
    decimal DiscountAmount,
    decimal ShippingCost,
    decimal TaxAmount,
    decimal TotalAmount,
    IReadOnlyList<OrderEmailLine> Lines,
    string? CancellationReason = null);

/// <summary>
/// Transactional email. Implementations MUST NOT throw — a mail-provider outage must never
/// fail the business operation that triggered it. Callers invoke these only AFTER their
/// SaveChangesAsync, so an email never describes a change that was rolled back.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends the confirmation link. Takes the raw token, not a URL — the implementation owns the
    /// frontend base address, so the Application layer stays unaware of frontend routing.
    /// </summary>
    Task SendEmailConfirmationAsync(string email, string name, string confirmationToken, CancellationToken cancellationToken = default);

    Task SendPasswordResetCodeAsync(string email, string name, string code, CancellationToken cancellationToken = default);

    Task SendNewsletterWelcomeAsync(string email, CancellationToken cancellationToken = default);

    Task SendOrderConfirmationAsync(OrderEmailModel order, CancellationToken cancellationToken = default);

    Task SendOrderStatusUpdateAsync(OrderEmailModel order, CancellationToken cancellationToken = default);

    Task SendContactAcknowledgementAsync(string email, string name, string subject, CancellationToken cancellationToken = default);
}
