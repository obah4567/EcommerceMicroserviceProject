using ECommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProduitApi.Application.Interfaces;
using ProduitApi.Infrastructure.Data;
using ProduitApi.Infrastructure.Repositories;

namespace ProduitApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            // Add database connectivity 
            // Add authentication scheme
            SharedServiceContainer.AddSharedServices<ProductDbContext>(services, configuration, configuration["MySerilog:FileName"]!);

            // Create Dependency Injection (DI)
            services.AddScoped<IProduct, ProductRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Register middleware such as:
            // Global Exception: handles external errors.
            // Listen to Only Api Gateway: blocks all outsider calls;
            SharedServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}
