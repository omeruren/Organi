using MediatR;
using Organi.Server.Application.Features.Auth.DTOs;

namespace Organi.Server.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber) : IRequest<AuthResponse>;
