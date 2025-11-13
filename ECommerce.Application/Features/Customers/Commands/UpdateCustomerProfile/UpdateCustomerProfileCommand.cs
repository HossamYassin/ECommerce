using AutoMapper;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Interfaces.Persistence;
using ECommerce.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Customers.Commands.UpdateCustomerProfile;

public record UpdateCustomerProfileCommand(
    Guid CustomerId,
    string Name,
    string Email,
    string? NewPassword) : IRequest<UserDto>;

public class UpdateCustomerProfileCommandValidator : AbstractValidator<UpdateCustomerProfileCommand>
{
    public UpdateCustomerProfileCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        When(x => !string.IsNullOrWhiteSpace(x.NewPassword), () =>
        {
            RuleFor(x => x.NewPassword!)
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
        });
    }
}

public class UpdateCustomerProfileCommandHandler : IRequestHandler<UpdateCustomerProfileCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCustomerProfileCommandHandler> _logger;

    public UpdateCustomerProfileCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        IMapper mapper,
        ILogger<UpdateCustomerProfileCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> Handle(UpdateCustomerProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.CustomerId, cancellationToken);
        if (user is null)
        {
            throw new ValidationException("Customer not found.");
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (!string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
        {
            var emailInUse = await _unitOfWork.Users.ExistsAsync(
                u => u.Email.ToLower() == normalizedEmail && u.Id != request.CustomerId,
                cancellationToken);

            if (emailInUse)
            {
                throw new ValidationException("The email address is already in use.");
            }

            user.Email = normalizedEmail;
        }

        user.Name = request.Name.Trim();

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        }

        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer {CustomerId} updated their profile.", user.Id);

        return _mapper.Map<UserDto>(user);
    }
}

