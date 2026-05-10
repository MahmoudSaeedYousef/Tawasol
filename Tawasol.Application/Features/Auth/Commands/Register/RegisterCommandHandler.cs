using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Auth;
using Tawasol.Application.Interfaces.Services;

namespace Tawasol.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(IIdentityService identityService)
    : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken ct)
    {
        return await identityService.RegisterAsync(request.FullName, request.PhoneNumber, request.Password, request.Role);
    }
}
