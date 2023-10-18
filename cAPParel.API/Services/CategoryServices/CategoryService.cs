using AutoMapper;
using cAPParel.API.Entities;
using cAPParel.API.Models;
using cAPParel.API.Services.Basic;
using Microsoft.AspNetCore.HttpOverrides;
using System.Runtime.CompilerServices;

namespace cAPParel.API.Services.CategoryServices
{
    public class CategoryService : BasicService<CategoryDto, Category, CategoryDto, CategoryForCreationDto, CategoryForUpdateDto>, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IBasicRepository<Category> basicRepository) : base(mapper,basicRepository)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public override async Task<OperationResult<CategoryDto>> DeleteByIdAsync(int id)
        {
            (bool exists, Category? entity) result = await _basicRepository.CheckIfIdExistsAsync(id);
            if (result.exists == false)
            {
                return new OperationResult<CategoryDto>
                {
                    IsSuccess = false,
                    ErrorMessage = $"Entity of type {typeof(Category).Name} with id={id} does not exist.",
                    HttpResponseCode = 404
                };
            }
            else
            {
                if(result.entity.ParentCategoryId!=null)
                {
                    var categories = await _categoryRepository.GetCategoriesWithParentIdAsync(id);
                    foreach(var category in categories)
                    {
                        category.ParentCategoryId = result.entity.ParentCategoryId;
                    }
                }
                else
                {
                    var categories = await _categoryRepository.GetCategoriesWithParentIdAsync(id);
                    foreach (var category in categories)
                    {
                        category.ParentCategoryId = null;
                    }
                }
                _basicRepository.DeleteAsync(result.entity);
                await _basicRepository.SaveChangesAsync();
                return new OperationResult<CategoryDto>
                {
                    IsSuccess = true,
                    HttpResponseCode = 204
                };
            }
        }
    }
}
