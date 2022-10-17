using System.Data.SqlTypes;
using System.Security.Cryptography;

namespace BlazorCookieAuthentication.Services
{
    public static class PasswordProcessor
    {
        private static readonly int IterationCount = 10_000;

        private static readonly int SaltSize = 128;

        private static readonly int HashSize = 512;

        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

        // Grabs Correct Algo from the string of name
        private static HashAlgorithmName GetHashAlgorithmName(string name)
        {
            return name switch
            {
                "SHA512" => HashAlgorithmName.SHA512,
                "SHA384" => HashAlgorithmName.SHA384,
                "SHA256" => HashAlgorithmName.SHA256,
                "SHA1" => HashAlgorithmName.SHA1,
                _ => HashAlgorithmName.SHA1,
            };
        }

        // Generate Hash + Salt for a given password
        public static string HashPassword(string password)
        {
            // Create the Salt 
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize / 8);

            // Hash the password and get the bytes
            var algo = new Rfc2898DeriveBytes(password, salt, IterationCount, HashAlgorithm);
            byte[] hashed = algo.GetBytes(HashSize / 8);

            // Convert into strings
            string saltString = Convert.ToBase64String(salt);
            string hashString = Convert.ToBase64String(hashed);
            string algoString = HashAlgorithm.ToString();

            // Format the final string
            return String.Format("{0}.{1}.{2}.{3}", IterationCount, algoString, saltString, hashString);
        }

        public static bool VerifyPassword(string userPassword, string hashedPassword)
        {
            // split the hased password into sections
            string[] hashedSections = hashedPassword.Split('.');

            // convert the sections into the correct data types
            int iterations = int.Parse(hashedSections[0]);
            var algorithmName = GetHashAlgorithmName(hashedSections[1]);
            byte[] salt = Convert.FromBase64String(hashedSections[2]);
            byte[] hashedStoredPassword = Convert.FromBase64String(hashedSections[3]);

            // Try to enocode the password with the same key + interations
            var algo = new Rfc2898DeriveBytes(userPassword, salt, iterations, algorithmName);
            byte[] hashedInputPassword = algo.GetBytes(hashedStoredPassword.Length);

            bool isSame = true;
            for (int i = 0; i < hashedStoredPassword.Length; i++)
            {
                if (hashedInputPassword[i] != hashedStoredPassword[i]) { isSame = false; }
            }
            return isSame;
        }

    }
}
