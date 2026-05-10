namespace Tawasol.Application.Interfaces.Services;

public interface IFcmService
{
    Task SendNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null);
}
