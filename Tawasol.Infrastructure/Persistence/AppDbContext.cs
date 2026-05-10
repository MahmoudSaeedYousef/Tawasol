using Microsoft.EntityFrameworkCore;
using Tawasol.Domain.Entities;

namespace Tawasol.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Case> Cases => Set<Case>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<VerificationReport> VerificationReports => Set<VerificationReport>();
    public DbSet<Donation> Donations => Set<Donation>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<CaseItem> CaseItems => Set<CaseItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
