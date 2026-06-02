namespace Swazor.Rendering;

public sealed class OperationDescriptionContext
{
    public string? OperationId { get; init; }

    public required string HttpMethod { get; init; }

    public required string RelativePath { get; init; }

    public string? ControllerName { get; init; }

    public string? ActionName { get; init; }

    public IReadOnlyList<string> ParameterNames { get; init; } = [];

    public IReadOnlyList<string> ResponseCodes { get; init; } = [];

    public string? Summary { get; init; }

    public required string DocumentName { get; init; }
}