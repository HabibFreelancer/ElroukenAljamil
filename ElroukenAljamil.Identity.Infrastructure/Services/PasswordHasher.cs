using System.Security.Cryptography;
using ElroukenAljamil.Identity.Application.Interfaces;

namespace ElroukenAljamil.Identity.Infrastructure.Services
{
    /// <summary>
    /// Hachage de mot de passe avec PBKDF2 (SHA-512, 100 000 itérations).
    /// Format stocké : {iterations}.{salt_base64}.{hash_base64}
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100_000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool Verify(string password, string hashString)
        {
            var parts = hashString.Split('.');
            if (parts.Length != 3)
                return false;

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var hash = Convert.FromBase64String(parts[2]);

            var computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, HashSize);

            return CryptographicOperations.FixedTimeEquals(computedHash, hash);
        }
    }
}
