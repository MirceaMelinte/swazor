namespace Swazor.Rendering;

/// <summary>
/// The model passed to a description template that declares <c>@model Swazor.Rendering.OperationDescriptionContext</c>.
/// Carries metadata about the OpenAPI operation the description is being rendered for.
/// </summary>
public sealed class OperationDescriptionContext
{
    /// <summary>The operation ID from the OpenAPI spec, if one is set.</summary>
    public string? OperationId { get; init; }

    /// <summary>The HTTP method of the operation, for example <c>GET</c> or <c>POST</c>.</summary>
    public required string HttpMethod { get; init; }

    /// <summary>The route template of the operation, for example <c>/products/{id}</c>.</summary>
    public required string RelativePath { get; init; }

    /// <summary>The controller name, if the operation comes from an MVC controller.</summary>
    public string? ControllerName { get; init; }

    /// <summary>The action method name, if the operation comes from an MVC controller.</summary>
    public string? ActionName { get; init; }

    /// <summary>The names of all parameters on the operation</summary>
    public IReadOnlyList<string> ParameterNames { get; init; } = [];

    /// <summary>The declared response status codes for the operation.</summary>
    public IReadOnlyList<string> ResponseCodes { get; init; } = [];

    /// <summary>The existing operation summary from XML docs or attributes, if any.</summary>
    public string? Summary { get; init; }

    /// <summary>The name of the OpenAPI document the operation belongs to.</summary>
    public required string DocumentName { get; init; }
}