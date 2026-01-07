using UserService.Core.Models;

namespace UserService.Core.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
        Task<User?> GetById(int id);
        Task<bool> Update(int id, User userUpdate);
        Task<bool> Remove(User entity);
    }
}