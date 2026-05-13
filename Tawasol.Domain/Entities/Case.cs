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
    public CaseItemType CaseType { get; private set; } // 👈 تغيير النوع لـ Enum
    public string? RejectionReason { get; private set; }
    private Dictionary<string, string> _extraDetails = new();
    public IReadOnlyDictionary<string, string> ExtraDetails => _extraDetails;
    public DateTime CreatedAt { get; private set; }

    // New Administrative Fields
    public Guid CreatedBy { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public Guid? RejectedBy { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public Guid? ClosedBy { get; private set; }
    public DateTime? ClosedAt { get; private set; }


    private readonly List<CaseAttachment> _attachments = new();
    public IReadOnlyCollection<CaseAttachment> Attachments => _attachments.AsReadOnly();

    private readonly List<CaseItem> _items = new();
    public IReadOnlyCollection<CaseItem> Items => _items.AsReadOnly();

    public VerificationReport? ResearchReport { get; private set; }

    private Case()
    {
    } // EF Core needs this

    public Case(string title, string description, decimal targetAmount, CaseItemType caseType, Guid createdBy,
        Dictionary<string, string>? extraDetails = null)
    {
        // if (string.IsNullOrWhiteSpace(title))
        //     throw new DomainException("Case title cannot be empty.");
        // if (string.IsNullOrWhiteSpace(description))
        //     throw new DomainException("Case description cannot be empty.");
        // if (targetAmount <= 0)
        //     throw new DomainException("Target amount must be positive.");
        //

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        TargetAmount = targetAmount;
        CaseType = caseType;
        Status = CaseStatus.Pending;
        CollectedAmount = 0;
        _extraDetails = extraDetails ?? new();
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy; // Initialize CreatedBy
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
        if (Status != CaseStatus.Published)
            throw new DomainException("Only published cases can receive contributions.");
        if (Status == CaseStatus.Fulfilled || Status == CaseStatus.Closed)
            throw new DomainException("Case is already fulfilled or closed.");

        CollectedAmount += amount;

        if (CollectedAmount >= TargetAmount)
        {
            TransitionTo(CaseStatus.Fulfilled); // Transition to Fulfilled
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

    public void TransitionTo(CaseStatus nextStatus, string? rejectionReason = null, VerificationReport? report = null,
        Guid? actorId = null)
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
                ApprovedBy = actorId; // Set ApprovedBy
                ApprovedAt = DateTime.UtcNow; // Set ApprovedAt
                break;

            case CaseStatus.Rejected:
                if (string.IsNullOrWhiteSpace(rejectionReason))
                    throw new DomainException("A rejection reason must be provided.");
                Status = CaseStatus.Rejected;
                RejectionReason = rejectionReason;
                RejectedBy = actorId; // Set RejectedBy
                RejectedAt = DateTime.UtcNow; // Set RejectedAt
                break;

            case CaseStatus.Fulfilled:
                if (CollectedAmount < TargetAmount)
                    throw new DomainException("Case cannot be fulfilled if collected amount is less than target.");
                Status = CaseStatus.Fulfilled;
                CloseCase(actorId); // Automatically close when fulfilled
                break;

            default:
                throw new DomainException($"Invalid status transition to {nextStatus}");
        }
    }

    public void CloseCase(Guid? actorId = null)
    {
        if (Status == CaseStatus.Closed) throw new DomainException("Case is already closed.");
        Status = CaseStatus.Closed;
        ClosedBy = actorId;
        ClosedAt = DateTime.UtcNow;
    }

    public void UpdateExtraDetails(Dictionary<string, string> details)
    {
        _extraDetails = details ?? throw new DomainException("Details cannot be null.");
    }

}
