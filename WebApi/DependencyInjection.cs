using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Domain.Interfaces;
using Infrastructure.Redis;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using WebApi.Services;

namespace WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebApiService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Document API", Version = "v1" });

                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
               {
                   builder.WithOrigins(
                   "http://localhost:3000",
                   "https://localhost:3000",
                   "http://192.168.1.22:3000",
                   "https://192.168.1.22:3000"
                   )
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });

            // Add Health Checks
            services.AddHealthChecks()
                .AddCheck<RedisHealthCheck>("redis", HealthStatus.Unhealthy, 
                    tags: new[] { "ready", "redis" });

            services.AddScoped<IUser, GetCurrentUser>();
            services.AddHttpContextAccessor();

            // We'll add a custom JWT authentication middleware since the package isn't properly installed

            return services;
        }
    }
}
