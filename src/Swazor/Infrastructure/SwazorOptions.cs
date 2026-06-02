namespace Swazor.Infrastructure;

public sealed class SwazorOptions
{
    // resolved relative to IWebHostEnvironment.ContentRootPath
    public string DescriptionsPath { get; init; } = "Descriptions";

    public TemplateNamingConvention NamingConvention { get; init; } = TemplateNamingConvention.Underscore;

    public bool WarnOnMissingTemplate { get; init; }
}