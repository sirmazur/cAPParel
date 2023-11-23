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
using cAPParel.ConsoleClient.Views;
using cAPParel.ConsoleClient.Helpers;

namespace cAPParel.ConsoleClient.Controllers
{
    public class HomeController : IHomeController
    {
        private readonly IItemService _itemService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;
        private readonly ProfileView _profileView;
        private readonly BrowserView _browserView;
        private readonly RegisterView _registerView;
        private readonly LoginView _loginView;
        private CurrentUserData _currentUserData;
        public HomeController(IItemService itemService, IUserService authenticationService, ICategoryService categoryService, 
            IOrderService orderService, BrowserView browserView, RegisterView registerView, LoginView loginView, ProfileView profileView)
        {
            _itemService = itemService;
            _userService = authenticationService;
            _orderService = orderService;
            _categoryService = categoryService;
            _currentUserData = CurrentUserData.Instance;
            _profileView = profileView;
            _browserView=browserView;
            _registerView=registerView;
            _loginView=loginView;
        }

        public async Task RunAsync()
        {
            List<Option> options = new List<Option>()
            {
                new Option("Register", async () => await _registerView.Register()),
                new Option("Log in", async () => await _loginView.Authenticate()),               
                new Option("Your Profile", async () => await _profileView.GetSelfData()),
                new Option("Browse Clothing", async () => await _browserView.GetItemsMenu()),
                new Option("Exit", () => Task.CompletedTask)
            };
            
            await MenuBuilder.CreateMenu(options);
           
        }       

                        
    }    
}
