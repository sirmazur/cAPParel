﻿using cAPParel.ConsoleClient.Services.UserServices;
using cAPParel.ConsoleClient.Services.ItemServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using cAPParel.ConsoleClient.Helpers;

namespace cAPParel.ConsoleClient.Controllers
{
    public class HomeController : IHomeController
    {
        private readonly IItemService _itemService;
        private readonly IUserService _userService;
        private CurrentUserData _currentUserData;
        public HomeController(IItemService itemService, IUserService authenticationService)
        {
            _itemService = itemService;
            _userService = authenticationService;
            _currentUserData = CurrentUserData.Instance;
        }

        public async Task RunAsync()
        {
            List<Option> options = new List<Option>()
            {
                new Option("Log in", async () => await Authenticate()),
                new Option("Your Profile", async () => await GetSelfData()),
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