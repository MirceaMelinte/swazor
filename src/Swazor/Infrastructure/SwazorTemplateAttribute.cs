namespace Swazor.Infrastructure;

// overrides the convention-based template key on controller action methods
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class SwazorTemplateAttribute(string templateKey) : Attribute
{
    public string TemplateKey { get; } = templateKey;
}