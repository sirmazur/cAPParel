using cAPParel.API.Entities;
using cAPParel.API.Services.Basic;

namespace cAPParel.API.Services.CategoryServices
{
    public interface ICategoryRepository
    {
        Task<Category> AddCategoryAsync(Category category);
        Task<IEnumerable<Category>> GetCategoriesWithParentIdAsync(int parentid);
    }
}
