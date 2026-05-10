namespace Tawasol.Application.DTOs.Admin;

public record WalletBalanceDto(string Category, decimal Balance);

public record FinanceSummaryDto(
    List<WalletBalanceDto> Wallets,
    int ActiveCasesCount,
    decimal TotalCollectedAmount);
