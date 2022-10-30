namespace BingService.Infrastructure.Hangfire;

using System.Diagnostics.CodeAnalysis;
using global::Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
internal static class DependencyInjection
{
    public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HangfireConnection");

        services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(connectionString);
        });

        services.AddHangfireServer(options =>
        {
            options.ServerName = nameof(ServiceConstants.ServiceName);
            options.WorkerCount = 1;
        });

        return services;
    }

    public static void UseHangfire(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseHangfireDashboard();
    }
}
