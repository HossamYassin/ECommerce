using AutoMapper;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Interfaces.Persistence;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Customers.Queries.GetCustomerProfile;

public record GetCustomerProfileQuery(Guid CustomerId) : IRequest<UserDto>;

public class GetCustomerProfileQueryValidator : AbstractValidator<GetCustomerProfileQuery>
{
    public GetCustomerProfileQueryValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}

public class GetCustomerProfileQueryHandler : IRequestHandler<GetCustomerProfileQuery, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCustomerProfileQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.CustomerId, cancellationToken);
        if (user is null)
        {
            throw new ValidationException("Customer not found.");
        }

        return _mapper.Map<UserDto>(user);
    }
}

