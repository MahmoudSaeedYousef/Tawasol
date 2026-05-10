using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Auth;
using Tawasol.Application.Interfaces.Services;

namespace Tawasol.Infrastructure.Identity;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration) : IIdentityService
{
    public async Task<Result<AuthResponseDto>> RegisterAsync(string fullName, string phoneNumber, string password, string role)
    {
        var user = new ApplicationUser
        {
            UserName = phoneNumber,
            PhoneNumber = phoneNumber,
            FullName = fullName,
            Points = 0
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return Result<AuthResponseDto>.Failure(result.Errors.Select(e => e.Description).ToList(), "Registration failed");
        }

        await userManager.AddToRoleAsync(user, role);

        var token = GenerateJwtToken(user, new List<string> { role });

        return Result<AuthResponseDto>.Success(new AuthResponseDto(token, user.FullName, user.PhoneNumber!, role, user.Points));
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(string phoneNumber, string password)
    {
        var user = await userManager.FindByNameAsync(phoneNumber);

        if (user == null || !await userManager.CheckPasswordAsync(user, password))
        {
            return Result<AuthResponseDto>.Failure("Invalid phone number or password");
        }

        var roles = await userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        return Result<AuthResponseDto>.Success(new AuthResponseDto(token, user.FullName, user.PhoneNumber!, roles.FirstOrDefault() ?? "GeneralUser", user.Points));
    }

    private string GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new("FullName", user.FullName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not found")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
