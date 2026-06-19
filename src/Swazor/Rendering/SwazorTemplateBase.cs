namespace Swazor.Rendering;

using System.Text;
using System.Text.Encodings.Web;

/// <summary>
/// Base class for compiled Swazor description templates.
/// </summary>
public abstract class SwazorTemplateBase
{
    private readonly StringBuilder output = new();

    /// <summary>The model passed to the template, or <c>null</c> when no model was supplied.</summary>
    public dynamic? Model { get; set; }

    /// <summary>Runs the compiled template body, writing its output. Invoked by the rendering engine.</summary>
    /// <returns>A task that completes when the template has finished executing.</returns>
    public abstract Task ExecuteAsync();

    /// <summary>Appends literal template markup to the output. Called by generated template code.</summary>
    /// <param name="literal">The literal text to append; ignored when <c>null</c></param>
    protected void WriteLiteral(string? literal)
    {
        if (literal is not null)
        {
            output.Append(literal);
        }
    }

    /// <summary>Appends an HTML-encoded value to the output. Called by generated template code for <c>@</c> expressions.</summary>
    /// <param name="value">The value to encode and append; ignored when <c>null</c></param>
    protected void Write(object? value)
    {
        if (value is not null)
        {
            output.Append(HtmlEncoder.Default.Encode(value.ToString() ?? string.Empty));
        }
    }

    internal string GetOutput() => output.ToString();
}