using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Cases.Commands.ApproveCase;

public class ApproveCaseCommandHandler(
    ICaseRepository caseRepository,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    IFcmService fcmService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ApproveCaseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ApproveCaseCommand request, CancellationToken ct)
    {
        var @case = await caseRepository.GetByIdAsync(request.CaseId, ct);
        if (@case == null)
            return Result<bool>.Failure("Case not found.");

        @case.TransitionTo(CaseStatus.Published);
        
        // 1. Notify Researchers or potentially all users?
        // For this scenario, let's assume we notify the researcher if one was assigned
        if (@case.ResearchReport != null)
        {
            var researcher = await userRepository.GetByIdAsync(@case.ResearchReport.ResearcherId, ct);
            if (researcher != null)
            {
                var title = "Case Published!";
                var message = $"The case you researched: '{@case.Title}' is now live.";
                
                var notification = new Notification(researcher.Id, title, message);
                await notificationRepository.AddAsync(notification, ct);

                if (!string.IsNullOrEmpty(researcher.DeviceToken))
                {
                    await fcmService.SendNotificationAsync(researcher.DeviceToken, title, message);
                }
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result<bool>.Success(true, "Case approved and published successfully.");
    }
}
