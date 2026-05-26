using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Auth;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserRepository userRepository,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<Result<AuthResponseDto>> RegisterAsync(string fullName, string phoneNumber, string password, string role)
        {
            var appUser = new ApplicationUser { UserName = phoneNumber, PhoneNumber = phoneNumber };
            var identityResult = await _userManager.CreateAsync(appUser, password);

            if (!identityResult.Succeeded)
            {
                return Result<AuthResponseDto>.Failure(identityResult.Errors.Select(e => e.Description).ToList());
            }

            var domainUser = new User(fullName, phoneNumber, Enum.Parse<UserRole>(role, true));
            // Manually set the domain user's ID to match the identity user's ID
            typeof(User).GetProperty("Id").SetValue(domainUser, appUser.Id);
            
            await _userRepository.AddAsync(domainUser);

            await _userManager.AddToRoleAsync(appUser, role);

            return await GenerateAuthResponse(appUser, domainUser, new List<string> { role });
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(string phoneNumber, string password)
        {
            var appUser = await _userManager.FindByNameAsync(phoneNumber);
            if (appUser == null) return Result<AuthResponseDto>.Failure("Invalid phone number or password");

            var result = await _signInManager.CheckPasswordSignInAsync(appUser, password, false);
            if (!result.Succeeded) return Result<AuthResponseDto>.Failure("Invalid phone number or password");

            var domainUser = await _userRepository.GetByIdAsync(appUser.Id);
            if (domainUser == null) return Result<AuthResponseDto>.Failure("User data not found.");

            var roles = await _userManager.GetRolesAsync(appUser);
            return await GenerateAuthResponse(appUser, domainUser, roles);
        }

        public Task<Result<AuthResponseDto>> RefreshTokenAsync(string expiredToken, string refreshToken)
        {
            // This needs to be re-implemented based on the new separation.
            // For now, returning a failure.
            return Task.FromResult(Result<AuthResponseDto>.Failure("RefreshToken not implemented."));
        }

        private async Task<Result<AuthResponseDto>> GenerateAuthResponse(ApplicationUser appUser, User domainUser, IEnumerable<string> roles)
        {
            var token = GenerateJwtToken(appUser, roles);
            var refreshToken = GenerateRefreshToken();

            return Result<AuthResponseDto>.Success(new AuthResponseDto(
                domainUser.Id.ToString(),
                token,
                refreshToken,
                domainUser.FullName,
                domainUser.PhoneNumber,
                roles.FirstOrDefault() ?? "GeneralUser",
                domainUser.Points,
                domainUser.GetTitle()
            ));
        }

        private string GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
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
    }
}
