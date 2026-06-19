namespace Swazor.Abstractions;

using Swazor.Rendering;

internal interface IDescriptionRenderer
{
    // returns null if the template does not exist or rendering fails
    Task<string?> RenderAsync(string templateKey, OperationDescriptionContext context, CancellationToken cancellationToken = default);

    bool TemplateExists(string templateKey);
}