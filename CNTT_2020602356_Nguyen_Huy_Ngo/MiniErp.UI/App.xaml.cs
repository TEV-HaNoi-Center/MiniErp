using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniErp.Core.Extensions;
using MiniErp.Domain;
using MiniErp.UI.DependencyInjection;
using MiniErp.UI.FingerScanner;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace MiniErp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public App()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var services = new ServiceCollection();
            services.RegisterUnitOfWork();
            services.AddApplicationViews(builder.Build());
            var serviceProvider = services.BuildServiceProvider();
            IoC.Register(serviceProvider);
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);
            var mainWd = IoC.Resolve<MainWindow>();
            mainWd.Show();
            var fingerSensor = IoC.ServiceProvider.GetRequiredService<FingerSensor>();
            if (fingerSensor.IsValid)
            {
                Thread t1 = new Thread(() =>
                {
                    while (true)
                    {
                        if (fingerSensor.IsAdding)
                        {
                            fingerSensor.AddResult = fingerSensor.AddFinger(fingerSensor.Index, 10);
                            continue;
                        }

                        int id = fingerSensor.CheckFinger(1);
                        if (id <= 0)
                            continue;
                        else
                        {
                            var code = id.ToString();
                            var dbContext = IoC.ServiceProvider.GetRequiredService<DbContext>();
                            var user = dbContext.Set<User>().FirstOrDefault(x => x.FingerprintCode == code);
                            if (user == null)
                                continue;
                            var check = new TimeKeeping { UserId = user.Id, CheckTime = DateTime.Now, Date = DateTime.Today };
                            dbContext.Set<TimeKeeping>().Add(check);
                            dbContext.SaveChanges();
                        }
                    }
                });
                t1.Start();
            }
        }
    }

}
