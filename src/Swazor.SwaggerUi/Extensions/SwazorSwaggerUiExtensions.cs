namespace Swazor.SwaggerUi.Extensions;

using Swashbuckle.AspNetCore.SwaggerUI;

/// <summary>
/// Swagger UI configuration extensions for displaying Swazor-rendered descriptions
/// </summary>
public static class SwazorSwaggerUiExtensions
{
    // two targeted fixes for how Swagger UI renders Swazor descriptions:
    // 1. dark mode paragraph
    // 2. light mode code block
    private const string SWAZOR_STYLES =
        "<style>" +
        "html.dark-mode .swagger-ui .opblock-description-wrapper h4," +
        "html.dark-mode .swagger-ui .opblock-description-wrapper li{color:#e4e6e6}" +
        ".swagger-ui .opblock-description-wrapper code{font-size:.85em;padding:1px 5px}" +
        "</style>";

    /// <summary>
    /// Injects CSS into Swagger UI, polishing how Swazor HTML descriptions render
    /// </summary>
    /// <param name="options">The Swagger UI options to configure</param>
    /// <returns>The same <paramref name="options"/> instance</returns>
    public static SwaggerUIOptions UseSwazorStyles(this SwaggerUIOptions options)
    {
        options.HeadContent += SWAZOR_STYLES;
        return options;
    }
}