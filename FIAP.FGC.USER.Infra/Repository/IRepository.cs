using FIAP.FGC.USER.Core.Models;

namespace FIAP.FGC.USER.Infra.Repository
{
	public interface IRepository<T> where T : EntityBase
	{
		Task<IEnumerable<T>> Get();
		Task<T?> Get(int id);
		Task<bool> Register(T entity);
		Task<bool> Edit(T entity);
		Task<bool> Delete(int id);
	}
}
