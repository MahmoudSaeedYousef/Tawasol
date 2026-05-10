using Tawasol.Domain.Enums;
using Tawasol.Domain.Exceptions;

namespace Tawasol.Domain.Entities;

public class CaseItem
{
    public Guid Id { get; private set; }
    public Guid CaseId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public CaseItemType Type { get; private set; }
    public decimal? TargetAmount { get; private set; }
    public bool IsPledged { get; private set; }
    public Guid? PledgedByDonorId { get; private set; }

    private CaseItem() { }

    public CaseItem(Guid caseId, string name, string description, CaseItemType type, decimal? targetAmount = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Item name is required.");
        if (type == CaseItemType.Monetary && (!targetAmount.HasValue || targetAmount <= 0))
            throw new DomainException("Monetary items must have a positive target amount.");

        Id = Guid.NewGuid();
        CaseId = caseId;
        Name = name;
        Description = description;
        Type = type;
        TargetAmount = targetAmount;
        IsPledged = false;
    }

    public void Pledge(Guid donorId)
    {
        if (Type != CaseItemType.PhysicalItem)
            throw new DomainException("Only physical items can be pledged.");
        if (IsPledged)
            throw new DomainException("This item is already pledged.");

        IsPledged = true;
        PledgedByDonorId = donorId;
    }
}
