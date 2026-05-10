using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Auth;

namespace Tawasol.Application.Interfaces.Services;

public interface IIdentityService
{
    Task<Result<AuthResponseDto>> RegisterAsync(string fullName, string phoneNumber, string password, string role);
    Task<Result<AuthResponseDto>> LoginAsync(string phoneNumber, string password);
}
