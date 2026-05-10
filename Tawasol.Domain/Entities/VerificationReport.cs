namespace Tawasol.Domain.Entities;

public class VerificationReport
{
    public Guid Id { get; private set; }
    public Guid CaseId { get; private set; }
    public Guid ResearcherId { get; private set; }
    public string FieldNotes { get; private set; }
    public bool IsUrgent { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private VerificationReport() { }

    public VerificationReport(Guid caseId, Guid researcherId, string fieldNotes, bool isUrgent)
    {
        Id = Guid.NewGuid();
        CaseId = caseId;
        ResearcherId = researcherId;
        FieldNotes = fieldNotes;
        IsUrgent = isUrgent;
        CreatedAt = DateTime.UtcNow;
    }
}
