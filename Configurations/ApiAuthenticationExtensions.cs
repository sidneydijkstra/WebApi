using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection{
    public static class ApiAuthenticationExtensions{
        public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration){
            // get JWT secret
            var key = Encoding.ASCII.GetBytes(configuration["JwtConfig:Secret"]);

            // setup standart token validation
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // this will validate the 3rd part of the jwt token using the secret that we added in the appsettings and verify we have generated the jwt token
                IssuerSigningKey = new SymmetricSecurityKey(key), // Add the secret key to our Jwt encryption
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                RequireExpirationTime = false,
                ClockSkew = TimeSpan.Zero
            };

            // within this section we are configuring the authentication and setting the default scheme
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt => {
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParams;
            });

            services.AddDefaultIdentity<AuthenticationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<APIDatabaseContext>();
            //.AddRoles<RoleManager<IdentityRole>>();

            // setup standart user rol
            services.AddIdentityCore<AuthenticationUser>()
                .AddRoles<IdentityRole>()
                .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<AuthenticationUser, IdentityRole>>()
                .AddEntityFrameworkStores<APIDatabaseContext>()
                .AddDefaultTokenProviders();


            // setup authentication services
            services.AddScoped<IAuthenticationService, AuthenticationManager>();
            services.AddSingleton(tokenValidationParams);

            return services;
        }
    }
}