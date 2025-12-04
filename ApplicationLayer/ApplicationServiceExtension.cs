using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MediatR;
using IMHub.ApplicationLayer.Common.Behaviors;
using AM = AutoMapper;

namespace IMHub.ApplicationLayer;
   public static class ApplicationServiceExtension 
   {
       public static IServiceCollection AddApplicationServices(this IServiceCollection services)
       {
        // 1. AUTOMAPPER MANUAL SETUP (No Deprecated Packages)
        // =========================================================

        // We use the alias 'AM' to call MapperConfiguration
        var mapperConfig = new AM.MapperConfiguration(cfg =>
        {
            // Explicitly verify Assembly usage
            cfg.AddMaps(Assembly.GetExecutingAssembly());
        });

        // Create the mapper from the config
        AM.IMapper mapper = mapperConfig.CreateMapper();

        // Register as Singleton
        services.AddSingleton(mapper);

        // 2. MediatR Setup (CQRS Pattern)
        services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            // 3. FluentValidation Setup (Validation Rules)
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // 4. Register Validation Pipeline Behavior
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
       }
   }
