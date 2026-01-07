using Microsoft.EntityFrameworkCore;
using UserService.Core.Interfaces.Repository;
using UserService.Core.Models;
using UserService.Infra.Context;

namespace UserService.Infra.Repository
{
    public class EFRepository<T> : IRepository<T> where T : EntityBase
    {
        protected ApplicationDbContext _context;
        protected DbSet<T> _dbSet;

        public EFRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<bool> Edit(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Register(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<T?> Get(int id) => await _dbSet.FirstOrDefaultAsync(entity => entity.Id == id);

        public async Task<IEnumerable<T>> Get() => await _dbSet.AsNoTracking().ToListAsync();
    }
}