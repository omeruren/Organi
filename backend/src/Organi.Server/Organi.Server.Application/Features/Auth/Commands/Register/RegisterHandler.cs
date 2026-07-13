using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Auth.DTOs;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Auth.Commands.Register;

public sealed class RegisterHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ILogger<RegisterHandler> logger) : IRequestHandler<RegisterCommand, AuthResponse>
{
    private const string DefaultRoleName = "Customer";

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await context.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
            throw new BusinessRuleException("Email is already registered.");

        var defaultRole = await context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Name == DefaultRoleName, cancellationToken)
            ?? throw new BusinessRuleException($"Default role '{DefaultRoleName}' is not configured.");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHasher.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            Roles = [defaultRole]
        };

        context.Users.Add(user);

        var roles = new[] { defaultRole.Name };
        var permissions = defaultRole.Permissions.Select(p => p.Name).Distinct().ToArray();

        var accessToken = tokenService.GenerateAccessToken(user, roles, permissions, vendorId: null);
        var refreshToken = tokenService.GenerateRefreshToken();

        context.RefreshTokens.Add(new RefreshToken
        {
            TokenHash = tokenService.HashToken(refreshToken.Value),
            ExpiresAt = refreshToken.ExpiresAt,
            UserId = user.Id
        });

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} registered", user.Id);

        return new AuthResponse(accessToken.Value, refreshToken.Value, accessToken.ExpiresAt);
    }
}
