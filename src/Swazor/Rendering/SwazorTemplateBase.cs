namespace Swazor.Rendering;

using System.Text;
using System.Text.Encodings.Web;

public abstract class SwazorTemplateBase
{
    private readonly StringBuilder output = new();

    public dynamic? Model { get; set; }

    public abstract Task ExecuteAsync();

    protected void WriteLiteral(string? literal)
    {
        if (literal is not null)
        {
            output.Append(literal);
        }
    }

    protected void Write(object? value)
    {
        if (value is not null)
        {
            output.Append(HtmlEncoder.Default.Encode(value.ToString() ?? string.Empty));
        }
    }

    internal string GetOutput() => output.ToString();
}