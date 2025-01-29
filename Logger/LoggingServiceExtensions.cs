using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Services.Logging
{
    public static class LoggingServiceExtensions
    {
        public static IServiceCollection AddAppLogging(this IServiceCollection services)
        {
            services.AddSingleton<IAppLogger, AppLogger>();
            return services;
        }
    }
}