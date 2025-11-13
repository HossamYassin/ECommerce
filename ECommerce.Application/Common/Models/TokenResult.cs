using ECommerce.Domain.Entities;

namespace ECommerce.Application.Common.Models;

public record TokenResult(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    RefreshToken RefreshToken);

