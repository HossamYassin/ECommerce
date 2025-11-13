using AutoMapper;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Interfaces.Persistence;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Orders.Commands.PlaceOrder;

public record PlaceOrderCommand(Guid CustomerId, IReadOnlyList<OrderItemRequest> Items) : IRequest<OrderDto>;

public record OrderItemRequest(Guid ProductId, int Quantity);

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("An order must contain at least one item.");

        RuleForEach(x => x.Items).SetValidator(new OrderItemRequestValidator());
    }
}

public class OrderItemRequestValidator : AbstractValidator<OrderItemRequest>
{
    public OrderItemRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PlaceOrderCommandHandler> _logger;
    private readonly IEmailService _emailService;

    public PlaceOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<PlaceOrderCommandHandler> logger,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<OrderDto> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Users.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            throw new ValidationException("Customer not found.");
        }

        var productIds = request.Items.Select(item => item.ProductId).Distinct().ToList();
        var products = await _unitOfWork.Products.ListAsync(
            product => productIds.Contains(product.Id),
            cancellationToken,
            product => product.Category);

        if (products.Count != productIds.Count)
        {
            throw new ValidationException("One or more products were not found.");
        }

        foreach (var product in products)
        {
            if (!product.IsActive || product.IsDeleted)
            {
                throw new ValidationException($"Product '{product.Name}' is not available.");
            }
        }

        var productLookup = products.ToDictionary(p => p.Id);
        decimal totalAmount = 0;

        foreach (var item in request.Items)
        {
            var product = productLookup[item.ProductId];
            if (product.StockQuantity < item.Quantity)
            {
                throw new ValidationException($"Insufficient stock for product '{product.Name}'.");
            }

            totalAmount += product.Price * item.Quantity;
        }

        var order = new Order
        {
            CustomerId = request.CustomerId,
            Customer = customer,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            Items = new List<OrderItem>()
        };

        foreach (var item in request.Items)
        {
            var product = productLookup[item.ProductId];
            product.StockQuantity -= item.Quantity;
            product.UpdatedDate = DateTime.UtcNow;

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = item.Quantity,
                PriceAtOrder = product.Price
            });
        }

        await _unitOfWork.Orders.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} placed by customer {CustomerEmail}.", order.Id, customer.Email);

        // Send email notification
        try
        {
            var emailSubject = $"Order Confirmation - Order #{order.Id:N}";
            var emailBody = CreateOrderConfirmationEmailBody(order, customer);
            await _emailService.SendEmailAsync(customer.Email, emailSubject, emailBody, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send order confirmation email to {Email}", customer.Email);
            // Don't fail the order placement if email fails
        }

        return _mapper.Map<OrderDto>(order);
    }

    private static string CreateOrderConfirmationEmailBody(Order order, User customer)
    {
        var itemsHtml = string.Join("", order.Items.Select(item =>
        {
            var productName = item.Product?.Name ?? "Product";
            var lineTotal = item.PriceAtOrder * item.Quantity;
            return $@"
            <tr>
                <td style=""padding: 12px; border-bottom: 1px solid #e0e0e0;"">{productName}</td>
                <td style=""padding: 12px; border-bottom: 1px solid #e0e0e0; text-align: center;"">{item.Quantity}</td>
                <td style=""padding: 12px; border-bottom: 1px solid #e0e0e0; text-align: right;"">${item.PriceAtOrder:F2}</td>
                <td style=""padding: 12px; border-bottom: 1px solid #e0e0e0; text-align: right; font-weight: bold;"">${lineTotal:F2}</td>
            </tr>";
        }));

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Order Confirmation</title>
</head>
<body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f5f5f5;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #f5f5f5; padding: 20px;"">
        <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background-color: #28a745; padding: 30px; text-align: center; border-radius: 8px 8px 0 0;"">
                            <h1 style=""color: #ffffff; margin: 0; font-size: 24px;"">Order Confirmed!</h1>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <p style=""color: #333333; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;"">
                                Dear {customer.Name},
                            </p>
                            <p style=""color: #333333; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;"">
                                Thank you for your order! We're excited to let you know that we've received your order and it's being processed. You'll receive another email when your order ships.
                            </p>
                            <!-- Order Details -->
                            <div style=""background-color: #f8f9fa; padding: 20px; border-radius: 6px; margin: 30px 0;"">
                                <h2 style=""color: #333333; font-size: 18px; margin: 0 0 15px 0; border-bottom: 2px solid #28a745; padding-bottom: 10px;"">
                                    Order Information
                                </h2>
                                <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                    <tr>
                                        <td style=""padding: 8px 0; color: #666666; width: 40%;"">Order Number:</td>
                                        <td style=""padding: 8px 0; color: #333333; font-weight: bold;"">{order.Id:N}</td>
                                    </tr>
                                    <tr>
                                        <td style=""padding: 8px 0; color: #666666;"">Order Date:</td>
                                        <td style=""padding: 8px 0; color: #333333;"">{order.OrderDate:MMMM dd, yyyy 'at' HH:mm}</td>
                                    </tr>
                                    <tr>
                                        <td style=""padding: 8px 0; color: #666666;"">Status:</td>
                                        <td style=""padding: 8px 0; color: #28a745; font-weight: bold;"">{order.Status}</td>
                                    </tr>
                                </table>
                            </div>
                            <!-- Order Items -->
                            <h2 style=""color: #333333; font-size: 18px; margin: 30px 0 15px 0;"">Order Items</h2>
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""border-collapse: collapse;"">
                                <thead>
                                    <tr style=""background-color: #f8f9fa;"">
                                        <th style=""padding: 12px; text-align: left; border-bottom: 2px solid #e0e0e0; color: #333333;"">Product</th>
                                        <th style=""padding: 12px; text-align: center; border-bottom: 2px solid #e0e0e0; color: #333333;"">Quantity</th>
                                        <th style=""padding: 12px; text-align: right; border-bottom: 2px solid #e0e0e0; color: #333333;"">Unit Price</th>
                                        <th style=""padding: 12px; text-align: right; border-bottom: 2px solid #e0e0e0; color: #333333;"">Subtotal</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {itemsHtml}
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td colspan=""3"" style=""padding: 15px 12px; text-align: right; font-weight: bold; font-size: 16px; border-top: 2px solid #333333;"">
                                            Total Amount:
                                        </td>
                                        <td style=""padding: 15px 12px; text-align: right; font-weight: bold; font-size: 16px; border-top: 2px solid #333333; color: #28a745;"">
                                            ${order.TotalAmount:F2}
                                        </td>
                                    </tr>
                                </tfoot>
                            </table>
                            <!-- Shipping Information -->
                            <div style=""background-color: #d1ecf1; border-left: 4px solid #17a2b8; padding: 15px; margin: 30px 0; border-radius: 4px;"">
                                <p style=""color: #0c5460; margin: 0; font-size: 14px; line-height: 1.6;"">
                                    <strong>What's Next?</strong> We'll send you a shipping confirmation email with a tracking number once your order has been shipped. Expected delivery time is typically 3-5 business days.
                                </p>
                            </div>
                            <!-- Support -->
                            <p style=""color: #333333; font-size: 16px; line-height: 1.6; margin: 30px 0 0 0;"">
                                If you have any questions about your order, please feel free to contact our customer support team. We're here to help!
                            </p>
                            <p style=""color: #333333; font-size: 16px; line-height: 1.6; margin: 20px 0 0 0;"">
                                Thank you for shopping with us!
                            </p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #f8f9fa; padding: 30px; text-align: center; border-radius: 0 0 8px 8px;"">
                            <p style=""color: #666666; font-size: 14px; margin: 0 0 10px 0;"">
                                <strong>E-Commerce Team</strong>
                            </p>
                            <p style=""color: #999999; font-size: 12px; margin: 0;"">
                                This is an automated email. Please do not reply directly to this message.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}

