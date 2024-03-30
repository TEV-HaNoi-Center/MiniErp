using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.UI.DependencyInjection
{
    public static class IoC
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static IServiceScope Scope { get; set; }
        public static void Register(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public static T Resolve<T>() where T : class
        {
            if (Scope != null)
                Scope.Dispose();

            Scope = ServiceProvider.CreateScope();
            var service = Scope.ServiceProvider.GetRequiredService<T>();
            return service;
            
        }
    }
}
