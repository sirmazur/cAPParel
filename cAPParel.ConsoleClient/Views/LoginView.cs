using cAPParel.ConsoleClient.Services.UserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Views
{
    public class LoginView
    {
        private readonly IUserService _userService;
        public LoginView(IUserService authenticationService)
        {
            _userService = authenticationService;
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
    }
}
