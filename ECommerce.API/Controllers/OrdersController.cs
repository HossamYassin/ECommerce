using System.Security.Claims;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Features.Orders.Commands.CancelOrder;
using ECommerce.Application.Features.Orders.Commands.PlaceOrder;
using ECommerce.Application.Features.Orders.Queries.GetOrderById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class OrdersController : ApiControllerBase
{
    [HttpPost]
    [Authorize(Policy = "CustomerOnly")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request, CancellationToken cancellationToken)
    {
        var customerId = GetCurrentUserId();
        var order = await Sender.Send(new PlaceOrderCommand(customerId, request.Items), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");
        var result = await Sender.Send(new CancelOrderCommand(id, userId, isAdmin), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userIdClaim = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");
        var result = await Sender.Send(new GetOrderByIdQuery(id, userIdClaim, isAdmin), cancellationToken);
        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (userId is null || !Guid.TryParse(userId, out var guid))
        {
            throw new UnauthorizedAccessException("User identifier not found.");
        }

        return guid;
    }
}

public record PlaceOrderRequest(IReadOnlyList<OrderItemRequest> Items);

