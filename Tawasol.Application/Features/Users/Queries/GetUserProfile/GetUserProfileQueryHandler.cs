using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Users;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Users.Queries.GetUserProfile;

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

        var profile = new UserProfileDto(
            user.Id,
            user.FullName,
            user.PhoneNumber,
            user.Points,
            user.GetTitle(),
            donationsCount);

        return Result<UserProfileDto>.Success(profile);
    }
}
