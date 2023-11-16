using cAPParel.ConsoleClient.Services.UserServices;
using cAPParel.ConsoleClient.Services.ItemServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using cAPParel.ConsoleClient.Helpers;
using cAPParel.ConsoleClient.Services.CategoryServices;
using cAPParel.ConsoleClient.Models;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace cAPParel.ConsoleClient.Controllers
{
    public class HomeController : IHomeController
    {
        private readonly IItemService _itemService;
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private CurrentUserData _currentUserData;
        public HomeController(IItemService itemService, IUserService authenticationService, ICategoryService categoryService)
        {
            _itemService = itemService;
            _userService = authenticationService;
            _categoryService = categoryService;
            _currentUserData = CurrentUserData.Instance;
        }

        public async Task RunAsync()
        {
            List<Option> options = new List<Option>()
            {
                new Option("Log in", async () => await Authenticate()),
                new Option("Your Profile", async () => await GetSelfData()),
                new Option("Browse Clothing", async () => await GetItemsMenu()),
                new Option("Exit", () => Task.CompletedTask)
            };
            
            await CreateMenu(options);

            //while (true)
            //{

            //    Console.WriteLine("1. Log in");
            //    Console.WriteLine("2. Your Profile");
            //    var input = Console.ReadKey();
            //    Console.Clear();
            //    switch (input.Key)
            //    {
            //        case ConsoleKey.D1:
            //            await Authenticate();
            //            break;
            //        case ConsoleKey.D2:
            //            await GetSelfData();
            //            break;
            //        default:
            //            Console.WriteLine("Invalid input");
            //            break;
            //    }
            //}

            //var items = await _itemService.GetItemsFriendly(true);
            //foreach (var item in items.Value)
            //{
            //    Console.WriteLine(item.Name);
            //}
            //Console.ReadKey();
        }

        public async Task GetItemsMenu()
        {
            ItemFilters? itemFilters = null;
            CategoryFullDto? category = null;
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
                new Option($"ShowOnlyAvailable: {(itemFilters!=null&&itemFilters.isAvailable==true ? "on" : "off")}", async () => await Task.Run(() =>{
                    Console.Clear();
                    if(itemFilters is null)
                    {
                        itemFilters = new ItemFilters();
                    }
                    itemFilters.isAvailable = !itemFilters.isAvailable;
                })),
                new Option($"Search results", async () => await Task.Run(async () =>{
                    if(itemFilters is not null)
                    {
                        var items = await _itemService.GetItemsFull(itemFilters);
                    }
                    else
                    {
                        var items = await _itemService.GetItemsFull();
                    }
                    itemFilters = null;
                    category = null;
                })),
                new Option("Exit", async () => await Task.Run(() =>{
                   exit = true;
                }))
                };
                await CreateSingularMenu(options);
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
            await CreateSingularColorMenu(categoryOptions);
            return _currentUserData.GetColorChoice();
        }
        public async Task<CategoryFullDto> DisplayCategoriesSelectionMenu()
        {
            Console.Clear();
            var categories = await _categoryService.GetCategoriesFull();
            var baseCategory = categories.Value.FirstOrDefault(x => x.ParentCategoryId == null);
            var listOfCategoryStrings = DisplayChildCategories(baseCategory, 0);
            List<Option> categoryOptions = new List<Option>();
            foreach(var categoryString in listOfCategoryStrings)
            {
                categoryOptions.Add(new Option(categoryString.Item1, async () => await Task.Run(() =>
                {
                    _currentUserData.SetCategory(categoryString.Item2);
                    Console.Clear();
                })));
            }
            await CreateSingularMenu(categoryOptions);
            return _currentUserData.GetCategory();
        }
        List<(string,CategoryFullDto)> DisplayChildCategories(CategoryFullDto category, int prefix)
        {
            List<(string,CategoryFullDto)> strings = new List<(string,CategoryFullDto)>();
            foreach (var childCategory in category.ChildCategories)
            {
                strings.Add(($"{new string(' ', prefix)}{childCategory.CategoryName}",childCategory));
                strings.AddRange(DisplayChildCategories(childCategory, prefix+1));
            }
            return strings;
        }

        public async Task GetSelfData()
        {
            Console.Clear();
            if(_currentUserData.GetToken() is null)
            {
                Console.WriteLine("You are not logged in");
                await Task.Delay(3000);
                Console.Clear();
                return;
            }
            var user = await _userService.GetSelfFull();
            Console.WriteLine($"Name: {user.Username}");
            Console.WriteLine($"Role: {user.Role}");
            Console.WriteLine($"Saldo: {user.Saldo:F2}");
            Console.WriteLine($"Shopping cart:");
            int enumerator = 1;
            foreach(var item in _currentUserData.GetShoppingCart())
            {               
                Console.WriteLine($"{enumerator}: {item.Item.Name}, {item.Size}, {item.Color}");
                enumerator++;
            }
            Console.WriteLine($"Orders:");
            enumerator = 1;
            foreach(var order in user.Orders)
            {
                Console.WriteLine($"{enumerator}: Date - {order.DateCreated}, Total - {order.TotalPrice:F2}, State: {order.State} ");

            }
            Console.ReadKey();
            Console.Clear();
        }

        public async Task Authenticate()
        {
            Console.Clear();
            Console.WriteLine("Enter username:");
            var userName = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Enter password:");            
            var password = Console.ReadLine();
            Console.Clear();
            try
            {
                await _userService.Authenticate(userName, password);
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse response)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Console.WriteLine("Unauthorized: Check your credentials.");
                    }
                    else
                    {
                        Console.WriteLine($"HTTP Status Code: {response.StatusCode}");
                    }
                }
                else
                {
                    Console.WriteLine("An HTTP error occurred.");
                }

                await Task.Delay(3000);
                Console.Clear();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.Delay(3000);
                Console.Clear();
                return;
            } 
            Console.WriteLine($"Hello {userName}!");
            await Task.Delay(3000);
            Console.Clear();

        }
        async Task CreateSingularMenu(List<Option> options)
        {
            int index = 0;
            ConsoleKeyInfo keyinfo;
            do
            {
                WriteMenu(options, options[index]);
                keyinfo = Console.ReadKey();

                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < options.Count)
                    {
                        index++;
                        WriteMenu(options, options[index]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteMenu(options, options[index]);
                    }
                }

                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    if (index == options.Count - 1)
                    {
                        await options[index].Selected.Invoke();
                        break;
                    }
                    else
                    {
                        await options[index].Selected.Invoke();
                        break;
                    }

                }
            }
            while (true);
        }

        async Task CreateMenu(List<Option> options)
        {
            int index = 0;
            ConsoleKeyInfo keyinfo;
            do
            {
                WriteMenu(options, options[index]);
                keyinfo = Console.ReadKey();

                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < options.Count)
                    {
                        index++;
                        WriteMenu(options, options[index]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteMenu(options, options[index]);
                    }
                }

                if (keyinfo.Key == ConsoleKey.Enter)
                {   
                    if(index == options.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        await options[index].Selected.Invoke();
                        index = 0;
                    }
                    
                }
            }
            while (true);
        }


        static void WriteMenu(List<Option> options, Option selectedOption)
        {
            Console.Clear();

            foreach (Option option in options)
            {
                if (option == selectedOption)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(option.Name);
            }
            Console.ResetColor();
        }

        async Task CreateSingularColorMenu(List<Option> options)
        {
            int index = 0;
            ConsoleKeyInfo keyinfo;
            do
            {
                WriteColorMenu(options, options[index]);
                keyinfo = Console.ReadKey();

                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < options.Count)
                    {
                        index++;
                        WriteColorMenu(options, options[index]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteColorMenu(options, options[index]);
                    }
                }

                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    if (index == options.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        await options[index].Selected.Invoke();
                        break;
                    }

                }
            }
            while (true);
        }

        static void WriteColorMenu(List<Option> options, Option selectedOption)
        {
            Console.Clear();
            string prefix;
            foreach (Option option in options)
            {
                switch (option.Name)
                {
                    case "Red":
                        Console.BackgroundColor = ConsoleColor.Red;
                        break;
                    case "Blue":
                        Console.BackgroundColor = ConsoleColor.Blue;
                        break;
                    case "Green":
                        Console.BackgroundColor = ConsoleColor.Green;
                        break;
                    case "Yellow":
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        break;
                    case "Black":
                        Console.BackgroundColor = ConsoleColor.Black;
                        break;
                    case "White":
                        Console.BackgroundColor = ConsoleColor.White;
                        break;
                    case "Gray":
                        Console.BackgroundColor = ConsoleColor.Gray;
                        break;
                    case "Brown":
                        Console.BackgroundColor = ConsoleColor.DarkYellow; 
                        break;
                    case "Pink":
                        Console.BackgroundColor = ConsoleColor.Magenta; 
                        break;
                    case "Purple":
                        Console.BackgroundColor = ConsoleColor.DarkMagenta; 
                        break;
                    case "Orange":
                        Console.BackgroundColor = ConsoleColor.DarkYellow; 
                        break;
                    default:
                        break;
                }
                if (option == selectedOption)
                {
                    prefix= ">";
                }
                else
                {
                    prefix = " ";
                }
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(prefix+option.Name);
            }
            Console.ResetColor();
        }
    }
    internal class Option
    {
        public string Name { get; }
        public Func<Task> Selected { get; }

        public Option(string name, Func<Task> selected)
        {
            Name = name;
            Selected = selected;
        }
    }
}
