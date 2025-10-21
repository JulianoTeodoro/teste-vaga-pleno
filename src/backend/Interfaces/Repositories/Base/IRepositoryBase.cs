using System.Linq.Expressions;

namespace Parking.Api.Interfaces.Repositories.Base
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        Task CreateAsync(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TEntity entity);
        Task SaveChangesAsync();
        void SaveChanges();
    }
}
