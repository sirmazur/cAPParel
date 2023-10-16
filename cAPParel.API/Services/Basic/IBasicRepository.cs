using System.Linq.Expressions;

namespace cAPParel.API.Services.Basic
{
    public interface IBasicRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<(bool,TEntity?)> CheckIfIdExistsAsync(int id);
        void DeleteAsync(TEntity entity);
        Task SaveChangesAsync();
        Task<TEntity> GetByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties);
    }
}
