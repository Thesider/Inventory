using Inventory.Authentication;
using Inventory.Data;
using Inventory.Logger;
using Inventory.Service;
using Inventory.ViewModels;
using Inventory.ViewModels.Interface;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.Globalization;


namespace Inventory
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddLocalization();
            builder.Services.AddAuthorizationCore(config =>
            {
                config.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                config.AddPolicy("AdminOrClinic", policy => policy.RequireRole("Admin", "Clinic"));
                config.AddPolicy("AdminOrPharmacy", policy => policy.RequireRole("Admin", "Pharmacy"));
            });
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<IAppLogger, AppLogger>();
            builder.Services.AddSingleton<IItemViewModel, ItemViewModel>();
            builder.Services.AddSingleton<ISaleViewModel, SaleViewModel>();
            builder.Services.AddScoped<IItemDetailViewModel, ItemDetailViewModel>();
            builder.Services.AddScoped<IUserViewModel, UserViewModel>();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            var defaultCulture = Preferences.Get("AppCulture", "en");
            CultureInfo.CurrentCulture = new CultureInfo(defaultCulture);
            CultureInfo.CurrentUICulture = new CultureInfo(defaultCulture);


            return builder.Build();
        }
    }
}
