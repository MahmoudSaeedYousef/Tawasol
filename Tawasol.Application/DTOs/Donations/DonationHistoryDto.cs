namespace Tawasol.Application.DTOs.Donations;

public record DonationHistoryDto(
    Guid Id,
    string Type, // "Financial" or "InKind"
    decimal? Amount,
    string Status,
    DateTime Date,
    string? CaseTitle,
    string? ItemName,
    string? DeliveryPhotoUrl
);
