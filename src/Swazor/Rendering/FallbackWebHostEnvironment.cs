namespace Swazor.Rendering;

using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

// MVC Razor view engine resolves IWebHostEnvironment during setup
// web hosts already register one, so the fallback only takes effect in non-web hosts (a console app or unit test project)
internal sealed class FallbackWebHostEnvironment : IWebHostEnvironment
{
    public FallbackWebHostEnvironment()
    {
        var fileProvider = new PhysicalFileProvider(AppContext.BaseDirectory);

        ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? nameof(Swazor);
        EnvironmentName = Environments.Production;
        ContentRootPath = AppContext.BaseDirectory;
        ContentRootFileProvider = fileProvider;
        WebRootPath = AppContext.BaseDirectory;
        WebRootFileProvider = fileProvider;
    }

    public string ApplicationName { get; set; }

    public string EnvironmentName { get; set; }

    public string ContentRootPath { get; set; }

    public IFileProvider ContentRootFileProvider { get; set; }

    public string WebRootPath { get; set; }

    public IFileProvider WebRootFileProvider { get; set; }
}