using Tawasol.Domain.Enums;
using Tawasol.Domain.Exceptions;

namespace Tawasol.Domain.Entities;

public class Case
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public decimal TargetAmount { get; private set; }
    public decimal CollectedAmount { get; private set; }
    public CaseStatus Status { get; private set; }
    public string CaseType { get; private set; }
    public string? RejectionReason { get; private set; }
    private Dictionary<string, string> _extraDetails = new();
    public IReadOnlyDictionary<string, string> ExtraDetails => _extraDetails;
    public DateTime CreatedAt { get; private set; }

    private readonly List<CaseAttachment> _attachments = new();
    public IReadOnlyCollection<CaseAttachment> Attachments => _attachments.AsReadOnly();

    private readonly List<CaseItem> _items = new();
    public IReadOnlyCollection<CaseItem> Items => _items.AsReadOnly();

    public VerificationReport? ResearchReport { get; private set; }

    private Case() { } // EF Core needs this

    public Case(string title, string description, decimal targetAmount, string caseType, Dictionary<string, string>? extraDetails = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Case title cannot be empty.");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Case description cannot be empty.");
        if (targetAmount <= 0)
            throw new DomainException("Target amount must be positive.");
        if (string.IsNullOrWhiteSpace(caseType))
            throw new DomainException("Case type is required.");

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        TargetAmount = targetAmount;
        CaseType = caseType;
        Status = CaseStatus.Pending;
        CollectedAmount = 0;
        _extraDetails = extraDetails ?? new();
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(string name, string description, CaseItemType type, decimal? amount = null)
    {
        _items.Add(new CaseItem(Id, name, description, type, amount));
    }

    public void AddAttachment(string fileName, string filePath, string fileType)
    {
        _attachments.Add(new CaseAttachment(Id, fileName, filePath, fileType));
    }

    public void AddContribution(decimal amount)
    {
        if (amount <= 0) throw new DomainException("Contribution amount must be positive.");
        if (Status != CaseStatus.Published) throw new DomainException("Only published cases can receive contributions.");

        CollectedAmount += amount;

        if (CollectedAmount >= TargetAmount)
        {
            Status = CaseStatus.Fulfilled;
        }
    }

    public void SubmitResearch(Guid researcherId, string fieldNotes, bool isUrgent)
    {
        // Business Rule: Only Pending or NeedsResearch cases can be researched
        if (Status != CaseStatus.Pending && Status != CaseStatus.NeedsResearch)
            throw new DomainException("Case is not in a state that allows research submission.");

        var report = new VerificationReport(Id, researcherId, fieldNotes, isUrgent);
        TransitionTo(CaseStatus.Researched, report: report);
    }

    public void TransitionTo(CaseStatus nextStatus, string? rejectionReason = null, VerificationReport? report = null)
    {
        switch (nextStatus)
        {
            case CaseStatus.Researched:
                if (report == null)
                    throw new DomainException("A Case cannot move to Researched status without a VerificationReport.");
                ResearchReport = report;
                Status = CaseStatus.Researched;
                break;

            case CaseStatus.Published:
                if (Status != CaseStatus.Researched)
                    throw new DomainException("Only researched cases can be published.");
                Status = CaseStatus.Published;
                RejectionReason = null;
                break;

            case CaseStatus.Rejected:
                if (string.IsNullOrWhiteSpace(rejectionReason))
                    throw new DomainException("A rejection reason must be provided.");
                Status = CaseStatus.Rejected;
                RejectionReason = rejectionReason;
                break;

            default:
                throw new DomainException($"Invalid status transition to {nextStatus}");
        }
    }

    public void UpdateExtraDetails(Dictionary<string, string> details)
    {
        _extraDetails = details ?? throw new DomainException("Details cannot be null.");
    }
}
