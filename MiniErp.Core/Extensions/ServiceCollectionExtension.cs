using Microsoft.Extensions.DependencyInjection;
using MiniErp.Core.Repositories;
using MiniErp.Core.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.Core.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}
