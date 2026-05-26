using Tawasol.Domain.Enums;

namespace Tawasol.Application.DTOs.Donations;

public record PledgeRequestDto(Guid CaseId, Guid CaseItemId, ItemCondition Condition, string? EvidencePhotoUrl)
{
    public int Quantity { get; set; } = 0;
}
