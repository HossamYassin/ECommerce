namespace ECommerce.Application.Common.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    bool IsActive,
    Guid CategoryId,
    string CategoryName);

