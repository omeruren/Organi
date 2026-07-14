using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Utilities;
using Organi.Server.Application.Features.Orders.DTOs;
using Organi.Server.Application.Features.Orders.Mappings;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Orders.Commands.CreateOrder;

public sealed class CreateOrderHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<CreateOrderHandler> logger) : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var cart = await context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null || cart.CartItems.Count == 0)
            throw new BusinessRuleException("Your cart is empty.");

        foreach (var item in cart.CartItems)
        {
            if (item.Quantity > item.Product.StockQuantity)
                throw new BusinessRuleException(
                    $"Insufficient stock for product '{item.Product.Name}'. Available: {item.Product.StockQuantity}, Requested: {item.Quantity}.");
        }

        var orderItems = cart.CartItems.Select(ci =>
        {
            var unitPrice = ci.Product.SalePrice ?? ci.Product.Price;
            return new OrderItem
            {
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                ProductSKU = ci.Product.SKU,
                VendorId = ci.Product.VendorId,
                Quantity = ci.Quantity,
                UnitPrice = unitPrice,
                TotalPrice = unitPrice * ci.Quantity
            };
        }).ToList();

        var subTotal = orderItems.Sum(oi => oi.TotalPrice);

        var orderNumber = await OrderNumberGenerator.GenerateUniqueAsync(
            candidate => context.Orders.AnyAsync(o => o.OrderNumber == candidate, cancellationToken));

        var order = new Order
        {
            OrderNumber = orderNumber,
            SubTotal = subTotal,
            DiscountAmount = 0m,
            ShippingCost = 0m,
            TaxAmount = 0m,
            TotalAmount = subTotal,
            Status = OrderStatus.Pending,
            Notes = request.Notes,
            ShippingFirstName = request.ShippingFirstName,
            ShippingLastName = request.ShippingLastName,
            ShippingAddress = request.ShippingAddress,
            ShippingCity = request.ShippingCity,
            ShippingPostalCode = request.ShippingPostalCode,
            ShippingPhone = request.ShippingPhone,
            ShippingEmail = request.ShippingEmail,
            UserId = userId,
            OrderItems = orderItems
        };

        context.Orders.Add(order);
        context.CartItems.RemoveRange(cart.CartItems);
        cart.CartItems.Clear();

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {OrderNumber} created by user {UserId}", order.OrderNumber, userId);

        return order.ToResponse();
    }
}
