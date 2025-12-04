using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IMHub.Infrastructure.Authentication;

namespace IMHub.API.Extensions
{
    public class AuthenticationServiceExtensions
    {
        public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Configure the Options (Mapping appsettings -> Class)
            services.Configure<JwtConfig>(configuration.GetSection(JwtConfig.SectionName));

            // 2. Get Settings manually for Key generation
            var jwtSettings = configuration.GetSection(JwtConfig.SectionName).Get<JwtConfig>();

            // Safety Check
            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret))
                throw new InvalidOperationException("JWT Secret is missing!");

            // 3. Add JWT Bearer
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
}
