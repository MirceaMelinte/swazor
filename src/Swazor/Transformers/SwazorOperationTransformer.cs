namespace Swazor.Transformers;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swazor.Abstractions;
using Swazor.Infrastructure;
using Swazor.Rendering;

internal sealed class SwazorOperationTransformer(
    IDescriptionRenderer renderer,
    IOptions<SwazorOptions> options,
    ILogger<SwazorOperationTransformer> logger) : IOpenApiOperationTransformer
{
    public async Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var templateKey = TemplateKeyResolver.Resolve(operation, context, options.Value);

        if (templateKey is null)
        {
            return;
        }

        var routeValues = context.Description.ActionDescriptor.RouteValues;
        routeValues.TryGetValue("controller", out var controllerName);
        routeValues.TryGetValue("action", out var actionName);

        var descriptionContext = new OperationDescriptionContext
        {
            OperationId = operation.OperationId,
            HttpMethod = context.Description.HttpMethod ?? string.Empty,
            RelativePath = context.Description.RelativePath ?? string.Empty,
            ControllerName = controllerName,
            ActionName = actionName,
            ParameterNames = operation.Parameters?.Select(param => param.Name ?? string.Empty).ToList() ?? [],
            ResponseCodes = operation.Responses?.Keys.ToList() ?? [],
            Summary = operation.Summary,
            DocumentName = context.DocumentName
        };

        var (templateExists, html) = await renderer.RenderAsync(templateKey, descriptionContext, cancellationToken);

        if (!templateExists)
        {
            if (options.Value.WarnOnMissingTemplate)
            {
                logger.LogWarning("No Swazor template found for key '{TemplateKey}'", templateKey);
            }

            return;
        }

        if (html is null)
        {
            return;
        }

        operation.Description = html;
    }
}