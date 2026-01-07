using Microsoft.Extensions.Configuration;
using UserService.Core.Models;


namespace UserService.Core.Interfaces.Repository
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