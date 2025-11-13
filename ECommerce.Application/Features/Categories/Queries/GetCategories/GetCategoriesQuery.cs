using AutoMapper;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Interfaces.Persistence;
using MediatR;

namespace ECommerce.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public GetCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = "categories:all";

        var cached = await _cacheService.GetAsync<IReadOnlyList<CategoryDto>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var categories = await _unitOfWork.Categories.ListAsync(null, cancellationToken);
        var categoryDtos = _mapper.Map<IReadOnlyList<CategoryDto>>(categories);

        await _cacheService.SetAsync(cacheKey, categoryDtos, TimeSpan.FromMinutes(10), cancellationToken);

        return categoryDtos;
    }
}

