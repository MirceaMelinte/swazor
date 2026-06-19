namespace Swazor.Extensions;

using Microsoft.AspNetCore.OpenApi;
using Swazor.Transformers;

/// <summary>
/// OpenAPI configuration extensions for registering Swazor description transformer.
/// </summary>
public static class OpenApiOptionsExtensions
{
    /// <summary>
    /// Adds the operation transformer for replacing operation descriptions with Swazor rendered HTML.
    /// </summary>
    /// <param name="options">The OpenAPI options to configure.</param>
    /// <returns>The same <paramref name="options"/> instance.</returns>
    public static OpenApiOptions AddSwazorDescriptions(this OpenApiOptions options)
    {
        options.AddOperationTransformer<SwazorOperationTransformer>();
        return options;
    }
}