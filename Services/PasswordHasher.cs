using System;
using System.Security.Cryptography;

namespace HelpDesk.Web.Services
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100000;

        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

        public static byte[] Hash(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var salt = new byte[SaltSize];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithm))
            {
                var key = deriveBytes.GetBytes(KeySize);
                var result = new byte[SaltSize + KeySize];

                Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
                Buffer.BlockCopy(key, 0, result, SaltSize, KeySize);

                return result;
            }
        }

        public static bool Verify(string password, byte[] storedHash)
        {
            if (password == null || storedHash == null || storedHash.Length != SaltSize + KeySize)
            {
                return false;
            }

            var salt = new byte[SaltSize];
            var storedKey = new byte[KeySize];

            Buffer.BlockCopy(storedHash, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(storedHash, SaltSize, storedKey, 0, KeySize);

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithm))
            {
                var key = deriveBytes.GetBytes(KeySize);
                return FixedTimeEquals(key, storedKey);
            }
        }

        private static bool FixedTimeEquals(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length)
            {
                return false;
            }

            var difference = 0;

            for (var i = 0; i < left.Length; i++)
            {
                difference |= left[i] ^ right[i];
            }

            return difference == 0;
        }
    }
}
