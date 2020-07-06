using JwtAuthAspNetCoreApi.Models;
using System;
using System.Security.Cryptography;

namespace JwtAuthAspNetCoreApi.Helpers
{
    public static class PasswordProtector
    {
        // These constants may be changed without breaking existing hashes.
        public const int SALTBYTES = 24;
        public const int HASHBYTES = 18;
        public const int PBKDF2ITERATIONS = 64000;

        public static PasswordHashModel GetHashAndSalt(string password)
        {
            PasswordHashModel passwordHashModel = new PasswordHashModel();
            if (!string.IsNullOrEmpty(password))
            {
                byte[] uniqueSalt = GetSalt();
                byte[] hashedPassword = GetHashPassword(password, uniqueSalt);

                passwordHashModel.SaltText = Convert.ToBase64String(uniqueSalt);
                passwordHashModel.HashText = Convert.ToBase64String(hashedPassword);
                passwordHashModel.HashIteration = PBKDF2ITERATIONS;
                passwordHashModel.HashLength = hashedPassword.Length;
            }

            return passwordHashModel;
        }

        private static byte[] GetHashPassword(string password, byte[] salt)
        {
            byte[] hash = PBKDF2(password, salt, PBKDF2ITERATIONS, HASHBYTES);

            return hash;
        }

        private static byte[] GetSalt()
        {
            byte[] salt = new byte[SALTBYTES];
            try
            {
                using RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
                csprng.GetBytes(salt);
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicException(
                    "Random number generator not available.",
                    ex
                );
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException(
                    "Invalid argument given to random number generator.",
                    ex
                );
            }

            return salt;
        }

        public static bool VerifyPassword(
            string password,
            string hashedPassword,
            string passwordSalt,
            int hashLength,
            int hashIteration)
        {
            if (
                !string.IsNullOrEmpty(password)
                && !string.IsNullOrEmpty(hashedPassword)
                && !string.IsNullOrEmpty(passwordSalt)
                && hashLength > 0
                && hashIteration > 1)
            {
                byte[] salt = Convert.FromBase64String(passwordSalt);
                byte[] storedHash = Convert.FromBase64String(hashedPassword);
                if (storedHash.Length == hashLength)
                {
                    byte[] newCreatedHash = PBKDF2(password, salt, hashIteration, storedHash.Length);

                    return SlowEquals(storedHash, newCreatedHash);
                }
            }

            return false;
        }

        private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt))
            {
                pbkdf2.IterationCount = iterations;
                return pbkdf2.GetBytes(outputBytes);
            }
        }

        private static bool SlowEquals(byte[] storedHash, byte[] newlyGeneratedHash)
        {
            uint diff = (uint)storedHash.Length ^ (uint)newlyGeneratedHash.Length;
            for (int i = 0; i < storedHash.Length && i < newlyGeneratedHash.Length; i++)
            {
                diff |= (uint)(storedHash[i] ^ newlyGeneratedHash[i]);
            }

            return diff == 0;
        }
    }
}
