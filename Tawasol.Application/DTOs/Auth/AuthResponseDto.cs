namespace Tawasol.Application.DTOs.Auth;

public record AuthResponseDto(
    string Token,
    string FullName,
    string PhoneNumber,
    string Role,
    int Points);
