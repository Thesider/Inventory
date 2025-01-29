namespace Inventory.Logger
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