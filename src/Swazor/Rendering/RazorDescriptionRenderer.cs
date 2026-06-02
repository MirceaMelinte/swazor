namespace Swazor.Rendering;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swazor.Abstractions;
using Swazor.Infrastructure;

internal sealed class RazorDescriptionRenderer(
    IOptions<SwazorOptions> options,
    IWebHostEnvironment environment,
    ILogger<RazorDescriptionRenderer> logger) : IDescriptionRenderer
{
    private readonly SwazorRazorEngine engine = new(Path.Combine(environment.ContentRootPath, options.Value.DescriptionsPath));

    public async Task<string?> RenderAsync(string templateKey, OperationDescriptionContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!engine.TemplateExists(templateKey))
            {
                return null;
            }

            return await engine.RenderAsync(templateKey, context, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to render template '{TemplateKey}'", templateKey);
            return null;
        }
    }

    public bool TemplateExists(string templateKey) => engine.TemplateExists(templateKey);
}