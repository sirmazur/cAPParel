using AutoMapper;
using cAPParel.API.DbContexts;
using cAPParel.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace cAPParel.API.Services.CategoryServices
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly cAPParelContext _context;
        public CategoryRepository(cAPParelContext context)
        {
            _context = context;
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithParentIdAsync(int parentid)
        {
            var result = await _context.Categories.Where(c => c.ParentCategoryId == parentid).ToListAsync();
            return result;
        }

        //public async Task<IEnumerable<Category>> GetFilteredCategoriesAsync(string? parentNameFilter, int? parentIdFilter)
        //{
        //    IQueryable<Category> result;
        //    if(parentNameFilter!=null)
        //    {
        //        result += _context.Categories.Where(c => c.CategoryName.Contains(parentNameFilter));
        //    }
        //}
    }
}
