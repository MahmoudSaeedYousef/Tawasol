using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Users;

namespace Tawasol.Application.Features.Users.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserProfileDto>>;
