using System;
using System.Text;
using CsvHelper.Configuration.Attributes;
using System.Security.Cryptography;
using Utility;
using Cryptography;
using System.Net.Sockets;

namespace PasswordManager.Model
{
    internal class User
    {
        [Name("username")]
        public string? Username { get; set; }
        [Name("password")]
        public string? Password { get; set; }
        [Name("email")]
        public string? Email { get; set; }

        [Name("firstname")]
        public string? Firstname { get; set; }

        [Name("lastname")]
        public string? Lastname { get; set; }


        public void EncryptPassword()
        {
            using var hashing = SHA256.Create();
            byte[] keyHash = hashing.ComputeHash(Encoding.Unicode.GetBytes(Email!));
            string key = Base64UrlEncoder.Encode(keyHash);
            Password = Fernet.Encrypt(key, Password!);

        }
        public string DecryptPassword()
        {
            using var hashing = SHA256.Create();
            byte[] keyHash = hashing.ComputeHash(Encoding.Unicode.GetBytes(Email!));
            string key = Base64UrlEncoder.Encode(keyHash);
            return Fernet.Decrypt(key, Password!);
        }
    }
}
