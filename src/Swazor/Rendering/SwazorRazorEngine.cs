namespace Swazor.Rendering;

using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

internal sealed class SwazorRazorEngine
{
    private readonly string templatesRootPath;
    private readonly HashSet<string> templateKeys;
    private readonly RazorProjectEngine razorEngine;
    private readonly ConcurrentDictionary<string, Lazy<Task<Type>>> typeCache = new();

    private static readonly Regex ModelDirectivePattern = new(
        @"^\s*@model\s+(.+)$",
        RegexOptions.Multiline | RegexOptions.Compiled);

    internal SwazorRazorEngine(string templatesRootPath)
    {
        this.templatesRootPath = templatesRootPath;
        templateKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (Directory.Exists(templatesRootPath))
        {
            foreach (var file in Directory.GetFiles(templatesRootPath, "*.cshtml", SearchOption.AllDirectories))
            {
                var key = Path.GetRelativePath(templatesRootPath, file);
                key = Path.ChangeExtension(key, null)!.Replace(Path.DirectorySeparatorChar, '/');
                templateKeys.Add(key);
            }
        }

        var fileSystem = RazorProjectFileSystem.Create(templatesRootPath);
        razorEngine = RazorProjectEngine.Create(RazorConfiguration.Default, fileSystem, builder =>
        {
            builder.SetRootNamespace("Swazor.Generated");

            builder.AddDefaultImports(
                "@using System",
                "@using System.Threading.Tasks",
                "@using Swazor.Rendering");
        });
    }

    internal bool TemplateExists(string templateKey) => templateKeys.Contains(templateKey);

    internal async Task<string> RenderAsync(string templateKey, object? model, CancellationToken cancellationToken = default)
    {
        var type = await typeCache.GetOrAdd(
            templateKey,
            key => new Lazy<Task<Type>>(() => CompileTemplateAsync(key))).Value;

        var instance = (SwazorTemplateBase)Activator.CreateInstance(type)!;
        instance.Model = model;

        await instance.ExecuteAsync();
        return instance.GetOutput();
    }

    private async Task<Type> CompileTemplateAsync(string templateKey)
    {
        var filePath = Path.Combine(
            templatesRootPath,
            templateKey.Replace('/', Path.DirectorySeparatorChar) + ".cshtml");

        var content = await File.ReadAllTextAsync(filePath);
        content = PreprocessModelDirective(content);

        var csharpCode = GenerateCode(templateKey, content);
        return CompileAndLoad(templateKey, csharpCode);
    }

    private static string PreprocessModelDirective(string content)
    {
        var match = ModelDirectivePattern.Match(content);

        if (match.Success)
        {
            var typeName = match.Groups[1].Value.Trim();
            return content.Replace(match.Value, $"@inherits Swazor.Rendering.SwazorTemplateBase<{typeName}>");
        }

        return $"@inherits Swazor.Rendering.SwazorTemplateBase\n{content}";
    }

    private string GenerateCode(string templateKey, string preprocessedContent)
    {
        var fileName = templateKey.Replace('/', '_') + ".cshtml";
        var source = RazorSourceDocument.Create(preprocessedContent, fileName);
        var codeDocument = razorEngine.Process(source, null, null, null);
        var csharpDocument = codeDocument.GetCSharpDocument();

        if (csharpDocument.Diagnostics.Count > 0)
        {
            var errors = csharpDocument
                .Diagnostics
                .Where(d => d.Severity == RazorDiagnosticSeverity.Error)
                .Select(d => d.GetMessage());

            var errorList = errors.ToList();

            if (errorList.Count > 0)
            {
                throw new InvalidOperationException($"Razor parsing failed for '{templateKey}':\n{string.Join('\n', errorList)}");
            }
        }

        return csharpDocument.GeneratedCode;
    }

    private static readonly Assembly CSharpRuntimeAssembly = typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly;

    private static Type CompileAndLoad(string templateKey, string csharpCode)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(csharpCode);

        var references = AppDomain
            .CurrentDomain
            .GetAssemblies()
            .Append(CSharpRuntimeAssembly)
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .DistinctBy(a => a.Location)
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToList();

        var compilation = CSharpCompilation.Create(
            $"SwazorTemplate_{templateKey.Replace('/', '_')}",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);

        if (!result.Success)
        {
            var errors = result
                .Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => d.ToString());

            throw new InvalidOperationException($"Template '{templateKey}' compilation failed:\n{string.Join('\n', errors)}");
        }

        stream.Seek(0, SeekOrigin.Begin);
        var assembly = AssemblyLoadContext.Default.LoadFromStream(stream);

        return assembly.GetTypes()
            .First(t => typeof(SwazorTemplateBase).IsAssignableFrom(t) && !t.IsAbstract);
    }
}