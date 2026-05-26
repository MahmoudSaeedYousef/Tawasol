using System.Collections.Generic;
using System.Linq;
using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Users;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Users.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler(
        IUserRepository userRepository,
        ITransactionRepository transactionRepository)
        : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
    {
        public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken ct)
        {
            var user = await userRepository.GetByIdAsync(request.UserId, ct);
            if (user == null) return Result<UserProfileDto>.Failure("User not found.");

            var transactions = await transactionRepository.GetByDonorIdAsync(user.Id, ct);
            var donationsCount = transactions.Count();

            var badges = new List<string>();
            if (user.Role == UserRole.Researcher && user.VerifiedDeliveriesCount > 10)
            {
                badges.Add("حارس القرية");
            }

            var profile = new UserProfileDto(
                user.Id,
                user.FullName,
                user.PhoneNumber,
                user.Points,
                user.GetTitle(),
                donationsCount,
                badges);

            return Result<UserProfileDto>.Success(profile);
        }
    }
}
