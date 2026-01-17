using FCG.Core.Models;
using Microsoft.Extensions.Configuration;


namespace FCG.Core.Interfaces.Repository
{
    public interface IAuthRepository
    {
        Task<User?> FindByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByCpfAsync(string cpf);
        Task<int> Create(User entity);
        string GenerateToken(IConfiguration configuration, User user);
    }

}