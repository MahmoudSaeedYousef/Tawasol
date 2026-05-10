using Tawasol.Domain.Enums;
using Tawasol.Domain.Exceptions;

namespace Tawasol.Domain.Entities;

public class Wallet
{
    public Guid Id { get; private set; }
    public WalletCategory Category { get; private set; }
    public decimal Balance { get; private set; }

    private Wallet() { }

    public Wallet(WalletCategory category)
    {
        Id = Guid.NewGuid();
        Category = category;
        Balance = 0;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new DomainException("Deposit amount must be positive.");
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0) throw new DomainException("Withdrawal amount must be positive.");
        if (amount > Balance) throw new DomainException("Insufficient balance.");
        Balance -= amount;
    }
}
