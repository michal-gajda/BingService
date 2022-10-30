namespace BingService.Infrastructure.Bing;

using System.Diagnostics.CodeAnalysis;
using BingService.Infrastructure.Bing.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Refit;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddBing(this IServiceCollection services)
    {
        services.AddRefitClient<IBingService>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://www.bing.com"));

        return services;
    }
}
