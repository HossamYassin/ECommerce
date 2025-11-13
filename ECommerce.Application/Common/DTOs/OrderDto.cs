namespace ECommerce.Application.Common.DTOs;

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status,
    IReadOnlyList<OrderItemDto> Items);

