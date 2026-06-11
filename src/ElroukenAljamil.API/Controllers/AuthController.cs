using ElroukenAljamil.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ElroukenAljamil.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    // In-memory code store (in production, use Redis/DB)
    private static readonly Dictionary<string, (string Code, DateTime Expiry)> _codes = new();

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _logger = logger;
    }

    [HttpPost("check-email")]
    public async Task<ActionResult> CheckEmail([FromBody] EmailRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return BadRequest(new { message = "Email requis." });

        var user = await _userManager.FindByEmailAsync(req.Email.Trim());
        return Ok(new { exists = user != null });
    }

    [HttpPost("send-code")]
    public ActionResult SendCode([FromBody] EmailRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return BadRequest(new { message = "Email requis." });

        var email = req.Email.ToLower().Trim();
        var code = GenerateCode(5);
        _codes[email] = (code, DateTime.UtcNow.AddMinutes(10));

        _logger.LogInformation("=== EMAIL VERIFICATION CODE ===");
        _logger.LogInformation("To: {Email} | Code: {Code}", email, code);
        _logger.LogInformation("================================");

        return Ok(new { message = "Code envoyé.", maskedEmail = MaskEmail(email) });
    }

    [HttpPost("verify-code")]
    public ActionResult VerifyCode([FromBody] VerifyCodeRequest req)
    {
        var email = req.Email?.ToLower().Trim() ?? "";
        if (!_codes.TryGetValue(email, out var entry) || entry.Code != req.Code || entry.Expiry < DateTime.UtcNow)
            return BadRequest(new { message = "Code invalide ou expiré." });

        _codes.Remove(email);
        return Ok(new { message = "Email vérifié." });
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { message = "Email et mot de passe requis." });

        var user = new ApplicationUser
        {
            UserName = req.Email.ToLower().Trim(),
            Email = req.Email.ToLower().Trim(),
            AccountType = req.AccountType ?? "personal",
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

        return Ok(new { message = "Compte créé.", userId = user.Id });
    }

    [HttpPost("send-sms-code")]
    public async Task<ActionResult> SendSmsCode([FromBody] PhoneRequest req)
    {
        var email = req.Email?.ToLower().Trim() ?? "";
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return BadRequest(new { message = "Utilisateur non trouvé." });

        var code = GenerateCode(6);
        _codes[$"phone_{email}"] = (code, DateTime.UtcNow.AddMinutes(10));

        // Save phone number
        user.PhoneNumber = req.Phone;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("=== SMS VERIFICATION CODE ===");
        _logger.LogInformation("To: {Phone} | Code: {Code}", req.Phone, code);
        _logger.LogInformation("==============================");

        return Ok(new { message = "SMS envoyé." });
    }

    [HttpPost("verify-phone")]
    public async Task<ActionResult> VerifyPhone([FromBody] VerifyCodeRequest req)
    {
        var email = req.Email?.ToLower().Trim() ?? "";
        var key = $"phone_{email}";

        if (!_codes.TryGetValue(key, out var entry) || entry.Code != req.Code || entry.Expiry < DateTime.UtcNow)
            return BadRequest(new { message = "Code invalide ou expiré." });

        _codes.Remove(key);

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return BadRequest(new { message = "Utilisateur non trouvé." });

        user.PhoneNumberConfirmed = true;
        user.IsActive = true;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("=== WELCOME EMAIL === To: {Email}", email);

        var token = GenerateJwtToken(user);
        return Ok(new { token, userId = user.Id, email = user.Email, phone = user.PhoneNumber, firstName = user.FirstName, lastName = user.LastName });
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest req)
    {
        var email = req.Email?.ToLower().Trim() ?? "";
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Unauthorized(new { message = "Email ou mot de passe incorrect." });

        var result = await _signInManager.CheckPasswordSignInAsync(user, req.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Email ou mot de passe incorrect." });

        var token = GenerateJwtToken(user);
        return Ok(new { token, userId = user.Id, email = user.Email, phone = user.PhoneNumber, firstName = user.FirstName, lastName = user.LastName });
    }

    [HttpGet("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<ActionResult> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId ?? "");
        if (user == null) return Unauthorized();
        return Ok(new { userId = user.Id, email = user.Email, phone = user.PhoneNumber, firstName = user.FirstName, lastName = user.LastName });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "ElroukenAljamilSuperSecretKey2024!@#$%^&*"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
            new Claim("accountType", user.AccountType)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "ElroukenAljamil",
            audience: _config["Jwt:Audience"] ?? "ElroukenAljamilApp",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateCode(int length)
    {
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        return string.Join("", bytes.Select(b => (b % 10).ToString())).Substring(0, length);
    }

    private string MaskEmail(string email)
    {
        var parts = email.Split('@');
        if (parts[0].Length <= 2) return email;
        return parts[0][0] + new string('*', parts[0].Length - 2) + parts[0][^1] + "@" + parts[1];
    }
}

public class EmailRequest { public string Email { get; set; } = ""; }
public class VerifyCodeRequest { public string Email { get; set; } = ""; public string Code { get; set; } = ""; }
public class RegisterRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; public string? AccountType { get; set; } }
public class PhoneRequest { public string Email { get; set; } = ""; public string Phone { get; set; } = ""; }
public class LoginRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }
