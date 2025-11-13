using AutoMapper;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Interfaces.Persistence;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Common.Extensions;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using FluentValidation;
using MediatR;
using System.Linq.Expressions;

namespace ECommerce.Application.Features.Orders.Queries.GetOrders;

public record GetOrdersQuery(
    int PageNumber,
    int PageSize,
    string? Status,
    Guid? CustomerId) : IRequest<PagedResult<OrderDto>>;

public class GetOrdersQueryValidator : AbstractValidator<GetOrdersQuery>
{
    public GetOrdersQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Order, bool>> predicate = order => true;

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<OrderStatus>(request.Status, true, out var status))
        {
            predicate = predicate.And(order => order.Status == status);
        }

        if (request.CustomerId.HasValue)
        {
            var customerId = request.CustomerId.Value;
            predicate = predicate.And(order => order.CustomerId == customerId);
        }

        var (orders, totalCount) = await _unitOfWork.Orders.GetPagedAsync(
            predicate,
            request.PageNumber,
            request.PageSize,
            query => query.OrderByDescending(o => o.CreatedDate),
            cancellationToken,
            o => o.Customer,
            o => o.Items);

        await PopulateOrderItemProductsAsync(orders, cancellationToken);

        var orderDtos = _mapper.Map<IReadOnlyList<OrderDto>>(orders);

        return new PagedResult<OrderDto>
        {
            Items = orderDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    private async Task PopulateOrderItemProductsAsync(IReadOnlyList<Order> orders, CancellationToken cancellationToken)
    {
        var productIds = orders
            .SelectMany(o => o.Items)
            .Select(i => i.ProductId)
            .Distinct()
            .ToList();

        if (productIds.Count == 0)
        {
            return;
        }

        var products = await _unitOfWork.Products.ListAsync(
            p => productIds.Contains(p.Id),
            cancellationToken);
        var lookup = products.ToDictionary(p => p.Id);

        foreach (var order in orders)
        {
            foreach (var item in order.Items)
            {
                if (lookup.TryGetValue(item.ProductId, out var product))
                {
                    item.Product = product;
                }
            }
        }
    }
}

