namespace Swazor.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Swazor.Abstractions;
using Swazor.Infrastructure;
using Swazor.Rendering;

public static class ServiceCollectionExtensions
{
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

        services.AddSingleton<IDescriptionRenderer, RazorDescriptionRenderer>();

        return services;
    }
}