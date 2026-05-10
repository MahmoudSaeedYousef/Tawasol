using Tawasol.Domain.Enums;
using Tawasol.Domain.Exceptions;

namespace Tawasol.Domain.Entities;

public class Donation
{
    public Guid Id { get; private set; }
    public Guid DonorId { get; private set; }
    public DonationType Type { get; private set; }
    public DonationStatus Status { get; private set; }
    public decimal? Amount { get; private set; }
    
    // Relations
    public Guid? CaseId { get; private set; }
    public Guid? CaseItemId { get; private set; }
    public Guid? WalletId { get; private set; }
    
    public string? ProofPictureUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Donation() { }

    // Financial Donation Factory
    public static Donation CreateFinancial(Guid donorId, decimal amount, string proofPictureUrl, Guid? caseId = null, Guid? walletId = null)
    {
        if (amount <= 0) throw new DomainException("Donation amount must be positive.");
        if (caseId == null && walletId == null) throw new DomainException("Financial donation must target a case or a wallet.");

        return new Donation
        {
            Id = Guid.NewGuid(),
            DonorId = donorId,
            Type = DonationType.Financial,
            Status = DonationStatus.Pending,
            Amount = amount,
            CaseId = caseId,
            WalletId = walletId,
            ProofPictureUrl = proofPictureUrl,
            CreatedAt = DateTime.UtcNow
        };
    }

    // In-Kind Donation Factory
    public static Donation CreateInKind(Guid donorId, Guid caseItemId)
    {
        return new Donation
        {
            Id = Guid.NewGuid(),
            DonorId = donorId,
            Type = DonationType.InKind,
            Status = DonationStatus.Pending,
            CaseItemId = caseItemId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Verify()
    {
        if (Status != DonationStatus.Pending) throw new DomainException("Only pending donations can be verified.");
        Status = DonationStatus.Verified;
    }

    public void MarkAsDelivered()
    {
        if (Status != DonationStatus.Verified) throw new DomainException("Only verified donations can be marked as delivered.");
        Status = DonationStatus.Delivered;
    }
}
