using AutoMapper;
using ECommerce.Application.Common.DTOs;
using ECommerce.Application.Common.Interfaces.Authentication;
using ECommerce.Application.Common.Interfaces.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommerce.Application.Features.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<AuthResponseDto>;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty();

        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        IMapper mapper,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken)
            ?? throw new ValidationException("Invalid access token.");

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        var subjectClaim = principal.FindFirst("sub");
        var identifier = subjectClaim?.Value ?? userIdClaim?.Value;

        if (identifier is null || !Guid.TryParse(identifier, out var userId))
        {
            throw new ValidationException("Invalid access token.");
        }

        var user = await _unitOfWork.Users.FirstOrDefaultAsync(
            u => u.Id == userId,
            cancellationToken,
            u => u.RefreshTokens);

        if (user is null)
        {
            throw new ValidationException("User not found.");
        }

        var existingToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == request.RefreshToken);

        if (existingToken is null)
        {
            throw new ValidationException("Refresh token not found.");
        }

        if (existingToken.IsRevoked || existingToken.IsExpired)
        {
            throw new ValidationException("Refresh token is no longer valid.");
        }

        existingToken.RevokedAt = DateTime.UtcNow;

        var tokenResult = await _jwtTokenService.GenerateTokensAsync(user, cancellationToken);
        existingToken.ReplacedByToken = tokenResult.RefreshToken.Token;

        user.RefreshTokens.Add(tokenResult.RefreshToken);
        await _unitOfWork.RefreshTokens.AddAsync(tokenResult.RefreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token rotated for user {Email}", user.Email);

        var userDto = _mapper.Map<UserDto>(user);

        return new AuthResponseDto(
            tokenResult.AccessToken,
            tokenResult.AccessTokenExpiresAt,
            tokenResult.RefreshToken.Token,
            tokenResult.RefreshToken.ExpiresAt,
            userDto);
    }
}

