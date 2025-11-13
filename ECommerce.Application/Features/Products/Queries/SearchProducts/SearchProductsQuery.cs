using AutoMapper;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Interfaces.Persistence;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using FluentValidation;
using MediatR;
using System.Linq.Expressions;
using ECommerce.Application.Common.Extensions;

namespace ECommerce.Application.Features.Products.Queries.SearchProducts;

public record SearchProductsQuery(
    string? Name,
    Guid? CategoryId,
    bool IncludeInactive,
    int PageNumber,
    int PageSize,
    string? SortBy,
    bool IsAscending) : IRequest<PagedResult<ProductDto>>;

public class SearchProductsQueryValidator : AbstractValidator<SearchProductsQuery>
{
    public SearchProductsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Name).MaximumLength(200);
    }
}

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, PagedResult<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SearchProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Product, bool>> predicate = product => true;

        if (!request.IncludeInactive)
        {
            predicate = predicate.And(product => product.IsActive && !product.IsDeleted);
        }

        if (request.CategoryId.HasValue)
        {
            var categoryId = request.CategoryId.Value;
            predicate = predicate.And(product => product.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim().ToLowerInvariant();
            predicate = predicate.And(product => product.Name.ToLower().Contains(name));
        }

        var orderBy = BuildOrderBy(request.SortBy, request.IsAscending);

        var (products, totalCount) = await _unitOfWork.Products.GetPagedAsync(
            predicate,
            request.PageNumber,
            request.PageSize,
            orderBy,
            cancellationToken,
            product => product.Category);

        var items = _mapper.Map<IReadOnlyList<ProductDto>>(products);

        return new PagedResult<ProductDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    private static Func<IQueryable<Product>, IOrderedQueryable<Product>> BuildOrderBy(string? sortBy, bool isAscending)
    {
        sortBy = sortBy?.ToLowerInvariant();

        return sortBy switch
        {
            "name" => query => isAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
            "price" => query => isAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
            "stock" => query => isAscending ? query.OrderBy(p => p.StockQuantity) : query.OrderByDescending(p => p.StockQuantity),
            _ => query => isAscending ? query.OrderBy(p => p.CreatedDate) : query.OrderByDescending(p => p.CreatedDate)
        };
    }
}

