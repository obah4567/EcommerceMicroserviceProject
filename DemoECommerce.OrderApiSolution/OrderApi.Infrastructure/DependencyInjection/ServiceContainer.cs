using ECommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;

namespace OrderApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructreService(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Database Connectivity
            // Add authentication scheme
            SharedServiceContainer.AddSharedServices<OrderDbContext>(services, configuration, configuration["MySerilog:Filename"]!);

            // Create Dependency Injection
            services.AddScoped<IOrder, OrderRepository>();

            return services;
        }

        public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Register middleware sush as:
            // Global Exception => handle external errors
            // ListenToApiGateway Only -> block all outsiders calls
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
