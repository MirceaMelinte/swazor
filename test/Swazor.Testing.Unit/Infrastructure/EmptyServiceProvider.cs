namespace Swazor.Testing.Unit.Infrastructure;

internal sealed class EmptyServiceProvider : IServiceProvider
{
    public object? GetService(Type serviceType) => null;
}