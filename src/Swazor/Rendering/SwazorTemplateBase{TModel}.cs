namespace Swazor.Rendering;

/// <summary>
/// Base class for compiled Swazor description templates that declare a strongly-typed <c>@model</c>.
/// </summary>
/// <typeparam name="TModel">The model type declared by the template.</typeparam>
public abstract class SwazorTemplateBase<TModel> : SwazorTemplateBase
{
    /// <summary>The strongly-typed model passed to the template.</summary>
    public new TModel? Model
    {
        get => (TModel?)base.Model;
        set => base.Model = value;
    }
}