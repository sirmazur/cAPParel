using cAPParel.API.Entities;
using cAPParel.API.Models;
using cAPParel.API.Services.Basic;

namespace cAPParel.API.Services.CategoryServices
{
    public interface ICategoryService : IBasicService<CategoryDto, Category, CategoryFullDto, CategoryForCreationDto, CategoryForUpdateDto>
    {
        Task<IEnumerable<CategoryDto>> GetAllSubcategories(int categoryId);
        Task<IEnumerable<int>> GetRelatedCategoriesIds(int categoryId);
    }
}
