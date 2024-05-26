using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniErp.EFCore;
using MiniErp.UI.FingerScanner;
using MiniErp.UI.Models;
using MiniErp.UI.Stores;
using MiniErp.UI.ViewModels;
using MiniErp.UI.Views;
using MiniErp.UI.Views.Category;
using MiniErp.UI.Views.Users;
using MiniErp.UI.Views.WarehouseManagement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.UI.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static void AddApplicationViews(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DbContext, MiniErpContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Default"));
            });
            var fireBaseKey = configuration["FIRE_BASE_KEY"].ToString();
            services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = fireBaseKey,
                Providers = new FirebaseAuthProvider[]
                {
                    new GoogleProvider().AddScopes("email"),
                    new EmailProvider()
                },
                AuthDomain = configuration["FIRE_BASE_AUTH_DOMAIN"].ToString()
            }));
            services.AddScoped<CurrencyView>();
            services.AddScoped<UnitView>();
            services.AddScoped<MainWindow>();
            services.AddScoped<MainContentView>();
            services.AddScoped<MainViewModel>();
            services.AddScoped<UnitViewModel>();
            services.AddScoped<CurrencyViewModel>();
            services.AddScoped<CustomerView>();
            services.AddScoped<CustomerViewModel>();
            services.AddScoped<ProviderView>();
            services.AddScoped<ProviderViewModel>();
            services.AddScoped<RoleView>();
            services.AddScoped<RoleViewModel>();
            services.AddScoped<ProductView>();
            services.AddScoped<ProductViewModel>();
            services.AddScoped<ReceiveNoteView>();
            services.AddScoped<ReceiveNoteMainViewModel>();
            services.AddScoped<ReceiveNoteViewModel>();
            services.AddScoped<DeliveryNoteViewModel>();
            services.AddScoped<DeliveryNoteMainViewModel>();
            services.AddScoped<DeliveryNoteMainView>();
            services.AddScoped<LoginView>();
            services.AddScoped<LoginViewModel>();
            services.AddScoped<RegisterView>();
            services.AddScoped<RegisterViewModel>();
            services.AddScoped<ResetPasswordViewModel>();
            services.AddScoped<PasswordResetView>();
            services.AddScoped<MainContentViewModel>();
            services.AddScoped<InventoryViewModel>();
            services.AddScoped<UserMainViewModel>();
            services.AddScoped<UserViewModel>();
            services.AddScoped<TimeKeepingViewModel>();
            services.AddScoped<RegisterFingersprintViewModel>();
            services.AddScoped<RegisterFingersprintView>();
            services.AddScoped<CheckOutView>();
            services.AddScoped<CheckOutViewModel>();
            services.AddScoped<PersonnelCalculationViewModel>();
            services.AddScoped<PersonnelCalculationView>();
            services.AddScoped<HomeView>();
            services.AddScoped<HomeViewModel>();



            services.AddSingleton<NavigationStore>();
            services.AddSingleton<MainContentStore>();
            services.AddSingleton<LoggedInUser>();
            services.AddSingleton(x =>
            {
                return FingerSensor.FindSensor();
            });

            services.AddTransient<SelectReceiveProductViewModel>();
            services.AddTransient<SelectDeliveryProductViewModel>();
        }
    }
}
