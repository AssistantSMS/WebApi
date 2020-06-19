using Microsoft.Extensions.DependencyInjection;
using ScrapMechanic.Data.Cache;
using ScrapMechanic.Data.Cache.Interface;
using ScrapMechanic.Domain.Configuration.Interface;
using ScrapMechanic.Integration.Repository;
using ScrapMechanic.Integration.Repository.Interface;

namespace ScrapMechanic.Data.Helper
{
    public static class RegisterServicesHelper
    {
        public static IServiceCollection RegisterCommonServices(this IServiceCollection services, IApiConfiguration config)
        {
            // Config Singletons
            services.AddSingleton(config);
            services.AddSingleton<IJwt>(construct => config.Jwt);
            services.AddSingleton<ILogging>(construct => config.Logging);
            services.AddSingleton<IDatabase>(construct => config.Database);

            // Repositories
            //services.AddTransient<IJwtRepository, JwtRepository>();

            services.AddTransient<IStreamNewsScrapeRepository, StreamNewsScrapeRepository>();
            services.AddTransient<IGithubRepository, GithubRepository>();

            // MemoryCache stuffs
            services.AddSingleton<ICustomCache, CustomCache>();


            // Services
            //services.AddTransient<IUserService, UserService>();

            return services;
        }

    }
}
