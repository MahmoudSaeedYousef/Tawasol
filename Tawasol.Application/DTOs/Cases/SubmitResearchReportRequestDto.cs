namespace Tawasol.Application.DTOs.Cases;

public record SubmitResearchReportRequestDto(
    Guid ResearcherId,
    string FieldNotes,
    bool IsVerified);
