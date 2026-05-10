using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Tawasol.Application.Interfaces.Services;

namespace Tawasol.Infrastructure.Services;

public class FcmService : IFcmService
{
    public FcmService(IConfiguration configuration)
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            var jsonPath = configuration["Firebase:ServiceAccountPath"];
            if (!string.IsNullOrEmpty(jsonPath) && File.Exists(jsonPath))
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(jsonPath)
                });
            }
        }
    }

    public async Task SendNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null)
    {
        if (string.IsNullOrEmpty(deviceToken) || FirebaseApp.DefaultInstance == null) return;

        var message = new Message()
        {
            Token = deviceToken,
            Notification = new FirebaseAdmin.Messaging.Notification()
            {
                Title = title,
                Body = body
            },
            Data = data
        };

        await FirebaseMessaging.DefaultInstance.SendAsync(message);
    }
}
