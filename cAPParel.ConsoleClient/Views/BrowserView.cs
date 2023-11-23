using cAPParel.ConsoleClient.Helpers;
using cAPParel.ConsoleClient.Models;
using cAPParel.ConsoleClient.Services.CategoryServices;
using cAPParel.ConsoleClient.Services.ItemServices;
using cAPParel.ConsoleClient.Services.OrderServices;
using cAPParel.ConsoleClient.Services.UserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = cAPParel.ConsoleClient.Models.Color;

namespace cAPParel.ConsoleClient.Views
{
    public class BrowserView
    {
        private readonly IItemService _itemService;
        private readonly ICategoryService _categoryService;
        private CurrentUserData _currentUserData;
        public BrowserView(IItemService itemService, IUserService authenticationService, ICategoryService categoryService,
            IOrderService orderService)
        {
            _itemService = itemService;
            _categoryService = categoryService;
            _currentUserData = CurrentUserData.Instance;
        }

        public async Task GetItemsMenu()
        {
            ItemFilters? itemFilters = null;
            CategoryFullDto? category = null;
            bool showImages = false;
            bool exit = false;
            do
            {
                List<Option> options = new List<Option>()
                {
                new Option($"Category: {(category!=null ? category.CategoryName : "")}", async () => await Task.Run(async () =>{
                    if(itemFilters is null)
                    {
                        itemFilters = new ItemFilters();
                    }
                    category = await DisplayCategoriesSelectionMenu();
                    itemFilters.categoryid = category.Id;
                })),
                new Option($"Size: {(itemFilters!=null&&itemFilters.size!=null ? itemFilters.size : "")}", async () => await Task.Run(() =>{
                Console.Clear();
                Console.WriteLine("Size:");
                    if(itemFilters is null)
                    {
                        itemFilters = new ItemFilters();
                    }
                    itemFilters.size = Console.ReadLine();
                    Console.Clear();
                })),
                new Option($"Color: {(itemFilters!=null&&itemFilters.color!=null ? itemFilters.color : "")}", async () => await Task.Run(async () =>{
                Console.Clear();
                    if(itemFilters is null)
                    {
                        itemFilters = new ItemFilters();
                    }
                    itemFilters.color = await DisplayColorsSelectionMenu();
                })),
                new Option($"Show Only Available: {(itemFilters!=null&&itemFilters.isAvailable==true ? "on" : "off")}", async () => await Task.Run(() =>{
                    Console.Clear();
                    if(itemFilters is null)
                    {
                        itemFilters = new ItemFilters();
                    }
                    itemFilters.isAvailable = !itemFilters.isAvailable;
                })),
                new Option($"Show Item Images: {(showImages==true ? "on" : "off")}", async () => await Task.Run(() =>{
                    Console.Clear();
                    showImages = !showImages;
                })),
                new Option($"Search results", async () => await Task.Run(async () =>{
                    var exitResults = false;
                    do
                    {
                    Console.Clear();
                    LinkedResourceList<ItemFullDto>? items;
                    if(itemFilters is not null)
                    {
                        items = await _itemService.GetItemsFullAsync(itemFilters);
                    }
                    else
                    {
                        items = await _itemService.GetItemsFullAsync();
                    }
                    List<Option> options = new List<Option>();
                    if(items is not null && items.Value is not null)
                    {
                        foreach(var item in items.Value)
                            {
                                options.Add(new Option($"{item.Name}, ({item.Description} {(item.FileData.Any(c=>c.Type==DataType.Image)&&showImages ? "\n"+MenuBuilder.ConvertImageToAscii(item.FileData.FirstOrDefault(c=>c.Type==DataType.Image).Data, 32) : "")})", async () => await Task.Run(async () =>
                                {
                                    bool exitOptions = false;
                                    do{
                                List<Option> piecesOptions = new List<Option>();
                                foreach(var piece in item.Pieces)
                                {   if(piece.IsAvailable==true && !_currentUserData.GetShoppingCart().Any(c=>c.Id == piece.Id))
                                    piecesOptions.Add(new Option($"{piece.Size}", async () => await Task.Run(async () =>
                                    {
                                        _currentUserData.AddToShoppingCart(piece);
                                        item.Pieces.Remove(piece);
                                    })));
                                }
                                piecesOptions.Add(new Option("Back", async () => await Task.Run(() =>{exitOptions=true; })));
                                    await MenuBuilder.CreateSingularMenu(piecesOptions);
                                }while(!exitOptions);
                            })));
                        }
                         options.Add(new Option("Back",  async () => await Task.Run(() =>{
                        exitResults = true;
                        })));
                        await MenuBuilder.CreateSingularMenu(options);
                    }
                    }while(!exitResults);
                })),
                new Option("Generate pricing pdf for category", async() => await Task.Run(async () =>
                {
                    Console.Clear();
                    if(category is not null)
                    {
                        try
                        {
                            await _categoryService.GeneratePricingPdf(category.Id);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            await Task.Delay(3000);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please select a category first");
                        await Task.Delay(3000);
                    }
                })),
                new Option("Clear filters", async() => await Task.Run(() =>
                {
                    itemFilters = null;
                    category = null;
                })),
                new Option("Back", async () => await Task.Run(() =>{
                   exit = true;
                }))
                };
                await MenuBuilder.CreateSingularMenu(options);
            }
            while (!exit);
        }

        public async Task<Color> DisplayColorsSelectionMenu()
        {
            Console.Clear();
            List<Option> categoryOptions = new List<Option>();
            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                categoryOptions.Add(new Option(color.ToString(), async () => await Task.Run(() =>
                {
                    _currentUserData.SetColorChoice(color);
                    Console.Clear();
                })));
            }
            await MenuBuilder.CreateSingularColorMenu(categoryOptions);
            return _currentUserData.GetColorChoice();
        }

        public async Task<CategoryFullDto> DisplayCategoriesSelectionMenu()
        {
            Console.Clear();
            var categories = await _categoryService.GetCategoriesFull();
            var baseCategory = categories.Value.FirstOrDefault(x => x.ParentCategoryId == null);
            var listOfCategoryStrings = DisplayChildCategories(baseCategory, 0);
            List<Option> categoryOptions = new List<Option>();
            foreach (var categoryString in listOfCategoryStrings)
            {
                categoryOptions.Add(new Option(categoryString.Item1, async () => await Task.Run(() =>
                {
                    _currentUserData.SetCategory(categoryString.Item2);
                    Console.Clear();
                })));
            }
            await MenuBuilder.CreateSingularMenu(categoryOptions);
            return _currentUserData.GetCategory();
        }

        List<(string, CategoryFullDto)> DisplayChildCategories(CategoryFullDto category, int prefix)
        {
            List<(string, CategoryFullDto)> strings = new List<(string, CategoryFullDto)>();
            foreach (var childCategory in category.ChildCategories)
            {
                strings.Add(($"{new string(' ', prefix)}{childCategory.CategoryName}", childCategory));
                strings.AddRange(DisplayChildCategories(childCategory, prefix+1));
            }
            return strings;
        }


    }
}
