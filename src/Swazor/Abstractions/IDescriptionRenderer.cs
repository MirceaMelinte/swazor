namespace Swazor.Abstractions;

using Swazor.Rendering;

internal interface IDescriptionRenderer
{
    Task<(bool TemplateExists, string? Html)> RenderAsync(
        string templateKey,
        OperationDescriptionContext context,
        CancellationToken cancellationToken = default);
}