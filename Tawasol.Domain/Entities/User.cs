using Microsoft.AspNetCore.Identity;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Tawasol.Domain.Exceptions; // تأكد من الـ Namespace الخاص بالـ DomainException بتاعك

namespace Tawasol.Domain.Entities;

public class User : IdentityUser<Guid>
{

    public string FullName { get; private set; } = string.Empty;
    public int Points { get; private set; }
    public int VerifiedDeliveriesCount { get; private set; }
    public string? DeviceToken { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User() : base() { }

    public User(string fullName, string phoneNumber, UserRole role) : base()
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name is required.");
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Phone number is required.");

        Id = Guid.NewGuid();
        FullName = fullName;
        
        // 🚀 الـ Identity يعتمد على الـ UserName والـ PhoneNumber
        PhoneNumber = phoneNumber;
        UserName = phoneNumber; // نستخدم رقم الهاتف كـ اسـم مستخدم لسهولة اللوجن
        
        Role = role;
        Points = 0;
        VerifiedDeliveriesCount = 0;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDeviceToken(string token)
    {
        DeviceToken = token;
    }

    public string GetTitle()
    {
        return Points switch
        {
            <= 100 => "جار الخير",
            <= 500 => "سند القرية",
            _ => "كريم القرية"
        };
    }

    public void AddPoints(int amount)
    {
        if (amount < 0) throw new DomainException("Points cannot be negative.");
        Points += amount;
    }

    public void IncrementVerifiedDeliveries()
    {
        VerifiedDeliveriesCount++;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
