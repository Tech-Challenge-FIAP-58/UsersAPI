using System.Security.Cryptography;
using System.Text;
using UserService.Core.Interfaces.Utils;

namespace UserService.Core.Utils
{
    public class Sha256PasswordHasher : IPasswordHasher
    {
        public string Hash(string raw)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
            return Convert.ToHexString(bytes);
        }

        public bool Verify(string raw, string hashed) => Hash(raw).Equals(hashed);
    }
}
