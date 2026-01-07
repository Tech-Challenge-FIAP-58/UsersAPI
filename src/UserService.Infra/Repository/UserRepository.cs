using UserService.Core.Interfaces.Repository;
using UserService.Core.Models;
using UserService.Infra.Context;

namespace UserService.Infra.Repository
{
    public class UserRepository(ApplicationDbContext context) : EFRepository<User>(context), IUserRepository
    {

        public async Task<IEnumerable<User>> GetAll() => await Get();

        public async Task<User?> GetById(int id) => await Get(id);

        public async Task<bool> Update(int id, User userUpdate) => await Edit(userUpdate);

        public async Task<bool> Remove(User entity) => await Delete(entity);
    }
}