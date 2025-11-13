using AutoMapper;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Interfaces.Persistence;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Orders.Queries.GetCustomerOrders;

public record GetCustomerOrdersQuery(Guid CustomerId, int PageNumber, int PageSize) : IRequest<PagedResult<OrderDto>>;

public class GetCustomerOrdersQueryValidator : AbstractValidator<GetCustomerOrdersQuery>
{
    public GetCustomerOrdersQueryValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public class GetCustomerOrdersQueryHandler : IRequestHandler<GetCustomerOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCustomerOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetCustomerOrdersQuery request, CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await _unitOfWork.Orders.GetPagedAsync(
            o => o.CustomerId == request.CustomerId,
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

