using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Products.Commands.CreateProduct;
using ECommerce.Application.Features.Products.Commands.DeleteProduct;
using ECommerce.Application.Features.Products.Commands.UpdateProduct;
using ECommerce.Application.Features.Products.Queries.SearchProducts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.Admin;

[Authorize(Policy = "AdminOnly")]
[Route("api/admin/products")]
public class AdminProductsController : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? name,
        [FromQuery] Guid? categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool isAscending = false,
        [FromQuery] bool includeInactive = true,
        CancellationToken cancellationToken = default)
    {
        var result = await Sender.Send(new SearchProductsQuery(
            name,
            categoryId,
            includeInactive,
            pageNumber,
            pageSize,
            sortBy,
            isAscending), cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new CreateProductCommand(
            request.Name,
            request.Description,
            request.Price,
            request.CategoryId,
            request.StockQuantity), cancellationToken);

        return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new UpdateProductCommand(
            id,
            request.Name,
            request.Description,
            request.Price,
            request.CategoryId,
            request.StockQuantity,
            request.IsActive), cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new DeleteProductCommand(id), cancellationToken);
        return NoContent();
    }
}

public record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    int StockQuantity);

public record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    int StockQuantity,
    bool IsActive);

