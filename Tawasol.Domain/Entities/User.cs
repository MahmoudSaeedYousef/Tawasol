using Tawasol.Domain.Enums;
using Tawasol.Domain.Exceptions;

namespace Tawasol.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }
    public string PhoneNumber { get; private set; }
    public int Points { get; private set; }
    public string? DeviceToken { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public User(string fullName, string phoneNumber, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name is required.");
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Phone number is required.");

        Id = Guid.NewGuid();
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Role = role;
        Points = 0;
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

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
