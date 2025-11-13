using AutoMapper;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Interfaces.Persistence;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId, Guid RequestedByUserId, bool IsAdmin) : IRequest<OrderDto>;

public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
{
    public GetOrderByIdQueryValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.RequestedByUserId).NotEmpty();
    }
}

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.FirstOrDefaultAsync(
            o => o.Id == request.OrderId,
            cancellationToken,
            o => o.Customer,
            o => o.Items);

        if (order is null)
        {
            throw new ValidationException("Order not found.");
        }

        if (!request.IsAdmin && order.CustomerId != request.RequestedByUserId)
        {
            throw new ValidationException("You are not authorized to view this order.");
        }

        var productIds = order.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _unitOfWork.Products.ListAsync(
            p => productIds.Contains(p.Id),
            cancellationToken);
        var lookup = products.ToDictionary(p => p.Id);

        foreach (var item in order.Items)
        {
            if (lookup.TryGetValue(item.ProductId, out var product))
            {
                item.Product = product;
            }
        }

        return _mapper.Map<OrderDto>(order);
    }
}

