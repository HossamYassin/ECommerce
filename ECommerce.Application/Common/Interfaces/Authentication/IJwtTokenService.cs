using System.Security.Claims;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Common.Interfaces.Authentication;

public interface IJwtTokenService
{
    Task<TokenResult> GenerateTokensAsync(
        User user,
        CancellationToken cancellationToken = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}