using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Domain.Interfaces.Repositories;
using Tawasol.Domain.Interfaces;

namespace Tawasol.Application.Features.Users.Commands.UpdateDeviceToken;

public class UpdateDeviceTokenCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateDeviceTokenCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateDeviceTokenCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, ct);
        if (user == null) return Result<bool>.Failure("User not found.");

        user.UpdateDeviceToken(request.DeviceToken);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<bool>.Success(true, "Device token updated.");
    }
}
