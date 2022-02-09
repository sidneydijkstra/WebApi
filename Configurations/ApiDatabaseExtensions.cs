using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;

namespace Microsoft.Extensions.DependencyInjection{
    public static class ApiDatabaseExtensions{
        public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration){
            // setup database service
            services.AddDbContextPool<APIDatabaseContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(
                        // Replace with your connection string.
                        configuration.GetConnectionString("DefaultConnection"),
                        // Replace with your server version and type.
                        // For common usages, see pull request #1233.
                        new MySqlServerVersion(new Version(8, 0, 21)) // use MariaDbServerVersion for MariaDB
                        //mySqlOptions => mySqlOptions
                        //    .CharSetBehavior(CharSetBehavior.NeverAppend))
                    )
                    // Everything from this point on is optional but helps with debugging.
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            );

            return services;
        }
    }
}