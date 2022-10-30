namespace BingService.Infrastructure;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BingService.Infrastructure.Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddHangfire(configuration);

        return services;
    }

    public static void UseInfrastructure(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseHangfire(configuration);
    }
}
