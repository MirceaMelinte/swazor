namespace Swazor.Infrastructure;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

internal sealed class SwazorStartupValidator(
    IOptions<SwazorOptions> options,
    IServiceProvider services) : IHostedService
{
    // AddOpenApi default document name; IOpenApiDocumentProvider is registered AND keyed by it
    private const string DEFAULT_DOCUMENT_NAME = "v1";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var value = options.Value;

        if (!value.ValidateTemplatesOnStartup || !value.WarnOnMissingTemplate)
        {
            return;
        }

        var documentProvider = services.GetKeyedService<IOpenApiDocumentProvider>(DEFAULT_DOCUMENT_NAME);

        if (documentProvider is null)
        {
            return;
        }

        await documentProvider.GetOpenApiDocumentAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}