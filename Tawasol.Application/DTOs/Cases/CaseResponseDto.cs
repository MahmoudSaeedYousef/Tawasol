namespace Tawasol.Application.DTOs.Cases;

public class CaseResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CollectedAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CaseType { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public Dictionary<string, string> ExtraDetails { get; set; } = new();
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public Guid? ApprovedBy { get; set; } 
    public DateTime? ApprovedAt { get; set; } 
    public Guid? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public Guid? ClosedBy { get; set; }
    public DateTime? ClosedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public List<string> AttachmentUrls { get; set; } = new();
}
