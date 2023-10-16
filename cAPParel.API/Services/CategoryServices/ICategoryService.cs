using cAPParel.API.Entities;
using cAPParel.API.Models;
using cAPParel.API.Services.Basic;

namespace cAPParel.API.Services.CategoryServices
{
    public interface ICategoryService : IBasicService<CategoryDto, Category, CategoryDto>
    {
        Task<CategoryDto> CreateCategoryAsync(CategoryForCreationDto category);
    }
}
