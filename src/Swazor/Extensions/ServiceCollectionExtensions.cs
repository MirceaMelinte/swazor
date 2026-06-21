namespace Swazor.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Swazor.Abstractions;
using Swazor.Infrastructure;
using Swazor.Rendering;

/// <summary>
/// Service registration extensions for Swazor
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Swazor template rendering services and options + the hosted service that can validate templates at startup.
    /// </summary>
    /// <param name="services">The service collection for configuring Swazor.</param>
    /// <param name="configure">An optional callback for configuring <see cref="SwazorOptions"/>.</param>
    /// <returns>The same <paramref name="services"/> instance.</returns>
    public static IServiceCollection AddSwazor(this IServiceCollection services, Action<SwazorOptions>? configure = null)
    {
        if (configure is not null)
        {
            services.Configure(configure);
        }
        else
        {
            services.AddOptions<SwazorOptions>();
        }

        services.AddRazorViewRendering();
        services.AddSingleton<IDescriptionRenderer, RazorDescriptionRenderer>();
        services.AddHostedService<SwazorStartupValidator>();

        return services;
    }
}