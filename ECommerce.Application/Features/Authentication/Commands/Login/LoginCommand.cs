using AutoMapper;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Interfaces.Authentication;
using ECommerce.Application.Common.Interfaces.Persistence;
using ECommerce.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Authentication.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenService jwtTokenService,
        IMapper mapper,
        ILogger<LoginCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _unitOfWork.Users.FirstOrDefaultAsync(
            u => u.Email == normalizedEmail,
            cancellationToken,
            u => u.RefreshTokens);

        if (user is null)
        {
            throw new ValidationException("Invalid credentials.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new ValidationException("Invalid credentials.");
        }

        // revoke expired tokens
        foreach (var token in user.RefreshTokens.Where(t => t.IsExpired && !t.IsRevoked))
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        var tokenResult = await _jwtTokenService.GenerateTokensAsync(user, cancellationToken);

        user.RefreshTokens.Add(tokenResult.RefreshToken);
        await _unitOfWork.RefreshTokens.AddAsync(tokenResult.RefreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Email} logged in.", user.Email);

        var userDto = _mapper.Map<UserDto>(user);

        return new AuthResponseDto(
            tokenResult.AccessToken,
            tokenResult.AccessTokenExpiresAt,
            tokenResult.RefreshToken.Token,
            tokenResult.RefreshToken.ExpiresAt,
            userDto);
    }
}

