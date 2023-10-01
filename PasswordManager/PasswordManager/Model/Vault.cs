using System;
using System.Text;
using CsvHelper.Configuration.Attributes;
using System.Security.Cryptography;
using Cryptography;
using CsvHelper;
using System.Globalization;
using Utility;

namespace PasswordManager.Model
{
    internal class Vault
    {
        [Name("user_id")]

        public string? UserId { get; set; }
        [Name("password")]
        public string? Password { get; set; }

        [Name("username")]
        public string? Username { get; set; }

        [Name("website")]
        public string? Website { get; set; }

        public static string? UserCsvPath { get; set; }
       
        [Ignore]
        public User? user
        {
            get
            {
                if (UserCsvPath == null) return null;
                using StreamReader reader = new StreamReader(UserCsvPath + "/user.csv");
                using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
                return csv.GetRecords<User>().FirstOrDefault(us => us.Username == UserId);
            }
        }
        public void EncryptPassword()
        {
            using var hashing = SHA256.Create();
            byte[] keyHash = hashing.ComputeHash(Encoding.Unicode.GetBytes(user!.Email!));
            string key = Base64UrlEncoder.Encode(keyHash);
            Password = Fernet.Encrypt(key, Password!);

        }
        public string DecryptPassword()
        {
            using var hashing = SHA256.Create();
            byte[] keyHash = hashing.ComputeHash(Encoding.Unicode.GetBytes(user!.Email!));
            string key = Base64UrlEncoder.Encode(keyHash);
            return Fernet.Decrypt(key, Password!);
        }

    }
}
