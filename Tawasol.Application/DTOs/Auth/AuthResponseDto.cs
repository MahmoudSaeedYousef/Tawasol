namespace Tawasol.Application.DTOs.Auth;

public record AuthResponseDto(
    string Id,             // 👈 ضيف ده ضروري
    string Token,
    string RefreshToken,
    string FullName,
    string PhoneNumber,
    string Role,
    int Points,
    string RankTitle     // 👈 ضيف ده عشان يظهر "جار الخير" أو غيره
);