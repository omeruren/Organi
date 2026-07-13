using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Auth.DTOs;

namespace Organi.Server.Application.Features.Auth.Commands.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;
