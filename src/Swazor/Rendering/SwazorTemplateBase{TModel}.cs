namespace Swazor.Rendering;

public abstract class SwazorTemplateBase<TModel> : SwazorTemplateBase
{
    public new TModel? Model
    {
        get => (TModel?)base.Model;
        set => base.Model = value;
    }
}