using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tawasol.Domain.Entities;

namespace Tawasol.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
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
        // 1. 🚀 تشغيل مابينج الـ Identity الأساسي أولاً (خطوة إجبارية)
        base.OnModelCreating(modelBuilder);
    
        // 2. 🚀 إلزام الـ EF بالأسماء الموحدة النظيفة لمنع التضارب
        modelBuilder.Entity<User>(b => b.ToTable("AspNetUsers"));
        modelBuilder.Entity<IdentityRole<Guid>>(b => b.ToTable("AspNetRoles"));
        modelBuilder.Entity<IdentityUserRole<Guid>>(b => b.ToTable("AspNetUserRoles"));
        modelBuilder.Entity<IdentityUserClaim<Guid>>(b => b.ToTable("AspNetUserClaims"));
        modelBuilder.Entity<IdentityUserLogin<Guid>>(b => b.ToTable("AspNetUserLogins"));
        modelBuilder.Entity<IdentityRoleClaim<Guid>>(b => b.ToTable("AspNetRoleClaims"));
        modelBuilder.Entity<IdentityUserToken<Guid>>(b => b.ToTable("AspNetUserTokens"));

        // 3. تطبيق مابينج الجداول الخاصة بالبيزنس بتاعتك
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
