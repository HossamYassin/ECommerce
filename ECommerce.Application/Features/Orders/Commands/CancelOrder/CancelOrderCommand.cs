using AutoMapper;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Interfaces.Persistence;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderCommand(Guid OrderId, Guid RequestedByUserId, bool IsAdmin) : IRequest<OrderDto>;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.RequestedByUserId).NotEmpty();
    }
}

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelOrderCommandHandler> _logger;
    private readonly IEmailService _emailService;

    public CancelOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CancelOrderCommandHandler> logger,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<OrderDto> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.FirstOrDefaultAsync(
            o => o.Id == request.OrderId,
            cancellationToken,
            o => o.Items,
            o => o.Customer);

        if (order is null)
        {
            throw new ValidationException("Order not found.");
        }

        if (!request.IsAdmin && order.CustomerId != request.RequestedByUserId)
        {
            throw new ValidationException("You are not authorized to cancel this order.");
        }

        if (order.Status != OrderStatus.Pending)
        {
            throw new ValidationException("Only pending orders can be cancelled.");
        }

        var productIds = order.Items.Select(i => i.ProductId).ToList();
        var products = await _unitOfWork.Products.ListAsync(
            p => productIds.Contains(p.Id),
            cancellationToken);
        var productLookup = products.ToDictionary(p => p.Id);

        foreach (var item in order.Items)
        {
            if (productLookup.TryGetValue(item.ProductId, out var product))
            {
                product.StockQuantity += item.Quantity;
                product.UpdatedDate = DateTime.UtcNow;
            }
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedDate = DateTime.UtcNow;

        await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} cancelled.", order.Id);

        // Send email notification
        try
        {
            var emailSubject = $"Order Cancellation Confirmation - Order #{order.Id:N}";
            var emailBody = CreateCancellationEmailBody(order);
            await _emailService.SendEmailAsync(order.Customer!.Email, emailSubject, emailBody, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send order cancellation email to {Email}", order.Customer?.Email);
            // Don't fail the cancellation if email fails
        }

        return _mapper.Map<OrderDto>(order);
    }

    private static string CreateCancellationEmailBody(Order order)
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
    <title>Order Cancellation Confirmation</title>
</head>
<body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f5f5f5;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #f5f5f5; padding: 20px;"">
        <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background-color: #dc3545; padding: 30px; text-align: center; border-radius: 8px 8px 0 0;"">
                            <h1 style=""color: #ffffff; margin: 0; font-size: 24px;"">Order Cancelled</h1>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <p style=""color: #333333; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;"">
                                Dear {order.Customer?.Name ?? "Customer"},
                            </p>
                            <p style=""color: #333333; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;"">
                                We have successfully processed the cancellation of your order. The refund will be processed to your original payment method within 5-7 business days.
                            </p>
                            <!-- Order Details -->
                            <div style=""background-color: #f8f9fa; padding: 20px; border-radius: 6px; margin: 30px 0;"">
                                <h2 style=""color: #333333; font-size: 18px; margin: 0 0 15px 0; border-bottom: 2px solid #dc3545; padding-bottom: 10px;"">
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
                                        <td style=""padding: 8px 0; color: #666666;"">Cancellation Date:</td>
                                        <td style=""padding: 8px 0; color: #333333;"">{order.UpdatedDate:MMMM dd, yyyy 'at' HH:mm}</td>
                                    </tr>
                                    <tr>
                                        <td style=""padding: 8px 0; color: #666666;"">Status:</td>
                                        <td style=""padding: 8px 0; color: #dc3545; font-weight: bold;"">{order.Status}</td>
                                    </tr>
                                </table>
                            </div>
                            <!-- Order Items -->
                            <h2 style=""color: #333333; font-size: 18px; margin: 30px 0 15px 0;"">Cancelled Items</h2>
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
                                        <td style=""padding: 15px 12px; text-align: right; font-weight: bold; font-size: 16px; border-top: 2px solid #333333; color: #dc3545;"">
                                            ${order.TotalAmount:F2}
                                        </td>
                                    </tr>
                                </tfoot>
                            </table>
                            <!-- Refund Information -->
                            <div style=""background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 30px 0; border-radius: 4px;"">
                                <p style=""color: #856404; margin: 0; font-size: 14px; line-height: 1.6;"">
                                    <strong>Refund Information:</strong> The full amount of ${order.TotalAmount:F2} will be refunded to your original payment method. Please allow 5-7 business days for the refund to appear in your account.
                                </p>
                            </div>
                            <!-- Support -->
                            <p style=""color: #333333; font-size: 16px; line-height: 1.6; margin: 30px 0 0 0;"">
                                If you have any questions or concerns about this cancellation, please don't hesitate to contact our customer support team.
                            </p>
                            <p style=""color: #333333; font-size: 16px; line-height: 1.6; margin: 20px 0 0 0;"">
                                Thank you for your understanding.
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
