namespace Swazor.Rendering;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Swazor.Infrastructure;

internal static class TemplateKeyResolver
{
    public static string? Resolve(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        SwazorOptions options)
    {
        var actionDescriptor = context.Description.ActionDescriptor;

        var attribute = actionDescriptor.EndpointMetadata?.OfType<SwazorTemplateAttribute>().FirstOrDefault();

        if (attribute is not null)
        {
            return attribute.TemplateKey;
        }

        if (!string.IsNullOrEmpty(operation.OperationId))
        {
            return operation.OperationId;
        }

        var routeValues = actionDescriptor.RouteValues;

        if (routeValues.TryGetValue("controller", out var controller) && !string.IsNullOrEmpty(controller)
            && routeValues.TryGetValue("action", out var action) && !string.IsNullOrEmpty(action))
        {
            return options.NamingConvention switch
            {
                TemplateNamingConvention.Subdirectory => $"{controller}/{action}",
                _ => $"{controller}_{action}"
            };
        }

        return null;
    }
}