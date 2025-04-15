using AspNetCoreRateLimit;

namespace Northwind.WebApi.Service
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddCustomHttpLogging(this IServiceCollection service)
        {
            service.AddHttpLogging(options =>
            {
                options.RequestHeaders.Add("Origin");

                // Add the rate limiting headers so they will not be redacted.
                options.RequestHeaders.Add("X-Client-Id");
                options.ResponseHeaders.Add("Retry-After");

                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
            });

            return service;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: "Northwind.Mvc.Policy",
                policy =>
                {
                    policy.WithOrigins("https://localhost:5082");
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services, ConfigurationManager configuration)
        {
            // Add services to store rate limit counters and rules in memory.
            services.AddMemoryCache();
            services.AddInMemoryRateLimiting();

            // Load default rate limit options from appsettings.json.
            services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));

            // Load client-specific policies from appsettings.json.
            services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));

            // Register the configuration.
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            return services;
        }

        public static async Task UseCustomClientRateLimiting(this WebApplication app)
        {
            using (IServiceScope scope = app.Services.CreateScope())
            {
                IClientPolicyStore clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();

                await clientPolicyStore.SeedAsync();
            }

            app.UseClientRateLimiting();
        }
    }
}
