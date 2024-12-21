using Microsoft.Extensions.Options;
using Mika.Framework.Models;
using Mika.Framework.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Framework.Services.Implementations
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bit 
        private const int KeySize = 32; // 256 bit
        private HashingOptions Options { get; }
        public PasswordHasher(IOptions<HashingOptions> options)
        {
            Options = options.Value;
        }
        public (bool Verified, bool NeedsUpgrade) Check(string hash, string password)
        {

            if (string.IsNullOrEmpty(hash))
            {
                return (false, false);
            }
            var parts = hash.Split(".", 3);
            if (parts.Length != 3)
            {
                return (false, false);
            }
            try
            {
                var iterations = Convert.ToInt32(parts[0]);
                var salt = Convert.FromBase64String(parts[1]);
                var key = Convert.FromBase64String(parts[2]);

                var needsUpgrade = iterations != Options.Iterations;

                using var algorithm = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
                var keyToCheck = algorithm.GetBytes(KeySize);

                var verified = keyToCheck.SequenceEqual(key);

                return (verified, needsUpgrade);
            }
            catch (Exception)
            {
                return (false, false);
            }

        }

        public string Hash(string password)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }
            using var algorithm =
                new Rfc2898DeriveBytes(password, SaltSize, Options.Iterations, HashAlgorithmName.SHA256);
            var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
            var salt = Convert.ToBase64String(algorithm.Salt);

            return $"{Options.Iterations}.{salt}.{key}";
        }
    }
}
