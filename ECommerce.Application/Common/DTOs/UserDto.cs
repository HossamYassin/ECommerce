namespace ECommerce.Application.Common.DTOs;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    string Role);

