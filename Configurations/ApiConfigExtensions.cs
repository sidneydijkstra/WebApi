using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection{
    public static class ApiConfigExtensions
    {
        public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration){
            // setup config files
            services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
            services.Configure<RolesConfig>(configuration.GetSection("RolesConfig"));

            return services;
        }
    }
}