using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Auth;

namespace Tawasol.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string ExpiredToken, string RefreshToken) : IRequest<Result<AuthResponseDto>>;
