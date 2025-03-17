using Amazon;
using Amazon.Runtime;
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
            where TContext : DbContext
        {

            JwtAuthenticationScheme.AddJwtAuthenticationScheme(services, config);

            // setting up AWS SNS asynchronous communication
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? "Development";
            if (env == "Development")
            {
                var key = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
                var secret = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
                var region = Environment.GetEnvironmentVariable("AWS_REGION");
                var cred = new BasicAWSCredentials(key, secret);

                if (cred is not null && region is not null)
                {
                    services.AddSingleton<IAmazonSimpleNotificationService>(service =>
                        new AmazonSimpleNotificationServiceClient(
                            cred,
                            RegionEndpoint.GetBySystemName(region)
                        )
                    );
                }
            }
            else
            {
                services.AddDefaultAWSOptions(config.GetAWSOptions("AWS"));
                services.AddAWSService<IAmazonSimpleNotificationService>();
            }

            services.AddSingleton<IPublisherService, PublisherService>();

            services.AddLogging(logger =>
            {
                logger.ClearProviders();
                logger.AddConsole();
                logger.AddDebug();
            });

            // Adding Generic DbContext from microservice
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION") 
                ?? config.GetConnectionString("defaultConnection");
            services.AddDbContext<TContext>(option =>
                option.UseNpgsql(connectionString)
                    .LogTo(Console.WriteLine, LogLevel.Information)
                        .EnableSensitiveDataLogging()
                );

            // Adding logging
            services.AddLogging(options =>
            {
                options.ClearProviders();
                options.AddConsole();
                options.SetMinimumLevel(LogLevel.Debug);
            });

            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalException>();

            // block all outsider calls
            // All apis will be localhost:5001/api/products/
            app.UseMiddleware<RedirectToApiGateway>();

            return app;
        }
    }
}
