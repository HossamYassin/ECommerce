using AutoMapper;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Interfaces.Persistence;
using ECommerce.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus Status) : IRequest<OrderDto>;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

    public UpdateOrderStatusCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateOrderStatusCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.FirstOrDefaultAsync(
            o => o.Id == request.OrderId,
            cancellationToken,
            o => o.Items,
            o => o.Customer);

        if (order is null)
        {
            throw new ValidationException("Order not found.");
        }

        if (order.Status == request.Status)
        {
            return _mapper.Map<OrderDto>(order);
        }

        if (request.Status == OrderStatus.Cancelled && order.Status != OrderStatus.Cancelled)
        {
            var productIds = order.Items.Select(i => i.ProductId).ToList();
            var products = await _unitOfWork.Products.ListAsync(
                p => productIds.Contains(p.Id),
                cancellationToken);
            var lookup = products.ToDictionary(p => p.Id);

            foreach (var item in order.Items)
            {
                if (lookup.TryGetValue(item.ProductId, out var product))
                {
                    product.StockQuantity += item.Quantity;
                    product.UpdatedDate = DateTime.UtcNow;
                }
            }
        }

        order.Status = request.Status;
        order.UpdatedDate = DateTime.UtcNow;

        await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} status updated to {Status}.", order.Id, order.Status);

        return _mapper.Map<OrderDto>(order);
    }
}

