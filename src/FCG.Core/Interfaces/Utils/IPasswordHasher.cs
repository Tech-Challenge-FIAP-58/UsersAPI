using System.Reflection.Metadata;

namespace FCG.Core.Interfaces.Utils
{
    public interface IPasswordHasher
    {
        string Hash(string raw);
        bool Verify(string raw, string hashed);
    }
}
