namespace Swazor.Infrastructure;

/// <summary>
/// Determines how a template file name is derived from the controller and action of an operation
/// </summary>
public enum TemplateNamingConvention
{
    /// <summary>Templates are named <c>{Controller}_{Action}.cshtml</c> in the root descriptions directory</summary>
    Underscore,

    /// <summary>Templates are organized as <c>{Controller}/{Action}.cshtml</c> in subdirectories</summary>
    Subdirectory
}