namespace Swazor.Infrastructure;

/// <summary>
/// Configures how Swazor locates and renders description templates.
/// </summary>
public sealed class SwazorOptions
{
    /// <summary>
    /// Directory containing the <c>.cshtml</c> description templates,
    /// resolved relative to <c>IWebHostEnvironment.ContentRootPath</c>.
    /// Defaults to <c>Descriptions</c>.
    /// </summary>
    public string DescriptionsPath { get; set; } = "Descriptions";

    /// <summary>
    /// How a template file is located for an operation: <see cref="TemplateNamingConvention.Underscore"/>
    /// looks for <c>{Controller}_{Action}.cshtml</c> in <see cref="DescriptionsPath"/>,
    /// while <see cref="TemplateNamingConvention.Subdirectory"/> looks for <c>{Controller}/{Action}.cshtml</c>.
    /// </summary>
    public TemplateNamingConvention NamingConvention { get; set; } = TemplateNamingConvention.Underscore;

    /// <summary>
    /// When <c>true</c>, logs a warning for any operation that resolves to a template key with no matching file.
    /// When <c>false</c>, the operation is simply left without a description.
    /// Off by default.
    /// </summary>
    public bool WarnOnMissingTemplate { get; set; }

    /// <summary>
    /// When <c>true</c>, the missing-template check (see <see cref="WarnOnMissingTemplate"/>) runs once at startup,
    /// instead of only when the OpenAPI document is first requested.
    /// This surfaces missing templates in the logs at boot - useful for catching them in CI or local startup - at the
    /// cost of forcing OpenAPI document generation eagerly during startup.
    /// This builds the document and renders every matching template once, adding to startup time.
    /// Has no effect unless <see cref="WarnOnMissingTemplate"/> is also <c>true</c>.
    /// Off by default.
    /// </summary>
    public bool ValidateTemplatesOnStartup { get; set; }
}