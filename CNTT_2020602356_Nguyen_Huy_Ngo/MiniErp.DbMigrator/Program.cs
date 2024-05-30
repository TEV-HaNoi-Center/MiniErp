// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniErp.DbMigrator;
using MiniErp.EFCore;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<MiniErpContext>(options =>
                options.UseSqlServer(context.Configuration.GetConnectionString("Default")));
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
