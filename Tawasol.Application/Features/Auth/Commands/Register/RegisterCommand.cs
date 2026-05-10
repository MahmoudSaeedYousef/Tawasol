using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Auth;

namespace Tawasol.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string FullName,
    string PhoneNumber,
    string Password,
    string Role) : IRequest<Result<AuthResponseDto>>;
