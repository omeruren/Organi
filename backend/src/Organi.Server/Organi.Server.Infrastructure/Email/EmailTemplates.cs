using System.Net;
using System.Text;
using Organi.Server.Application.Common.Interfaces;

namespace Organi.Server.Infrastructure.Email;

/// <summary>
/// Inline HTML email bodies. Kept in the repo (rather than Brevo-hosted templates) so they are
/// version-controlled and need no dashboard setup. All interpolated values are HTML-encoded —
/// product names, customer names and cancellation reasons are user-supplied.
/// </summary>
public static class EmailTemplates
{
    private const string Brand = "#7cc000";

    private static string Layout(string heading, string body) => $"""
        <!doctype html>
        <html>
          <body style="margin:0;padding:24px;background:#f4f6f0;font-family:Arial,Helvetica,sans-serif;color:#333;">
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width:560px;margin:0 auto;background:#fff;border-radius:12px;overflow:hidden;">
              <tr>
                <td style="background:{Brand};padding:20px 28px;">
                  <span style="color:#fff;font-size:22px;font-weight:bold;letter-spacing:1px;">ORGANI</span>
                </td>
              </tr>
              <tr>
                <td style="padding:28px;">
                  <h1 style="margin:0 0 16px;font-size:20px;color:#292929;">{heading}</h1>
                  {body}
                </td>
              </tr>
              <tr>
                <td style="padding:18px 28px;background:#fafafa;color:#8a8a8a;font-size:12px;">
                  You are receiving this email because of activity on your Organi account.
                </td>
              </tr>
            </table>
          </body>
        </html>
        """;

    private static string Button(string url, string label) =>
        $"""<p style="margin:24px 0;"><a href="{WebUtility.HtmlEncode(url)}" style="background:{Brand};color:#fff;text-decoration:none;padding:12px 26px;border-radius:24px;display:inline-block;font-weight:bold;">{label}</a></p>""";

    private static string Money(decimal value) => $"${value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}";

    private static string E(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);

    public static (string Subject, string Html) EmailConfirmation(string name, string confirmUrl) => (
        "Confirm your Organi email address",
        Layout($"Welcome, {E(name)}!", $"""
            <p style="line-height:1.7;">Thanks for creating an Organi account. Please confirm your email address so we can keep your account secure.</p>
            {Button(confirmUrl, "Confirm Email")}
            <p style="line-height:1.7;color:#6b6b6b;font-size:13px;">This link expires in 24 hours. If the button doesn't work, copy this link into your browser:<br>
            <span style="word-break:break-all;">{E(confirmUrl)}</span></p>
            <p style="line-height:1.7;color:#6b6b6b;font-size:13px;">If you didn't create this account, you can safely ignore this email.</p>
            """));

    public static (string Subject, string Html) PasswordResetCode(string name, string code) => (
        "Your Organi password reset code",
        Layout("Reset your password", $"""
            <p style="line-height:1.7;">Hi {E(name)}, use the code below to set a new password.</p>
            <p style="margin:24px 0;font-size:32px;font-weight:bold;letter-spacing:8px;color:{Brand};">{E(code)}</p>
            <p style="line-height:1.7;color:#6b6b6b;font-size:13px;">This code expires in 15 minutes and can only be used once.</p>
            <p style="line-height:1.7;color:#6b6b6b;font-size:13px;">If you didn't request a password reset, ignore this email — your password is unchanged.</p>
            """));

    public static (string Subject, string Html) NewsletterWelcome() => (
        "Welcome to the Organi newsletter",
        Layout("You're subscribed!", """
            <p style="line-height:1.7;">Thanks for subscribing. You'll now hear from us about seasonal organic produce, new vendors and the occasional recipe.</p>
            <p style="line-height:1.7;color:#6b6b6b;font-size:13px;">You can unsubscribe at any time from the link in any newsletter.</p>
            """));

    public static (string Subject, string Html) ContactAcknowledgement(string name, string subject) => (
        "We received your message",
        Layout($"Thanks for reaching out, {E(name)}", $"""
            <p style="line-height:1.7;">We've received your message and our team will get back to you shortly.</p>
            <p style="line-height:1.7;"><strong>Subject:</strong> {E(subject)}</p>
            """));

    public static (string Subject, string Html) OrderConfirmation(OrderEmailModel order) => (
        $"Order {order.OrderNumber} confirmed",
        Layout("Thank you for your order!", $"""
            <p style="line-height:1.7;">Hi {E(order.RecipientName)}, we've received order <strong>{E(order.OrderNumber)}</strong> and it's being prepared.</p>
            {ItemsTable(order)}
            """));

    public static (string Subject, string Html) OrderStatusUpdate(OrderEmailModel order)
    {
        var message = order.Status switch
        {
            "Confirmed" => "Your order has been confirmed and is being prepared.",
            "Shipped" => "Good news — your order is on its way!",
            "Delivered" => "Your order has been delivered. Enjoy!",
            "Cancelled" => "Your order has been cancelled."
                + (string.IsNullOrWhiteSpace(order.CancellationReason) ? "" : $" Reason: {E(order.CancellationReason)}"),
            _ => $"Your order status is now {E(order.Status)}."
        };

        return (
            $"Order {order.OrderNumber} is now {order.Status}",
            Layout($"Order {E(order.Status)}", $"""
                <p style="line-height:1.7;">Hi {E(order.RecipientName)}, {message}</p>
                <p style="line-height:1.7;"><strong>Order:</strong> {E(order.OrderNumber)}</p>
                {ItemsTable(order)}
                """));
    }

    private static string ItemsTable(OrderEmailModel order)
    {
        var rows = new StringBuilder();

        foreach (var line in order.Lines)
        {
            rows.Append($"""
                <tr>
                  <td style="padding:8px 0;border-bottom:1px solid #eee;">{E(line.ProductName)} &times; {line.Quantity}</td>
                  <td style="padding:8px 0;border-bottom:1px solid #eee;text-align:right;">{Money(line.TotalPrice)}</td>
                </tr>
                """);
        }

        var discountRow = order.DiscountAmount > 0
            ? $"""<tr><td style="padding:4px 0;color:{Brand};">Discount</td><td style="padding:4px 0;text-align:right;color:{Brand};">-{Money(order.DiscountAmount)}</td></tr>"""
            : string.Empty;

        return $"""
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="margin:20px 0;font-size:14px;">
              {rows}
              <tr><td style="padding:8px 0 4px;">Subtotal</td><td style="padding:8px 0 4px;text-align:right;">{Money(order.SubTotal)}</td></tr>
              {discountRow}
              <tr><td style="padding:4px 0;">Shipping</td><td style="padding:4px 0;text-align:right;">{Money(order.ShippingCost)}</td></tr>
              <tr><td style="padding:4px 0;">Tax</td><td style="padding:4px 0;text-align:right;">{Money(order.TaxAmount)}</td></tr>
              <tr>
                <td style="padding:10px 0 0;border-top:2px solid #292929;font-weight:bold;font-size:16px;">Total</td>
                <td style="padding:10px 0 0;border-top:2px solid #292929;font-weight:bold;font-size:16px;text-align:right;">{Money(order.TotalAmount)}</td>
              </tr>
            </table>
            """;
    }
}
