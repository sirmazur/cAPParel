using AutoMapper;
using cAPParel.API.Entities;
using cAPParel.API.Models;
using cAPParel.API.Services.Basic;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;
using System.Runtime.CompilerServices;
using QuestPDF.Previewer;
using QuestPDF.Helpers;
using cAPParel.API.Migrations;

namespace cAPParel.API.Services.CategoryServices
{
    public class CategoryService : BasicService<CategoryDto, Category, CategoryFullDto, CategoryForCreationDto, CategoryForUpdateDto>, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IBasicRepository<Category> basicRepository) : base(mapper,basicRepository)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            QuestPDF.Settings.License=LicenseType.Community;
        }
        
        public async Task<(byte[],string)> GeneratePdfForCategoryAsync(int id)
        {
            var items = await _categoryRepository.GetItemsByCategoryIdAsync(id);
            var category = await _basicRepository.GetByIdAsync(id);
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Header().AlignCenter()
                    .Text($"{category.CategoryName} category price list.")
                    .Bold().FontSize(30).FontColor(Colors.Black);

                    page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(20);
                        foreach (var item in items)
                        {
                            x.Item().Text($"{item.Name} - {item.Price} USD").FontSize(20).FontColor(Colors.Black);
                            var bytes = item.FileData.FirstOrDefault(c => c.Type==0);
                            x.Item().Image(bytes.Data);
                        }
                    });

                    page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
                });
            })
            .GeneratePdf($"cAPParel_{category.CategoryName}_Pricing.pdf");
            byte[] pdfBytes = await File.ReadAllBytesAsync($"cAPParel_{category.CategoryName}_Pricing.pdf");
            File.Delete($"cAPParel_{category.CategoryName}_Pricing.pdf");
            return (pdfBytes, $"cAPParel_{category.CategoryName}_Pricing.pdf");

        }
        public async Task<IEnumerable<int>> GetRelatedCategoriesIds(int categoryId)
        {
            var categories = await GetAllSubcategories(categoryId);
            var IdList = categories.Select(c => c.Id).ToList();
            IdList.Add(categoryId);
            return IdList;
        }
        public async Task<IEnumerable<CategoryDto>> GetAllSubcategories(int categoryId)
        {
            var items = await _basicRepository.GetAllAsync();
            var subCategories = CreateSubCategoriesList(items, categoryId);
            var subCategoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(subCategories);
            return subCategoriesDto;

        }
        private IEnumerable<Category> CreateSubCategoriesList(IEnumerable<Category> categories, int? parentId)
        {
            List<Category> categoriesList = new List<Category>();
            var subCategories = categories.Where(c => c.ParentCategoryId == parentId);
            foreach(var category in subCategories)
            {
                categoriesList.Add(category);
                categoriesList.Concat(CreateSubCategoriesList(categories, category.Id));
            }
            return categoriesList;
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
