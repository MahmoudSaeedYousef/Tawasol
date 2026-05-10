namespace Tawasol.Application.DTOs.Auth;

public record RefreshTokenRequestDto(string ExpiredToken, string RefreshToken);
