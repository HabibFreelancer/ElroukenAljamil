namespace ElroukenAljamil.Identity.Domain.Entities
{
    public class VerificationCode
    {
        public int Id { get; private set; }
        public string Target { get; private set; } = string.Empty; // email ou "phone_email"
        public string Code { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }

        private VerificationCode() { }

        public static VerificationCode Create(string target, string code, int expirationMinutes = 10) =>
            new()
            {
                Target = target.ToLowerInvariant().Trim(),
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                IsUsed = false
            };

        public bool IsValid(string code) =>
            !IsUsed && Code == code && ExpiresAt > DateTime.UtcNow;

        public void MarkAsUsed() => IsUsed = true;
    }
}
