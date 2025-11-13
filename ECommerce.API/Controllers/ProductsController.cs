using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Products.Queries.GetProductById;
using ECommerce.Application.Features.Products.Queries.SearchProducts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class ProductsController : ApiControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? name,
        [FromQuery] Guid? categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool isAscending = false,
        CancellationToken cancellationToken = default)
    {
        var result = await Sender.Send(new SearchProductsQuery(
            name,
            categoryId,
            IncludeInactive: false,
            PageNumber: pageNumber,
            PageSize: pageSize,
            SortBy: sortBy,
            IsAscending: isAscending), cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetProductByIdQuery(id), cancellationToken);
        return Ok(result);
    }
}

