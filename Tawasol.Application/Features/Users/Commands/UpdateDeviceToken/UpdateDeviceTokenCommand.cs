using MediatR;
using Tawasol.Application.Common.Models;

namespace Tawasol.Application.Features.Users.Commands.UpdateDeviceToken;

public record UpdateDeviceTokenCommand(Guid UserId, string DeviceToken) : IRequest<Result<bool>>;
