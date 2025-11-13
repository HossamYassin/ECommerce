namespace ECommerce.Application.Common.DTOs;

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal PriceAtOrder);

