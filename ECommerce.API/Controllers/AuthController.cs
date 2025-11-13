using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Features.Authentication.Commands.Login;
using ECommerce.Application.Features.Authentication.Commands.RefreshToken;
using ECommerce.Application.Features.Authentication.Commands.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new RegisterCommand(request.Name, request.Email, request.Password), cancellationToken);
        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new LoginCommand(request.Email, request.Password), cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new RefreshTokenCommand(request.AccessToken, request.RefreshToken), cancellationToken);
        return Ok(result);
    }
}

public record RegisterRequest(string Name, string Email, string Password);

public record LoginRequest(string Email, string Password);

public record RefreshTokenRequest(string AccessToken, string RefreshToken);

