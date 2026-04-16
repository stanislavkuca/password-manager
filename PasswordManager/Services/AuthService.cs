using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PasswordManager.Services
{
    /// <summary>
    /// Simple hashing helper for master password handling.
    /// Note: This uses plain SHA256 for simplicity; consider a salted KDF (PBKDF2/Argon2)
    /// </summary>
    public static class AuthService
    {
        // Hash input using SHA256 and return Base64 string.
        public static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }

        public static bool Verify(string password, string storedHash)
        {
            return Hash(password) == storedHash;
        }
    }
}
