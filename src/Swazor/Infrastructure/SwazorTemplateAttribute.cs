namespace Swazor.Infrastructure;

/// <summary>
/// Overrides the convention-based template key for a controller action, pointing it at a specific template.
/// </summary>
/// <param name="templateKey">The template key to use for the decorated action, i.e. <c>Products_Details</c></param>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class SwazorTemplateAttribute(string templateKey) : Attribute
{
    /// <summary>The template key to use for the decorated action</summary>
    public string TemplateKey { get; } = templateKey;
}