using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniErp.Domain;
using MiniErp.EFCore;

namespace MiniErp.DbMigrator
{
    internal class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _lifeTime;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IHostApplicationLifetime lifeTime)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _lifeTime = lifeTime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetService<MiniErpContext>();
            context.Database.Migrate();
            //seeding role
            var admin = new Role { Id = Guid.Parse("aee52c96-6130-486e-bc75-394e5b0ac649"), Name = "Admin", Permission = ""};
            var manager = new Role { Id = Guid.Parse("007e88da-c584-4ca6-9a3f-ca2cab72b5db"), Name = "Manager", Permission = "" };
            var user = new Role { Id = Guid.Parse("ed6b6842-8938-4477-a8f8-316fbbddbb33"), Name = "User" , Permission = ""};
            context.Roles.AddRange(new List<Role> { admin, manager, user });
            await context.SaveChangesAsync();
            _lifeTime.StopApplication();
        }
    }
}
