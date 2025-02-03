using Inventory.Data;
using Inventory.Logger;
using Inventory.ViewModels;
using Microsoft.Extensions.Logging;

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
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddSingleton<IItemViewModel, ItemViewModel>();
            builder.Services.AddSingleton<ISaleViewModel, SaleViewModel>();
            builder.Services.AddScoped<IItemDetailViewModel, ItemDetailViewModel>();
            builder.Services.AddSingleton<IAppLogger, AppLogger>();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}