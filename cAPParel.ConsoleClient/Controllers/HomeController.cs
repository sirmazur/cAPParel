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
using cAPParel.ConsoleClient.Services.OrderServices;
using System.Reflection.Metadata.Ecma335;
using Color = cAPParel.ConsoleClient.Models.Color;
using SixLabors.ImageSharp;

namespace cAPParel.ConsoleClient.Controllers
{
    public class HomeController : IHomeController
    {
        private readonly IItemService _itemService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;
        private CurrentUserData _currentUserData;
        public HomeController(IItemService itemService, IUserService authenticationService, ICategoryService categoryService, IOrderService orderService)
        {
            _itemService = itemService;
            _userService = authenticationService;
            _orderService = orderService;
            _categoryService = categoryService;
            _currentUserData = CurrentUserData.Instance;
        }
        [STAThread]
        public async Task RunAsync()
        {
            List<Option> options = new List<Option>()
            {
                new Option("Register", async () => await Register()),
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
        public async Task Register()
        {
            Console.Clear();
            Console.WriteLine("Enter your desired username:");
            var username = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Enter your desired password:");
            var password = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Enter your invitation code if you have one:");
            var invitationCode = Console.ReadLine();
            Console.Clear();
            try
            {
                await _userService.Register(new UserForClientCreation()
                {
                    Username = username,
                    Password = password,
                    AdminCode = invitationCode
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await Task.Delay(3000);
                return;
            }
            Console.WriteLine("Registration successful! You can now log in.");
            await Task.Delay(3000);
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
                                options.Add(new Option($"{item.Name}, ({item.Description} {(item.FileData.Any(c=>c.Type==DataType.Image)&&showImages ? "\n"+ConvertImageToAscii(item.FileData.FirstOrDefault(c=>c.Type==DataType.Image).Data, 32) : "")})", async () => await Task.Run(async () =>
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
                                    await CreateSingularMenu(piecesOptions);
                                }while(!exitOptions);
                            })));
                        }
                         options.Add(new Option("Back",  async () => await Task.Run(() =>{
                        exitResults = true;
                        })));
                        await CreateSingularMenu(options);
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
        public async Task DisplayAdminCategoriesSelectionMenu()
        {
            bool exicAdminCategoriesSelectionMenu = false;
            do
            {
                Console.Clear();
                var categories = await _categoryService.GetCategoriesFull();
                var baseCategory = categories.Value.FirstOrDefault(x => x.ParentCategoryId == null);
                var listOfCategoryStrings = DisplayChildCategories(baseCategory, 1);
                List<Option> categoryOptions = new List<Option>();
                categoryOptions.Add(new Option($"{baseCategory.CategoryName}", async () => await Task.Run(async () =>
                {
                    bool exitmanageCategoryOptions = false;
                    do
                    {
                        List<Option> manageCategoryOptions = new List<Option>()
                    {

                    new Option("Add Subcategory", async () => await Task.Run(async () =>
                    {
                        Console.Clear();
                        Console.WriteLine("Enter category name:");
                        var categoryName = Console.ReadLine();
                        Console.Clear();
                        await _categoryService.CreateCategoryAsync(new CategoryForCreationDto()
                        {
                            CategoryName = categoryName,
                            ParentCategoryId = baseCategory.Id
                        });
                    })),
                    new Option("Back", () => Task.Run(()=>{bool exitmanageCategoryOptions = false;}))
                    };
                        await CreateSingularMenu(manageCategoryOptions);
                    } while (!exitmanageCategoryOptions);
                })));
                foreach (var categoryString in listOfCategoryStrings)
                {
                    categoryOptions.Add(new Option(categoryString.Item1, async () => await Task.Run(async () =>
                    {
                        bool exitmanageCategoryOptions = false;
                        do
                        {
                            List<Option> manageCategoryOptions = new List<Option>()
                    {
                    new Option("Add Subcategory", async () => await Task.Run(async () =>
                    {
                        Console.Clear();
                        Console.WriteLine("Enter category name:");
                        var categoryName = Console.ReadLine();
                        Console.Clear();
                        await _categoryService.CreateCategoryAsync(new CategoryForCreationDto()
                        {
                            CategoryName = categoryName,
                            ParentCategoryId = categoryString.Item2.Id
                        });
                    })),
                    new Option("Delete this category", async () => await Task.Run(async () =>
                    {
                        Console.Clear();
                        exitmanageCategoryOptions=true;
                        try
                        {
                        await _categoryService.DeleteCategoryAsync(categoryString.Item2.Id);
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                            await Task.Delay(3000);
                        }
                    })),
                    new Option("Back", () => Task.Run(()=>{exitmanageCategoryOptions=true; }))
                    };
                            await CreateSingularMenu(manageCategoryOptions);
                        } while (!exitmanageCategoryOptions);
                    })));
                }
                categoryOptions.Add(new Option("Back", () => Task.Run(() => { exicAdminCategoriesSelectionMenu = true; })));
                await CreateSingularMenu(categoryOptions);
            } while (!exicAdminCategoriesSelectionMenu);
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
            bool exit = false;
            do
            {
                Console.Clear();
                if (_currentUserData.GetToken() is null)
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
                List<Option> options = new List<Option>()
                {                    
                new Option($"Top up your account | current balance: {user.Saldo:F2}", async () => await Task.Run(async () =>{
                    Console.Clear();
                    Console.WriteLine("Enter target balance:");
                    double amount = double.Parse(Console.ReadLine());
                    await _userService.TopUpAccountAsync(user.Id, amount);                    
                })),
                new Option("Show shopping cart", async () => await Task.Run(async () =>{
                    bool exitCart = false;
                    do{
                    Console.Clear();
                    var cartItems = _currentUserData.GetShoppingCart();
                    Console.WriteLine($"Shopping cart total: {cartItems.Sum(p => (p.Item.Price)*p.Item.PriceMultiplier):F2}\nSelect any item below to remove.");
                    var shoppingCartItemOptions = new List<Option>();
                    foreach (var item in _currentUserData.GetShoppingCart())
                    {
                        shoppingCartItemOptions.Add(new Option($"{item.Item.Name}, {item.Size}, {item.Item.Price}", async () => await Task.Run(() =>
                        {
                            _currentUserData.RemoveFromShoppingCart(item);
                        })));
                    }
                    shoppingCartItemOptions.Add(new Option("Back", async () => await Task.Run(() =>{exitCart=true; })));
                    await CreateSingularMenu(shoppingCartItemOptions);
                    }while(!exitCart);
                })),
                new Option("Purchase cart items", async () => await Task.Run(async () =>
                {
                    Console.Clear();
                    if(_currentUserData.GetShoppingCart().Count == 0)
                    {
                        Console.WriteLine("Your shopping cart is empty");
                        await Task.Delay(3000);
                        Console.Clear();
                        return;
                    }
                    var idList = _currentUserData.GetShoppingCart().Select(x => x.Id).ToList();
                    var order = await _orderService.CreateOrderAsync(idList);
                    Console.WriteLine($"Order created: {order.DateCreated}, Total: {order.TotalPrice:F2}");
                    _currentUserData.ClearShoppingCart();
                    await Task.Delay(3000);
                    Console.Clear();
                })),
                new Option("Previous orders", async () => await Task.Run(async () =>
                {
                    bool exitOrders = false;
                    do{
                    List<Option> orderOptions = new List<Option>();
                    var user = await _userService.GetSelfFull();
                    foreach(var order in user.Orders)
                    {
                        orderOptions.Add(new Option($"{order.DateCreated}, {order.TotalPrice:F2}, {order.State}", async () => await Task.Run(async () =>
                        {
                            Console.Clear();
                            var orderInfo = await _orderService.GetOrderFullAsync(order.Id);
                            var orderItemIds = orderInfo.Pieces.Select(x => x.ItemId).ToList();
                            var orderitems = await _itemService.GetItemsFullAsync(new ItemFilters(){ids=orderItemIds});
                            foreach(var piece in orderInfo.Pieces)
                            {
                                var item = orderitems.Value.FirstOrDefault(c=>c.Id == piece.ItemId);
                                Console.WriteLine($"{item.Name}, {piece.Size}, {item.Price:F2}");
                            }
                            List<Option> manageOrderOptions = new List<Option>();
                            manageOrderOptions.Add(new Option("View items", async () => await Task.Run(async () =>
                            {
                                Console.Clear();
                                 foreach(var piece in orderInfo.Pieces)
                                 {
                                    var item = orderitems.Value.FirstOrDefault(c=>c.Id == piece.ItemId);
                                    Console.WriteLine($"{item.Name}, {piece.Size}, {item.Price:F2}");
                                 }
                                 List<Option> viewItemOptions = new List<Option>()
                                 {
                                     new Option("Back",  () => Task.CompletedTask)
                                 };
                                Console.ReadKey();
                            })));
                            if(order.State == State.Accepted)
                            {
                                manageOrderOptions.Add(new Option("Cancel Order", async () => await Task.Run(async () =>
                                {
                                    await _orderService.CancelOrderAsync(order.Id);
                                })));
                            }
                            manageOrderOptions.Add(new Option("Back", () => Task.CompletedTask));
                            await CreateSingularMenu(manageOrderOptions);
                        })));
                    }
                    orderOptions.Add(new Option("Back", async () => await Task.Run(() =>{exitOrders=true; })));
                        await CreateSingularMenu(orderOptions);
                    }
                    while(!exitOrders);
                }))
            };
                if(user.Role is Role.Admin)
                {
                    options.Add(new Option("Open admin panel", async () => await Task.Run(async () => 
                    {
                        List<Option> adminOptions = new List<Option>();
                        adminOptions.Add(new Option("Add Item", async () => await Task.Run(async () =>
                        {
                            bool exitItemCreation = false;
                            ItemForCreationDto item = new ItemForCreationDto();
                            string? name = null;
                            double? price = null;
                            ItemType? type = null;
                            string? description = null;
                            Color? color = null;
                            FileDataForCreationDto? image = null;
                            CategoryFullDto? category = null;
                            do
                            {
                                List<Option> itemInfoOptions = new List<Option>()
                            {
                                new Option($"Name: {(name is not null ? name : "")}", async () => await Task.Run(() =>
                                {
                                    Console.Clear();
                                    Console.WriteLine("Enter name:");
                                    name = Console.ReadLine();
                                })),
                                new Option($"Price: {(price is not null ? price : "")}", async () => await Task.Run(async () =>
                                {
                                    Console.Clear();
                                    Console.WriteLine("Enter price:");
                                    try
                                    {
                                        price = Convert.ToDouble(Console.ReadLine());
                                    }catch(Exception ex)
                                    {
                                        Console.WriteLine("Invalid price");
                                        await Task.Delay(3000);
                                    }

                                })),
                                new Option($"Color: {(color is not null ? color.ToString() : "")}", async () => await Task.Run(async () =>
                                {
                                    Console.Clear();

                                    color = await DisplayColorsSelectionMenu();

                                })),
                                new Option($"Category: {(category is not null ? category.CategoryName : "")}", async () => await Task.Run(async () =>{
                                Console.Clear();
                                category = await DisplayCategoriesSelectionMenu();

                                })),
                                new Option($"Type: {(type is not null ? type.ToString() : "")}", async () => await Task.Run(async () =>{
                                Console.Clear();
                                List<Option> typeOptions = new List<Option>();
                                    foreach(var typeOption in Enum.GetValues(typeof(ItemType)))
                                    {
                                    typeOptions.Add(new Option(typeOption.ToString(), async () => await Task.Run(() =>
                                    {
                                        type = (ItemType)typeOption;
                                    })));
                                    }                                    
                                    await CreateSingularMenu(typeOptions);
                                })),
                                new Option($"Description: {(description is not null ? description : "")}", async () => await Task.Run(() =>
                                {
                                    Console.Clear();
                                    Console.WriteLine("Enter description:");
                                    description = Console.ReadLine();
                                })),
                                new Option($"Image: \n{(image is not null ? ConvertImageToAscii(image.Data, 32) : "")}", async () => await Task.Run(async () =>
                                {
                                    Console.Clear();
                                    string folderName = "Images";

                                    string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);

                                    Directory.CreateDirectory(folderPath);
                                    List<Option> fileOptions = new List<Option>();
                                    foreach (string filePath in Directory.GetFiles(folderPath))
                                    {
                                        fileOptions.Add(new Option(Path.GetFileName(filePath), async () => await Task.Run(() =>
                                        {
                                            Console.Clear();
                                            image = new FileDataForCreationDto()
                                            {
                                                Data = File.ReadAllBytes(filePath),
                                                Description = "Main image",
                                                Type = DataType.Image
                                            };
                                        })));
                                    }
                                    await CreateSingularMenu(fileOptions);
                                })),
                                new Option("Finalize Item Creation", async () => await Task.Run( async () =>
                                {
                                    ItemForCreationDto itemToCreate = new ItemForCreationDto()
                                    {
                                        Name = name,
                                        Price = (double)price,
                                        Type = type.Value,
                                        Color = color.Value,                                       
                                        Description = description,
                                        CategoryId = category.Id,
                                        FileData = new List<FileDataForCreationDto>(){image}

                                    };
                                    var createdItem = await _itemService.CreateItemAsync(itemToCreate);
                                    Console.Clear();
                                    Console.WriteLine($"Item created with id: {createdItem.Id}");
                                    await Task.Delay(3000);
                                    exitItemCreation = true;
                                })),
                                new Option("Back", async () => await Task.Run(() => { exitItemCreation = true;}))
                            };
                                await CreateSingularMenu(itemInfoOptions);
                            }while(!exitItemCreation);
                        })));
                        adminOptions.Add(new Option("Add or remove pieces of an item", async () => await GetAdminItemsMenu()));
                        adminOptions.Add(new Option("Manage categories", async () => await DisplayAdminCategoriesSelectionMenu()));
                        adminOptions.Add(new Option("Manage clients' orders", async () => await Task.Run(async () =>
                        {
                            await GetOrdersMenu();
                        })));
                        adminOptions.Add(new Option("Back", () => Task.CompletedTask));
                        await CreateMenu(adminOptions);
                    })));
                }
                options.Add(new Option("Back", async () => await Task.Run(() => { exit=true; })));
                await CreateSingularMenu(options);
            } while (!exit);
        }

        public async Task GetOrdersMenu()
        {
            Console.Clear();
            bool exit = false;
            do
            {

                var orders = await _orderService.GetOrdersFullAsync("DateCreated");
                List<Option> orderMenuOptions = new List<Option>();
                List<int> userIds = orders.Value
                .Select(order => order.UserId)
                .Distinct()
                .ToList();
                var users = await _userService.GetUsersFullAsync(userIds);
                foreach (var order in orders.Value)
                {

                    if (order.State==State.Ongoing || order.State==State.Accepted)

                        orderMenuOptions.Add(new Option($"{order.DateCreated}, State:{order.State.ToString()}, Total:{order.TotalPrice}, User:{users.Value.FirstOrDefault(u => u.Id == order.UserId).Username}", async () => await Task.Run(async () =>
                        {
                            bool exitOrderOptions = false;
                            do
                            {
                                Console.Clear();
                                List<Option> manageOrderOptions = new List<Option>()
                            {
                            new Option("Proceed order", async () => await Task.Run(async () =>
                            {
                                Console.Clear();
                                order.State++;
                                await _orderService.PatchOrderAsync(order.State++, order.Id);
                                exitOrderOptions = true;
                            })),

                            new Option("Cancel order", async () => await Task.Run(async () =>
                            {
                                 Console.Clear();
                                await _orderService.CancelOrderAsync(order.Id);
                                exitOrderOptions = true;
                            })),
                            new Option("Display items", async () => await Task.Run(async () =>
                            {
                                Console.Clear();
                                var orderItemIds = order.Pieces.Select(x => x.ItemId).ToList();
                                var orderitems = await _itemService.GetItemsFullAsync(new ItemFilters(){ids=orderItemIds});
                                foreach(var piece in order.Pieces)
                                {
                                    var item = orderitems.Value.FirstOrDefault(c=>c.Id == piece.ItemId);
                                    Console.WriteLine($"{item.Name}, {piece.Size}, {item.Price:F2}");
                                }
                                Console.ReadKey();
                            })),
                            new Option("Back",() => Task.Run(() =>
                            {
                                exitOrderOptions = true;
                            }))
                        };
                                await CreateSingularMenu(manageOrderOptions);
                            } while (!exitOrderOptions);
                        })));
                }
                orderMenuOptions.Add(new Option("Back", () => Task.Run(() => { exit=true; })));
                await CreateSingularMenu(orderMenuOptions);
            } while (!exit);
        }

        public async Task GetAdminItemsMenu()
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
                    bool exitResults = false;
                    do{
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
                            options.Add(new Option($"{item.Name}", async () => await Task.Run(async () =>
                            {
                                bool exitOptions = false;
                                do{
                                List<Option> piecesOptions = new List<Option>();
                                foreach(var piece in item.Pieces)
                                {   if(piece.IsAvailable==true)
                                    piecesOptions.Add(new Option($"{piece.Size}", async () => await Task.Run(async () =>
                                    {
                                        await _itemService.DeletePieceAsync(piece.Id);
                                        item.Pieces.Remove(piece);
                                    })));
                                }
                                piecesOptions.Add(new Option("Add more pieces", async () => await Task.Run(async () =>{
                                string? size= null;
                                do
                                {
                                Console.Clear();
                                Console.WriteLine("Size:");
                                size = Console.ReadLine();
                                }
                                while(size is null);
                                var pieceToCreate = new PieceForCreationDto()
                                {
                                    Size = size
                                };
                                await _itemService.CreatePieceAsync(item.Id, pieceToCreate);
                                })));
                                piecesOptions.Add(new Option("Remove item and every piece", async () => await Task.Run(async () =>
                                {
                                    await _itemService.DeleteItemAsync(item.Id);
                                    exitOptions=true;
                                })));
                                piecesOptions.Add(new Option("Back", async () => await Task.Run(() =>{exitOptions=true; })));
                                await CreateSingularMenu(piecesOptions);
                                }while(!exitOptions);
                            })));
                        }
                         options.Add(new Option("Back",async () => await Task.Run(() =>{
                         exitResults = true;
                        })));
                        await CreateSingularMenu(options);                            
                    }
                    }while(!exitResults);
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
                await CreateSingularMenu(options);
            }
            while (!exit);
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
        static string ConvertImageToAscii(byte[] imageBytes, int width)
        {
            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(imageBytes))
            {

                int height = (int)Math.Ceiling((double)width * image.Height / image.Width);

                image.Mutate(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Max
                }));

                string asciiChars = "@%#*+=-:. ";
                int totalChars = asciiChars.Length;

                var result = new char[width * height + height]; 
                int index = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x < image.Width && y < image.Height)
                        {
                            var pixel = image[x, y];
                            int brightness = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);

                            int charIndex = brightness * (totalChars - 1) / 255;
                            result[index++] = asciiChars[charIndex];
                        }
                    }

                    if (y < height - 1)
                        result[index++] = '\n'; 
                }

                return new string(result);
            }
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
