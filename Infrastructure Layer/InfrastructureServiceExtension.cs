using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.Infrastruture;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Infrastructure.Data;
using IMHub.Infrastructure.Data.Interceptors;
using IMHub.Infrastructure.Repositories;
using IMHub.Infrastructure.Services;
using IMHub.Infrastructure.Authentication;
using IMHub.Infrastructure.Data.DbInitializers_Seeds;
using IMHub.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IMHub.Infrastructure
{
    public static class InfrastructureServiceExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Register the Interceptor as a Scoped Service
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

            // 2. Add DbContext and attach the Interceptor
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                // Retrieve the interceptor
                var interceptor = sp.GetRequiredService<ISaveChangesInterceptor>();

                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                       .AddInterceptors(interceptor); // <--- ATTACH IT HERE
            });

            services.AddScoped<DbInitializer>();
            services.AddScoped<ICustomSeeder, RoleSeeder>();
            services.AddScoped<ICustomSeeder, SuperAdminSeeder>();

            services.AddTransient<IEmailService, SendGridEmailService>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            // Register Repositories (also registered via UnitOfWork, but can be used independently if needed)
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPlatformAdminRepository, PlatformAdminRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IPrinterRepository, PrinterRepository>();
            services.AddScoped<IDistributionRepository, DistributionRepository>();
            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<ITemplateVersionRepository, TemplateVersionRepository>();
            services.AddScoped<ITemplatePageRepository, TemplatePageRepository>();
            services.AddScoped<ITemplateFieldRepository, TemplateFieldRepository>();
            services.AddScoped<ICsvUploadRepository, CsvUploadRepository>();
            services.AddScoped<IContentRepository, ContentRepository>();
            services.AddScoped<IContentFieldValueRepository, ContentFieldValueRepository>();
            services.AddScoped<ISendoutRepository, SendoutRepository>();
            services.AddScoped<ISendoutStatusHistoryRepository, SendoutStatusHistoryRepository>();

            return services;
        }
    }
}

