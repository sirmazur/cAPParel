using cAPParel.ConsoleClient.Models;
using cAPParel.ConsoleClient.Services.AuthenticationServices;
using cAPParel.ConsoleClient.Services.ItemServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Controllers
{
    public class HomeController : IHomeController
    {
        private readonly IItemService _itemService;
        private readonly IAuthenticationService _authenticationService;
        public HomeController(IItemService itemService, IAuthenticationService authenticationService)
        {
            _itemService = itemService;
            _authenticationService = authenticationService;
        }

        public async Task RunAsync()
        {
            while(true)
            {
                Console.WriteLine("1. Log in");
                var input = Console.ReadKey();
                Console.Clear();
                switch (input.Key)
                {
                    case ConsoleKey.D1:
                        await Authenticate();
                        break;
                    default:
                        Console.WriteLine("Invalid input");
                        break;
                }
            }

            var items = await _itemService.GetItemsFriendly(true);
            foreach (var item in items.Value)
            {
                Console.WriteLine(item.Name);
            }
            Console.ReadKey();
        }
        public async Task Authenticate()
        {
            Console.WriteLine("Enter username:");
            var userName = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Enter password:");            
            var password = Console.ReadLine();
            Console.Clear();
            try
            {
                await _authenticationService.Authenticate(userName, password);
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
            Console.WriteLine("Authenticated");
            await Task.Delay(3000);
            Console.Clear();

        }
    }
}
