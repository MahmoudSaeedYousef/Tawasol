using Tawasol.Domain.Enums;
using Tawasol.Domain.Exceptions;

namespace Tawasol.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid DonorId { get; private set; }
    public Guid? CaseId { get; private set; } // Nullable for general donations
    public decimal Amount { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string ProofPictureUrl { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public string ReferenceNumber { get; set; }

    private Transaction() { }

    public Transaction(Guid donorId, decimal amount, string proofPictureUrl, Guid? caseId = null)
    {
        if (amount <= 0)
            throw new DomainException("Transaction amount must be positive.");
        if (string.IsNullOrWhiteSpace(proofPictureUrl))
            throw new DomainException("Proof picture URL is required.");

        Id = Guid.NewGuid();
        DonorId = donorId;
        CaseId = caseId;
        Amount = amount;
        ProofPictureUrl = proofPictureUrl;
        Status = TransactionStatus.Pending;
        TransactionDate = DateTime.UtcNow;
    }

    public void Verify()
    {
        if (Status != TransactionStatus.Pending)
            throw new DomainException("Only pending transactions can be verified.");
        Status = TransactionStatus.Verified;
    }

    public void Reject()
    {
        if (Status != TransactionStatus.Pending)
            throw new DomainException("Only pending transactions can be rejected.");
        Status = TransactionStatus.Rejected;
    }
}
