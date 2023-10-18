using cAPParel.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace cAPParel.API.Services.Basic
{
    public interface IBasicService<TDto, TEntity, TExtendedDto, TCreationDto> where TDto : class where TEntity : class where TExtendedDto : class where TCreationDto : class
    {
        Task<TDto> GetByIdAsync(int id);
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto> CreateAsync(TCreationDto creationDto);
        Task<OperationResult<TDto>> UpdateAsync(int id, TCreationDto creationDto);
        Task<OperationResult<TDto>> PartialUpdateAsync(int id, JsonPatchDocument<TCreationDto> patchDocument);
        Task<(bool, TEntity?)> CheckIfIdExistsAsync(int id);
        Task<OperationResult<TDto>> DeleteByIdAsync(int id);
        Task<TEntity> GetEntityByIdAsync(int id);
        Task<TEntity> GetEntityByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TDto> GetByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TExtendedDto> GetExtendedByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<IEnumerable<TExtendedDto>> GetExtendedListWithEagerLoadingAsync(IEnumerable<int> ids, params Expression<Func<TEntity, object>>[] includeProperties);
    }
}
