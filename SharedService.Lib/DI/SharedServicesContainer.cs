using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedService.Lib.Interfaces;
using SharedService.Lib.Middleware;
using SharedService.Lib.PubSub;

namespace SharedService.Lib.DI
{
    public static class SharedServicesContainer
    {
        public static IServiceCollection AddSharedServices<TContext>
            (this IServiceCollection services, IConfiguration config)
            where TContext: DbContext
        {

            JwtAuthenticationScheme.AddJwtAuthenticationScheme(services, config);

            // setting up AWS SNS asynchronous communication
            services.AddDefaultAWSOptions(config.GetAWSOptions());
            services.AddAWSService<IAmazonSimpleNotificationService>();
            services.AddScoped<PublisherService>();

            services.AddLogging(logger => {
                logger.ClearProviders();
                logger.AddConsole();
                logger.AddDebug();
            });

            // Adding Generic DbContext from microservice
            services.AddDbContext<TContext>(option => 
                option.UseNpgsql(config.GetConnectionString("defaultConnection"))
                    .LogTo(Console.WriteLine, LogLevel.Information)
                        .EnableSensitiveDataLogging()
                );
            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalException>();

            // block all outsider calls
            app.UseMiddleware<RedirectToApiGateway>();

            return app;
        }
    }
}
