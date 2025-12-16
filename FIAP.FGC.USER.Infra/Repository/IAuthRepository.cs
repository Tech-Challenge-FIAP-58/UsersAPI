using FIAP.FGC.USER.Core.Inputs;
using FIAP.FGC.USER.Core.Models;
using Microsoft.Extensions.Configuration;


namespace FIAP.FGC.USER.Infra.Repository
{
    public interface IAuthRepository
    {
        Task<int> Create(UserRegisterDto dto);
        Task<User?> FindByCredentialsAsync(LoginDto dto);
        string GenerateToken(IConfiguration configuration, User user);
    }
}
