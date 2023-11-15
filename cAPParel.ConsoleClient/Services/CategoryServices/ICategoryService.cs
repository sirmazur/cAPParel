using cAPParel.ConsoleClient.Models;

namespace cAPParel.ConsoleClient.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<LinkedResourceList<CategoryDto>?> GetCategoriesFriendly(int? parentcategoryid = null, bool? includeLinks = false);
        Task<LinkedResourceList<CategoryFullDto>?> GetCategoriesFull(int? parentcategoryid = null, bool? includeLinks = false);
    }
}