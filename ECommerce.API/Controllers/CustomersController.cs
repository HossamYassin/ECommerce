using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Customers.Commands.UpdateCustomerProfile;
using ECommerce.Application.Features.Customers.Queries.GetCustomerProfile;
using ECommerce.Application.Features.Orders.Queries.GetCustomerOrders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Authorize(Policy = "CustomerOnly")]
[Route("api/[controller]")]
public class CustomersController : ApiControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var customerId = GetCurrentUserId();
        var result = await Sender.Send(new GetCustomerProfileQuery(customerId), cancellationToken);
        return Ok(result);
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerProfileRequest request, CancellationToken cancellationToken)
    {
        var customerId = GetCurrentUserId();
        var result = await Sender.Send(new UpdateCustomerProfileCommand(
            customerId,
            request.Name,
            request.Email,
            request.NewPassword), cancellationToken);
        return Ok(result);
    }

    [HttpGet("me/orders")]
    [ProducesResponseType(typeof(PagedResult<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var customerId = GetCurrentUserId();
        var result = await Sender.Send(new GetCustomerOrdersQuery(customerId, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }
}

public record UpdateCustomerProfileRequest(string Name, string Email, string? NewPassword);

