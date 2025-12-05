using IMHub.API.Extensions;
using IMHub.API.Middleware;
using IMHub.ApplicationLayer;
using IMHub.Infrastructure;
using IMHub.Infrastructure.Data.DbInitializers_Seeds;

namespace IMHub.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration); // Includes IFileStorageService registration
            builder.Services.AddOpenApi();
            builder.Services.AddApiAuthentication(builder.Configuration);
            
            // Add HttpContextAccessor for CurrentUserService
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IMHub.ApplicationLayer.Common.Interfaces.ICurrentUserService, IMHub.API.Services.CurrentUserService>();
            
            // Add HttpClientFactory for external API calls
            builder.Services.AddHttpClient();
            
            // File Storage Service is registered in AddInfrastructureServices() above
            // builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>(); // Already registered

            // Configure CORS (must be before Build())
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            // Database initialization
            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    logger.LogInformation("Starting database initialization...");
                    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
                    await initializer.RunAsync();
                    logger.LogInformation("Database initialization completed successfully");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while initializing the database");
                    logger.LogError("Application will continue, but database may not be properly initialized");
                    logger.LogError("Please check database connection and restart the application");
                    // Note: In production, you might want to fail fast here
                    // throw; // Uncomment to fail fast on database initialization errors
                }
            }

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            // Middleware order is critical
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles(); // Serve static files from wwwroot
            app.UseCors("AllowReactApp");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            await app.RunAsync();
        }
    }
}
