namespace Swazor.Abstractions;

internal interface IRazorViewRenderer
{
    Task<string?> RenderToStringAsync(string viewPath, object? model, CancellationToken cancellationToken = default);
}