using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _sender;

    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected Guid GetCurrentUserId()
    {
        var userId = User.Identity?.Name 
                     ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub) 
                     ?? User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? User.FindFirstValue("sub");

        if (userId is null || !Guid.TryParse(userId, out var guid))
        {
            throw new UnauthorizedAccessException("User identifier not found in token claims.");
        }

        return guid;
    }
}

