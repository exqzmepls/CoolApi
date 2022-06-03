using System;
using System.Security.Cryptography;
using System.Text;

namespace CoolApi.Database.Hashers
{
    public class SHA256PasswordHasher : IPasswordHasher
    {
        public string GetPasswordHash(string password)
        {
            using var hasher = SHA256.Create();
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = hasher.ComputeHash(passwordBytes);
            var hashString = Convert.ToHexString(hashBytes);

            return hashString;
        }

        public bool VerifyPassword(string passwordHash, string password)
        {
            var providedPasswordHash = GetPasswordHash(password);
            var result = passwordHash == providedPasswordHash;

            return result;
        }
    }
}
