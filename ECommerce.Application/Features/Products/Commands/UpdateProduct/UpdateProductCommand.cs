using AutoMapper;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Interfaces.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    int StockQuantity,
    bool IsActive) : IRequest<ProductDto>;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.FirstOrDefaultAsync(
            p => p.Id == request.Id,
            cancellationToken,
            p => p.Category);

        if (product is null)
        {
            throw new ValidationException("Product not found.");
        }

        if (product.CategoryId != request.CategoryId)
        {
            var newCategory = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
            if (newCategory is null)
            {
                throw new ValidationException("Category not found.");
            }

            product.CategoryId = request.CategoryId;
            product.Category = newCategory;
        }

        var normalizedName = request.Name.Trim();
        var normalizedNameLower = normalizedName.ToLowerInvariant();

        var duplicateExists = await _unitOfWork.Products.ExistsAsync(
            p => p.Id != request.Id &&
                 p.CategoryId == request.CategoryId &&
                 p.Name.ToLower() == normalizedNameLower,
            cancellationToken);

        if (duplicateExists)
        {
            throw new ValidationException("Another product with the same name exists in this category.");
        }

        product.Name = normalizedName;
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.IsActive = request.IsActive;
        product.UpdatedDate = DateTime.UtcNow;

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {ProductId} updated.", product.Id);

        return _mapper.Map<ProductDto>(product);
    }
}

