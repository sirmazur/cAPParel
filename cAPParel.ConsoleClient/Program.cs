using cAPParel.ConsoleClient;
using cAPParel.ConsoleClient.Controllers;
using cAPParel.ConsoleClient.Helpers;
using cAPParel.ConsoleClient.Services.ItemServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton<JsonSerializerOptionsWrapper>();
                    services.AddHttpClient<cAPParelAPIClient>();
                    services.AddScoped<IItemService, ItemService>();
                    services.AddSingleton<IHomeController, HomeController>();
                }).Build();

            await host.Services.GetRequiredService<IHomeController>().RunAsync();

            Console.ReadLine();




    



