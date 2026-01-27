using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskList.Application.Common;
using TaskList.Application.DTOs.Auth;
using TaskList.Application.Interfaces;
using TaskList.Infrastucture.Indentity;

namespace TaskList.Infrastucture.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager, 
    IJwtTokenService jwtTokenService,
    IOptions<JwtSettings> jwtSettings) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        // Create new user
        var user = new ApplicationUser
        {
            Name = request.Name,
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Name, user.Email!);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Store refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
        await _userManager.UpdateAsync(user);

        return new AuthResponse
        {
            Name = user.Name,
            Email = user.Email!,
            Token = accessToken
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Find user by email
        var user = await _userManager.FindByEmailAsync(request.Email) ?? throw new InvalidOperationException("Invalid email or password.");

        // Check password
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Name, user.Email!);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Store refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
        await _userManager.UpdateAsync(user);

        return new AuthResponse
        {
            Name = user.Name,
            Email = user.Email!,
            Token = accessToken
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        // Find user by refresh token
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken) ?? throw new InvalidOperationException("Invalid refresh token.");

        // Check if refresh token is expired
        if (user.RefreshTokenExpiry == null || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Refresh token has expired.");
        }

        // Generate new tokens
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Name, user.Email!);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        // Update refresh token
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
        await _userManager.UpdateAsync(user);

        return new AuthResponse
        {
            Name = user.Name,
            Email = user.Email!,
            Token = newAccessToken
        };
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        // Find user by refresh token
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user is null)
        {
            return false;
        }

        // Clear refresh token
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return true;
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        throw new InvalidOperationException($"Failed to revoke token: {errors}");
    }
}


