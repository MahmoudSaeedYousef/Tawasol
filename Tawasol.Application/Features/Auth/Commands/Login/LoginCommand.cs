using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Auth;

namespace Tawasol.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string PhoneNumber,
    string Password) : IRequest<Result<AuthResponseDto>>;
