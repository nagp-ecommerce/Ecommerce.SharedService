using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace SharedService.Lib.DI
{
    public static class JwtAuthenticationScheme
    {
        public static IServiceCollection AddJwtAuthenticationScheme(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                    var key = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("AUTH_SECRET", EnvironmentVariableTarget.User)
                            ?? config["Authentication:Key"]!)
                    );

                    var (issuer, audience) = (
                        Environment.GetEnvironmentVariable("AUTH_ISSUER", EnvironmentVariableTarget.User) ?? config["Authentication:Issuer"],
                        Environment.GetEnvironmentVariable("AUTH_AUDIENCE", EnvironmentVariableTarget.User) ?? config["Authentication:Audience"]
                    );

                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = key,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        RoleClaimType = ClaimTypes.Role,
                    };
                });
            return services;
        }
    }
}
