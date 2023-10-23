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

        public async Task<IEnumerable<TDto>> GetAllAsync(IEnumerable<IFilter> filters)
        {
            var listToReturn = await _basicRepository.GetAllAsync();

                foreach (var filter in filters)
                {
                listToReturn = listToReturn.Where(entity =>
                {
                    var propertyInfo = entity.GetType().GetProperty(filter.FieldName);

                    if (propertyInfo != null)
                    {
                        var propertyValue = propertyInfo.GetValue(entity, null);

                        if(filter is NumericFilter)
                        {
                            var filterValue = Convert.ToInt32(filter.Value);
                            return propertyValue != null && Convert.ToInt32(propertyValue) == filterValue;
                        }
                        else if(filter is TextFilter)
                        {
                            var filterValue = filter.Value.ToString();
                            return propertyValue != null && propertyValue.ToString() == filterValue;
                        }
                    }
                    return true;
                });
            }
                
            var finalListToReturn = _mapper.Map<IEnumerable<TDto>>(listToReturn);
            return finalListToReturn;
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
