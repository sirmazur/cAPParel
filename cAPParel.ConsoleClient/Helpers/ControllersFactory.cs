using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Helpers
{
    public class ControllersFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ControllersFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TController CreateController<TController>() where TController : class
        {
            using var scope = _serviceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<TController>();
        }
    }
}
