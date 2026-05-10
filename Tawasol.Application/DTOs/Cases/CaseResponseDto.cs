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
    public Dictionary<string, string> ExtraDetails { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
