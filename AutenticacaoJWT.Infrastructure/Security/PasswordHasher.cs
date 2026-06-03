using AutenticacaoJWT.Domain.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace AutenticacaoJWT.Infra.Data.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 128 / 8;
        private const int HashSize = 256 / 8;
        private const int IterationCount = 100_000;

        public byte[] GenerateSalt()
        {
            var salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

        public string HashPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: HashSize));
        }

        public bool VerifyPassword(string password, byte[] salt, string hashedPassword)
        {
            try
            {
                var hash = HashPassword(password, salt);
                var hashBytes = Convert.FromBase64String(hash);
                var storedBytes = Convert.FromBase64String(hashedPassword);
                if (hashBytes.Length != storedBytes.Length)
                    return false;

                return CryptographicOperations.FixedTimeEquals(hashBytes, storedBytes);
            }
            catch
            {
                return false;
            }
        }
    }
}
