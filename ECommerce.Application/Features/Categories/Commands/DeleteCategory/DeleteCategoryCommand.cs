using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Interfaces.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<Unit>;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;
    private readonly ICacheService _cacheService;

    public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCategoryCommandHandler> logger, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            throw new ValidationException("Category not found.");
        }

        var hasProducts = await _unitOfWork.Products.ExistsAsync(
            product => product.CategoryId == category.Id && product.IsActive && !product.IsDeleted,
            cancellationToken);

        if (hasProducts)
        {
            throw new ValidationException("Cannot delete category with active products.");
        }

        await _unitOfWork.Categories.DeleteAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync("categories:all", cancellationToken);

        _logger.LogInformation("Category {CategoryId} deleted.", category.Id);

        return Unit.Value;
    }
}

