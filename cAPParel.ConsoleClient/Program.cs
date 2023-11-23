using cAPParel.ConsoleClient;
using cAPParel.ConsoleClient.Controllers;
using cAPParel.ConsoleClient.Helpers;
using cAPParel.ConsoleClient.Services.UserServices;
using cAPParel.ConsoleClient.Services.ItemServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using cAPParel.ConsoleClient.Services.CategoryServices;
using cAPParel.ConsoleClient.Services.OrderServices;
using cAPParel.ConsoleClient.Views;

using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton<JsonSerializerOptionsWrapper>();
                    services.AddHttpClient<cAPParelAPIClient>();
                    services.AddScoped<IItemService, ItemService>();
                    services.AddScoped<IUserService, UserService>();
                    services.AddScoped<ICategoryService, CategoryService>();
                    services.AddScoped<IOrderService, OrderService>();
                    services.AddSingleton<BrowserView>();
                    services.AddSingleton<RegisterView>();
                    services.AddSingleton<LoginView>();
                    services.AddSingleton<ProfileView>();
                    services.AddSingleton<IHomeController, HomeController>();
                }).Build();

            await host.Services.GetRequiredService<IHomeController>().RunAsync();




    



