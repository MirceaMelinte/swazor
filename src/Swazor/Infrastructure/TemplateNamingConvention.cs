namespace Swazor.Infrastructure;

public enum TemplateNamingConvention
{
    // {Controller}_{Action}.cshtml in root descriptions directory
    Underscore,

    // {Controller}/{Action}.cshtml in subdirectories
    Subdirectory
}