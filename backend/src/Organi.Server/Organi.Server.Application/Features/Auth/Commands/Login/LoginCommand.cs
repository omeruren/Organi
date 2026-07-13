using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Auth.DTOs;

namespace Organi.Server.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;
