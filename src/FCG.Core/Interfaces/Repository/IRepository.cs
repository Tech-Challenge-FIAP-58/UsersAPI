using FCG.Core.Models;

namespace FCG.Core.Interfaces.Repository
{
    public interface IRepository<T> where T : EntityBase
    {
        Task<IEnumerable<T>> Get();
        Task<T?> Get(int id);
        Task<bool> Register(T entity);
        Task<bool> Edit(T entity);
        Task<bool> Delete(T entity);
    }
}