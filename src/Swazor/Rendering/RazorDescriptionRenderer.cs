namespace Swazor.Rendering;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swazor.Abstractions;
using Swazor.Infrastructure;

internal sealed class RazorDescriptionRenderer(
    IRazorViewRenderer renderer,
    IOptions<SwazorOptions> options,
    ILogger<RazorDescriptionRenderer> logger) : IDescriptionRenderer
{
    public async Task<(bool TemplateExists, string? Html)> RenderAsync(
        string templateKey,
        OperationDescriptionContext context,
        CancellationToken cancellationToken = default)
    {
        var viewPath = ResolveViewPath(templateKey);

        try
        {
            var html = await renderer.RenderToStringAsync(viewPath, context, cancellationToken);

            // a null result means there is no view for this key
            // an exception means the view exists but failed to render, handled below
            return html is null ? (false, null) : (true, html);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to render template '{TemplateKey}'", templateKey);
            return (true, null);
        }
    }

    private string ResolveViewPath(string templateKey)
    {
        var root = options.Value.DescriptionsPath.Replace('\\', '/').Trim('/');
        return $"/{root}/{templateKey}.cshtml";
    }
}