namespace ElroukenAljamil.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public bool IsActive { get; set; } = true;
    public string AccountType { get; set; } = "personal"; // personal, business
    public string? VerificationCode { get; set; }
    public DateTime? CodeExpiry { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
