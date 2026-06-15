namespace Swazor.Extensions;

using Microsoft.AspNetCore.OpenApi;
using Swazor.Transformers;

public static class OpenApiOptionsExtensions
{
    public static OpenApiOptions AddSwazorDescriptions(this OpenApiOptions options)
    {
        options.AddOperationTransformer<SwazorOperationTransformer>();
        return options;
    }
}