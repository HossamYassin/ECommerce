using System.Security.Claims;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Orders.Commands.CancelOrder;
using ECommerce.Application.Features.Orders.Commands.UpdateOrderStatus;
using ECommerce.Application.Features.Orders.Queries.GetOrderById;
using ECommerce.Application.Features.Orders.Queries.GetOrders;
using ECommerce.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.Admin;

[Authorize(Policy = "AdminOnly")]
[Route("api/admin/orders")]
public class AdminOrdersController : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] Guid? customerId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Sender.Send(new GetOrdersQuery(pageNumber, pageSize, status, customerId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await Sender.Send(new GetOrderByIdQuery(id, userId, true), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await Sender.Send(new CancelOrderCommand(id, userId, true), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<OrderStatus>(request.Status, true, out var status))
        {
            return BadRequest(new { message = "Invalid order status." });
        }

        var result = await Sender.Send(new UpdateOrderStatusCommand(id, status), cancellationToken);
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

public record UpdateOrderStatusRequest(string Status);

