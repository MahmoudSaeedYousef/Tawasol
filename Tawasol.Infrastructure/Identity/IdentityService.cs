using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Auth;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;

namespace Tawasol.Infrastructure.Identity;

public class IdentityService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IConfiguration configuration)
    : IIdentityService
{
    public async Task<Result<AuthResponseDto>> RegisterAsync(string fullName, string phoneNumber, string password, string role)
    {
        // Correctly create a domain User instance
        var user = new User(fullName, phoneNumber, Enum.Parse<UserRole>(role, true));

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return Result<AuthResponseDto>.Failure(result.Errors.Select(e => e.Description).ToList(), "Registration failed");
        }

        await userManager.AddToRoleAsync(user, role);

        return await GenerateAuthResponse(user, new List<string> { role });
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(string phoneNumber, string password)
    {
        var user = await userManager.FindByNameAsync(phoneNumber);

        if (user == null || !await userManager.CheckPasswordAsync(user, password))
        {
            return Result<AuthResponseDto>.Failure("Invalid phone number or password");
        }

        var roles = await userManager.GetRolesAsync(user);
        return await GenerateAuthResponse(user, roles);
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string expiredToken, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(expiredToken);
        if (principal == null) return Result<AuthResponseDto>.Failure("Invalid token");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userManager.FindByIdAsync(userId!);

        // Refresh token logic should be implemented in the User entity if needed, or managed here.
        // For now, assuming it's not part of the core domain User properties.
        // if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        // {
        //     return Result<AuthResponseDto>.Failure("Invalid refresh token");
        // }

        var roles = await userManager.GetRolesAsync(user);
        return await GenerateAuthResponse(user, roles);
    }

    private async Task<Result<AuthResponseDto>> GenerateAuthResponse(User user, IEnumerable<string> roles)
    {
        var token = GenerateJwtToken(user, roles);
        var refreshToken = GenerateRefreshToken();

        // Refresh token logic is not part of the domain User entity.
        // await userManager.UpdateAsync(user);

        return Result<AuthResponseDto>.Success(new AuthResponseDto(
            user.Id.ToString(),
            token,
            refreshToken,
            user.FullName,
            user.PhoneNumber!,
            roles.FirstOrDefault() ?? "GeneralUser",
            user.Points,
            user.GetTitle() // Use the domain method to get the title
        ));
    }

    private string GenerateJwtToken(User user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new("FullName", user.FullName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15), // Short-lived access token
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            return null;

        return principal;
    }
}
