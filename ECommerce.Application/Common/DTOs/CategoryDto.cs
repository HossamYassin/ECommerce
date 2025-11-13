namespace ECommerce.Application.Common.DTOs;

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description);

