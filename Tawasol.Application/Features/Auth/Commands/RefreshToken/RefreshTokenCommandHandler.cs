using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Auth;
using Tawasol.Application.Interfaces.Services;

namespace Tawasol.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(IIdentityService identityService)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        return await identityService.RefreshTokenAsync(request.ExpiredToken, request.RefreshToken);
    }
}
