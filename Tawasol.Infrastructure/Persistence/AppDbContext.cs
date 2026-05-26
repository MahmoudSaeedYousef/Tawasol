using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tawasol.Domain.Entities;
using Tawasol.Infrastructure.Identity;

namespace Tawasol.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    // Domain Entities
    public DbSet<User> DomainUsers => Set<User>();
    public DbSet<Case> Cases => Set<Case>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<VerificationReport> VerificationReports => Set<VerificationReport>();
    public DbSet<Donation> Donations => Set<Donation>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<CaseItem> CaseItems => Set<CaseItem>();
    public DbSet<InKindDonation> InKindDonations => Set<InKindDonation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map the domain User to the "Users" table
        modelBuilder.Entity<User>().ToTable("Users");
        
        // Identity tables will be mapped by IdentityDbContext to AspNet* tables by default.

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
