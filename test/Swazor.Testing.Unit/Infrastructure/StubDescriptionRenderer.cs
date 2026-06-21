namespace Swazor.Testing.Unit.Infrastructure;

using Swazor.Abstractions;
using Swazor.Rendering;

internal sealed class StubDescriptionRenderer(Dictionary<string, string> templates) : IDescriptionRenderer
{
    public Task<(bool TemplateExists, string? Html)> RenderAsync(
        string templateKey,
        OperationDescriptionContext context,
        CancellationToken cancellationToken = default)
        => Task.FromResult(templates.TryGetValue(templateKey, out var html) ? (true, html) : (false, (string?)null));
}