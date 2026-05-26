using Tawasol.Domain.Enums;
using Tawasol.Domain.Exceptions;

namespace Tawasol.Domain.Entities;

public class InKindDonation
{
    public Guid Id { get; private set; }
    public Guid DonorId { get; private set; }
    public Guid CaseItemId { get; private set; }
    public ItemCondition ItemCondition { get; private set; }
    public int Quantity { get; set; } = 0;
    public DonationStatus Status { get; private set; }
    public string? EvidencePhotoUrl { get; private set; }
    public string? DeliveryPhotoUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private InKindDonation() { }

    public InKindDonation(Guid donorId, Guid caseItemId, ItemCondition itemCondition,int quantity, string? evidencePhotoUrl = null)
    {
        Id = Guid.NewGuid();
        DonorId = donorId;
        CaseItemId = caseItemId;
        ItemCondition = itemCondition;
        Quantity = quantity;
        EvidencePhotoUrl = evidencePhotoUrl;
        Status = DonationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Verify()
    {
        if (Status != DonationStatus.Pending) throw new DomainException("Only pending donations can be verified.");
        Status = DonationStatus.Verified;
    }

    public void MarkAsDelivered(string deliveryPhotoUrl)
    {
        if (Status != DonationStatus.Verified && Status != DonationStatus.Pending) throw new DomainException("Donation must be verified or pending to be marked as delivered.");
        if (string.IsNullOrWhiteSpace(deliveryPhotoUrl)) throw new DomainException("A delivery photo is required to mark as delivered.");
        
        DeliveryPhotoUrl = deliveryPhotoUrl;
        Status = DonationStatus.Delivered;
    }

    public void Reject()
    {
        if (Status != DonationStatus.Pending) throw new DomainException("Only pending donations can be rejected.");
        Status = DonationStatus.Rejected;
    }
}
