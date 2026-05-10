namespace Tawasol.Application.DTOs.Cases;

public record SubmitFieldReportRequestDto(
    string FieldNotes,
    bool IsUrgent);
