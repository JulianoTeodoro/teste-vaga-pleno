using Microsoft.EntityFrameworkCore;
using Parking.Api.Interfaces.Repositories.Base;
using System.Linq.Expressions;

namespace Parking.Api.Data.Repositories.Base
{
    public class EFRepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public EFRepositoryBase(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            if (predicate != null)
                return await _dbSet.Where(predicate).ToListAsync();

            return await _dbSet.ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public async Task Delete(TEntity entity)
        {
            _dbSet.Remove(entity);

        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

    }

}
