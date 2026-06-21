namespace Swazor.Testing.Unit.Infrastructure;

using Swazor.Abstractions;

internal sealed class StubViewRenderer : IRazorViewRenderer
{
    private readonly Func<string, string?> handler;

    public StubViewRenderer(string? result) => handler = _ => result;

    public StubViewRenderer(Func<string, string?> handler) => this.handler = handler;

    public string? LastViewPath { get; private set; }

    public Task<string?> RenderToStringAsync(string viewPath, object? model, CancellationToken cancellationToken = default)
    {
        LastViewPath = viewPath;
        return Task.FromResult(handler(viewPath));
    }
}