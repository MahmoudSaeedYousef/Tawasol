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

        var phoneNumber = principal.Identity?.Name;
        var user = await userManager.FindByNameAsync(phoneNumber!);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Result<AuthResponseDto>.Failure("Invalid refresh token");
        }

        var roles = await userManager.GetRolesAsync(user);
        return await GenerateAuthResponse(user, roles);
    }

    private async Task<Result<AuthResponseDto>> GenerateAuthResponse(ApplicationUser user, IEnumerable<string> roles)
    {
        var token = GenerateJwtToken(user, roles);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
    
        // تأكد إن الـ userManager بيسيف التعديلات دي
        await userManager.UpdateAsync(user);

        return Result<AuthResponseDto>.Success(new AuthResponseDto(
            user.Id,                // 👈 مرر الـ ID
            token, 
            refreshToken, 
            user.FullName, 
            user.PhoneNumber!, 
            roles.FirstOrDefault() ?? "GeneralUser", 
            user.Points,
            user.RankTitle ?? "جار الخير" // 👈 مرر اللقب (لو نل حط القيمة الافتراضية)
        ));
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
