using cAPParel.ConsoleClient.Services.ItemServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Controllers
{
    public class HomeController : IHomeController
    {
        private readonly IItemService _itemService;

        public HomeController(IItemService itemService)
        {
            _itemService = itemService;
        }

        public async Task RunAsync()
        {
            var items = await _itemService.GetItemsFriendly(true);
            foreach (var item in items.Value)
            {
                Console.WriteLine(item.Name);
            }
            Console.ReadKey();
        }
    }
}
