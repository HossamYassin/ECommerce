using AutoMapper;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Interfaces.Persistence;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.FirstOrDefaultAsync(
            p => p.Id == request.Id,
            cancellationToken,
            p => p.Category);

        if (product is null)
        {
            throw new ValidationException("Product not found.");
        }

        return _mapper.Map<ProductDto>(product);
    }
}

