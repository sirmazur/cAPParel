using AutoMapper;
using cAPParel.API.Filters;
using cAPParel.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection.Metadata;

namespace cAPParel.API.Services.Basic
{
    public class BasicService<TDto, TEntity, TExtendedDto, TCreationDto, TUpdateDto> where TEntity : class where TDto : class where TExtendedDto : class where TCreationDto : class where TUpdateDto : class
    {
        protected readonly IMapper _mapper;
        protected readonly IBasicRepository<TEntity> _basicRepository;
        public BasicService(IMapper mapper, IBasicRepository<TEntity> basicRepository)
        {
            _mapper=mapper;
            _basicRepository=basicRepository;
        }

        public async Task<TDto> CreateAsync(TCreationDto creationDto)
        {
            var entity = _mapper.Map<TEntity>(creationDto);
            await _basicRepository.AddAsync(entity);
            await _basicRepository.SaveChangesAsync();
            var entityToReturn = _mapper.Map<TDto>(entity);
            return entityToReturn;
        }

        public async Task<TDto> GetByIdAsync(int id)
        {
            var item = await _basicRepository.GetByIdAsync(id);
            var itemToReturn = _mapper.Map<TDto>(item);
            return itemToReturn;
        }

        public async Task<TEntity> GetEntityByIdAsync(int id)
        {
            var item = await _basicRepository.GetByIdAsync(id);
            return item;
        }

        public async Task<TDto> GetByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var item = await _basicRepository.GetByIdWithEagerLoadingAsync(id, includeProperties);
            var itemToReturn = _mapper.Map<TDto>(item);
            return itemToReturn;
        }

        public async Task<TExtendedDto> GetExtendedByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var item = await _basicRepository.GetByIdWithEagerLoadingAsync(id, includeProperties);
            var itemToReturn = _mapper.Map<TExtendedDto>(item);
            return itemToReturn;
        }

        public async Task<TEntity> GetEntityByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return await _basicRepository.GetByIdWithEagerLoadingAsync(id, includeProperties);
        }

        public async Task<IEnumerable<TDto>> GetAllAsync(IEnumerable<IFilter> filters, string searchQuery)
        {
            var query = _basicRepository.GetQueryableAll();

            foreach (var filter in filters)
            {
                query = ApplyFilter(query, filter);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = ApplySearch(query, searchQuery);
            }

            var finalListToReturn = await _mapper.ProjectTo<TDto>(query).ToListAsync();
            return finalListToReturn;
        }

        private IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, IFilter filter)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            var property = Expression.Property(parameter, filter.FieldName);
            var filterValue = Expression.Constant(filter.Value);

            Expression body;

            if (filter is NumericFilter)
            {
                body = Expression.Equal(property, filterValue);
            }
            else if (filter is TextFilter)
            {
                body = Expression.Equal(Expression.Call(property, "ToString", null), filterValue);
            }
            else
            {
                // Handle other filter types if needed.
                return query;
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);
            return query.Where(lambda);
        }

        private IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query, string searchQuery)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "entity");

            var properties = typeof(TEntity).GetProperties();

            Expression orExpression = null;

            foreach (var propertyInfo in properties)
            {
                var property = Expression.Property(parameter, propertyInfo);
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var call = Expression.Call(property, containsMethod, Expression.Constant(searchQuery));

                if (orExpression == null)
                {
                    orExpression = call;
                }
                else
                {
                    orExpression = Expression.Or(orExpression, call);
                }
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(orExpression, parameter);
            return query.Where(lambda);
        }

        public async Task<IEnumerable<TExtendedDto>> GetExtendedListWithEagerLoadingAsync(IEnumerable<int> ids, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            List<TExtendedDto> list = new List<TExtendedDto>();
            foreach (var id in ids) 
            {
                var add = await _basicRepository.GetByIdWithEagerLoadingAsync(id, includeProperties);
                list.Add(_mapper.Map<TExtendedDto>(add));
            }
            return list;
        }

        public async Task<OperationResult<TDto>> UpdateAsync(int id, TUpdateDto creationDto)
        {
            var item = await _basicRepository.GetByIdAsync(id);
            if( item == null)
            {
                return new OperationResult<TDto>
                {
                    IsSuccess = false,
                    ErrorMessage = $"Entity of type {typeof(TEntity).Name} with id={id} does not exist.",
                    HttpResponseCode = 404
                };
            }
            else
            {
                _mapper.Map(creationDto, item);
                await _basicRepository.SaveChangesAsync();
                return new OperationResult<TDto>
                {
                    IsSuccess = true,
                    HttpResponseCode = 204
                };
            }
        }

        public async Task<OperationResult<TDto>> PartialUpdateAsync(int id, JsonPatchDocument<TUpdateDto> patchDocument)
        {
            var item = await _basicRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new OperationResult<TDto>
                {
                    IsSuccess = false,
                    ErrorMessage = $"Entity of type {typeof(TEntity).Name} with id={id} does not exist.",
                    HttpResponseCode = 404
                };
            }
            else
            {
                var itemToPatch = _mapper.Map<TUpdateDto>(item);
                patchDocument.ApplyTo(itemToPatch);
                await _basicRepository.SaveChangesAsync();
                return new OperationResult<TDto>
                {
                    IsSuccess = true,
                    HttpResponseCode = 204
                };
            }
        }

        public async Task<(bool,TEntity?)> CheckIfIdExistsAsync(int id)
        {
            return await _basicRepository.CheckIfIdExistsAsync(id);
        }
        public virtual async Task<OperationResult<TDto>> DeleteByIdAsync(int id)
        {
            (bool exists, TEntity ? entity) result = await _basicRepository.CheckIfIdExistsAsync(id);
            if (result.exists == false)
            {
                return new OperationResult<TDto>
                {
                    IsSuccess = false,
                    ErrorMessage = $"Entity of type {typeof(TEntity).Name} with id={id} does not exist.",
                    HttpResponseCode = 404
                };
            }
            else
            {
                _basicRepository.DeleteAsync(result.entity);
                await _basicRepository.SaveChangesAsync();
                return new OperationResult<TDto>
                {
                    IsSuccess = true,
                    HttpResponseCode = 204
                };
            }
        }

    }
}
