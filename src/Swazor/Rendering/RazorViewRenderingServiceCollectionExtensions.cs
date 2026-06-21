namespace Swazor.Rendering;

using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Swazor.Abstractions;

internal static class RazorViewRenderingServiceCollectionExtensions
{
    private const string DIAGNOSTIC_SOURCE_NAME = "Swazor";

    // minimal MVC Razor view engine needed to render precompiled views to strings
    // registers the compiled views of every loaded assembly as application parts so they are resolvable by path
    internal static IServiceCollection AddRazorViewRendering(this IServiceCollection services)
    {
        services.TryAddSingleton<IWebHostEnvironment>(new FallbackWebHostEnvironment());
        services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

        var diagnosticListener = new DiagnosticListener(DIAGNOSTIC_SOURCE_NAME);
        services.TryAddSingleton<DiagnosticSource>(diagnosticListener);
        services.TryAddSingleton(diagnosticListener);

        services.AddLogging();
        services.AddHttpContextAccessor();

        services
            .AddMvcCore()
            .AddRazorViewEngine()
            .ConfigureApplicationPartManager(CompiledViewApplicationPartLoader.AddCompiledViewParts);

        services.TryAddSingleton<IRazorViewRenderer, RazorViewRenderer>();

        return services;
    }
}