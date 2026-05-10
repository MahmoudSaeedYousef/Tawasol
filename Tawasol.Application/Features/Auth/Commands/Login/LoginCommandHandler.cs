using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Auth;
using Tawasol.Application.Interfaces.Services;

namespace Tawasol.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(IIdentityService identityService)
    : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        return await identityService.LoginAsync(request.PhoneNumber, request.Password);
    }
}
