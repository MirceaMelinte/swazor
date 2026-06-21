namespace Swazor.Rendering;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Swazor.Abstractions;

internal sealed class RazorViewRenderer(IServiceProvider services) : IRazorViewRenderer
{
    public async Task<string?> RenderToStringAsync(string viewPath, object? model, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await using var scope = services.CreateAsyncScope();
        var scopedServices = scope.ServiceProvider;

        var viewEngine = scopedServices.GetRequiredService<IRazorViewEngine>();
        var viewResult = viewEngine.GetView(executingFilePath: null, viewPath, isMainPage: true);

        if (!viewResult.Success)
        {
            return null;
        }

        var httpContext = new DefaultHttpContext { RequestServices = scopedServices };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        var viewData = new ViewDataDictionary(
            scopedServices.GetRequiredService<IModelMetadataProvider>(),
            new ModelStateDictionary())
        {
            Model = model
        };

        await using var output = new StringWriter();

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewData,
            new TempDataDictionary(httpContext, scopedServices.GetRequiredService<ITempDataProvider>()),
            output,
            new HtmlHelperOptions());

        await viewResult.View.RenderAsync(viewContext);

        return output.ToString();
    }
}