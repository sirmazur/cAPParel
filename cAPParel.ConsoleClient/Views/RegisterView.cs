using cAPParel.ConsoleClient.Helpers;
using cAPParel.ConsoleClient.Models;
using cAPParel.ConsoleClient.Services.UserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Views
{
    public class RegisterView
    {
        private readonly IUserService _userService;
        public RegisterView(IUserService authenticationService)
        {
            _userService = authenticationService;
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
    }
}
