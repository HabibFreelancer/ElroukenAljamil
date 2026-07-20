using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Identity.Application.DTOs
{
    public record RegisterDto
    {
        public string Email { get; init; } = string.Empty;
        public string UserName { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string ConfirmPassword { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
        public string AccountType { get; init; } = "personal"; // personal | pro
    }

    public record LoginDto
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }

    public record AuthResponseDto
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        public UserProfileDto User { get; init; } = null!;
    }

    public record UserProfileDto
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string UserName { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
        public string Role { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string? AvatarUrl { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public record RefreshTokenDto
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
    }

    public record CheckEmailRequest
    {
        public string Email { get; init; } = string.Empty;
    }

    public record SendCodeRequest
    {
        public string Email { get; init; } = string.Empty;
    }

    public record VerifyCodeRequest
    {
        public string Email { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
    }

    public record SendSmsCodeRequest
    {
        public string Email { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
    }
}
