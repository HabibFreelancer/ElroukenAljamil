using ElroukenAljamil.Domain.Entities;
using ElroukenAljamil.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ElroukenAljamil.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AppDbContext context, ILogger<AuthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("check-email")]
    public async Task<ActionResult> CheckEmail([FromBody] EmailRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return BadRequest(new { message = "Email requis." });

        var exists = await _context.Users.AnyAsync(u => u.Email == req.Email.ToLower().Trim());
        return Ok(new { exists });
    }

    [HttpPost("send-code")]
    public async Task<ActionResult> SendCode([FromBody] EmailRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return BadRequest(new { message = "Email requis." });

        var email = req.Email.ToLower().Trim();
        var code = GenerateCode(5);

        // Store code temporarily (in a real app, use a cache/Redis)
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            user = new User { Email = email, VerificationCode = code, CodeExpiry = DateTime.UtcNow.AddMinutes(10) };
            _context.Users.Add(user);
        }
        else
        {
            user.VerificationCode = code;
            user.CodeExpiry = DateTime.UtcNow.AddMinutes(10);
        }
        await _context.SaveChangesAsync();

        // Mock email sending (log to console)
        _logger.LogInformation("=== EMAIL VERIFICATION CODE ===");
        _logger.LogInformation("To: {Email}", email);
        _logger.LogInformation("Code: {Code}", code);
        _logger.LogInformation("================================");

        return Ok(new { message = "Code envoyé.", maskedEmail = MaskEmail(email) });
    }

    [HttpPost("verify-code")]
    public async Task<ActionResult> VerifyCode([FromBody] VerifyCodeRequest req)
    {
        var email = req.Email?.ToLower().Trim();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return BadRequest(new { message = "Utilisateur non trouvé." });

        if (user.VerificationCode != req.Code || user.CodeExpiry < DateTime.UtcNow)
            return BadRequest(new { message = "Code invalide ou expiré." });

        user.EmailVerified = true;
        user.VerificationCode = null;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Email vérifié." });
    }

    [HttpPost("set-password")]
    public async Task<ActionResult> SetPassword([FromBody] SetPasswordRequest req)
    {
        var email = req.Email?.ToLower().Trim();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || !user.EmailVerified)
            return BadRequest(new { message = "Email non vérifié." });

        user.PasswordHash = HashPassword(req.Password);
        user.AccountType = req.AccountType ?? "personal";
        await _context.SaveChangesAsync();

        return Ok(new { message = "Mot de passe défini." });
    }

    [HttpPost("send-sms-code")]
    public async Task<ActionResult> SendSmsCode([FromBody] PhoneRequest req)
    {
        var email = req.Email?.ToLower().Trim();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return BadRequest(new { message = "Utilisateur non trouvé." });

        var code = GenerateCode(6);
        user.Phone = req.Phone;
        user.VerificationCode = code;
        user.CodeExpiry = DateTime.UtcNow.AddMinutes(10);
        await _context.SaveChangesAsync();

        // Mock SMS sending
        _logger.LogInformation("=== SMS VERIFICATION CODE ===");
        _logger.LogInformation("To: {Phone}", req.Phone);
        _logger.LogInformation("Code: {Code}", code);
        _logger.LogInformation("==============================");

        return Ok(new { message = "SMS envoyé." });
    }

    [HttpPost("verify-phone")]
    public async Task<ActionResult> VerifyPhone([FromBody] VerifyCodeRequest req)
    {
        var email = req.Email?.ToLower().Trim();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return BadRequest(new { message = "Utilisateur non trouvé." });

        if (user.VerificationCode != req.Code || user.CodeExpiry < DateTime.UtcNow)
            return BadRequest(new { message = "Code invalide ou expiré." });

        user.PhoneVerified = true;
        user.IsActive = true;
        user.VerificationCode = null;
        await _context.SaveChangesAsync();

        // Mock: send welcome email
        _logger.LogInformation("=== WELCOME EMAIL ===");
        _logger.LogInformation("To: {Email}", user.Email);
        _logger.LogInformation("Subject: Bienvenue sur ElroukenAljamil !");
        _logger.LogInformation("=====================");

        return Ok(new { message = "Compte créé et activé.", userId = user.Id, email = user.Email, phone = user.Phone });
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest req)
    {
        var email = req.Email?.ToLower().Trim();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            return Unauthorized(new { message = "Email ou mot de passe incorrect." });

        if (HashPassword(req.Password) != user.PasswordHash)
            return Unauthorized(new { message = "Email ou mot de passe incorrect." });

        return Ok(new { userId = user.Id, email = user.Email, firstName = user.FirstName, lastName = user.LastName, phone = user.Phone });
    }

    private string GenerateCode(int length)
    {
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        return string.Join("", bytes.Select(b => (b % 10).ToString())).Substring(0, length);
    }

    private string MaskEmail(string email)
    {
        var parts = email.Split('@');
        if (parts[0].Length <= 2) return email;
        return parts[0][0] + new string('*', parts[0].Length - 2) + parts[0][^1] + "@" + parts[1];
    }

    private string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password + "_ElroukenSalt"));
        return Convert.ToBase64String(bytes);
    }
}

public class EmailRequest { public string Email { get; set; } = ""; }
public class VerifyCodeRequest { public string Email { get; set; } = ""; public string Code { get; set; } = ""; }
public class SetPasswordRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; public string? AccountType { get; set; } }
public class PhoneRequest { public string Email { get; set; } = ""; public string Phone { get; set; } = ""; }
public class LoginRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }
