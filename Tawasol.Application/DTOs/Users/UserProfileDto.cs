namespace Tawasol.Application.DTOs.Users;

public record UserProfileDto(
    Guid Id,
    string FullName,
    string PhoneNumber,
    int Points,
    string Title,
    int TotalDonationsCount);
