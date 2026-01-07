using System.Reflection.Metadata;

namespace UserService.Core.Interfaces.Utils
{
    public interface IPasswordHasher
    {
        string Hash(string raw);
        bool Verify(string raw, string hashed);
    }
}
