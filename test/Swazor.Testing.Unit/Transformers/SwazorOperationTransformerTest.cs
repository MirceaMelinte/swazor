namespace Swazor.Testing.Unit.Transformers;

using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swazor.Abstractions;
using Swazor.Infrastructure;
using Swazor.Rendering;
using Swazor.Transformers;

public class SwazorOperationTransformerTest
{
    private static SwazorOperationTransformer CreateTransformer(IDescriptionRenderer renderer, SwazorOptions? options = null)
        => new(renderer, Options.Create(options ?? new SwazorOptions()), NullLogger<SwazorOperationTransformer>.Instance);

    private static OpenApiOperationTransformerContext CreateContext(string? controller = null, string? action = null)
    {
        var routeValues = new Dictionary<string, string?>();

        if (controller is not null)
        {
            routeValues["controller"] = controller;
        }

        if (action is not null)
        {
            routeValues["action"] = action;
        }

        var actionDescriptor = new ActionDescriptor
        {
            RouteValues = routeValues,
            EndpointMetadata = []
        };

        return new OpenApiOperationTransformerContext
        {
            DocumentName = "v1",
            Description = new ApiDescription
            {
                ActionDescriptor = actionDescriptor,
                HttpMethod = "GET",
                RelativePath = "api/products"
            },
            ApplicationServices = new EmptyServiceProvider()
        };
    }

    [TestClass]
    public class TransformAsync : SwazorOperationTransformerTest
    {
        [TestMethod, TestCategory("SwazorOperationTransformer"), TestCategory("TransformAsync")]
        public async Task SetsDescriptionWhenTemplateExists()
        {
            // Arrange
            var renderer = new StubRenderer(new Dictionary<string, string> { ["Products_GetById"] = "<p>rendered</p>" });

            var transformer = CreateTransformer(renderer);
            var operation = new OpenApiOperation();
            var context = CreateContext(controller: "Products", action: "GetById");

            // Act
            await transformer.TransformAsync(operation, context, CancellationToken.None);

            // Assert
            Assert.AreEqual("<p>rendered</p>", operation.Description);
        }

        [TestMethod, TestCategory("SwazorOperationTransformer"), TestCategory("TransformAsync")]
        public async Task IsNoOpWhenNoTemplateFound()
        {
            // Arrange
            var renderer = new StubRenderer(new Dictionary<string, string>());

            var transformer = CreateTransformer(renderer);
            var operation = new OpenApiOperation();
            var context = CreateContext(controller: "Products", action: "GetById");

            // Act
            await transformer.TransformAsync(operation, context, CancellationToken.None);

            // Assert
            Assert.IsNull(operation.Description);
        }

        [TestMethod, TestCategory("SwazorOperationTransformer"), TestCategory("TransformAsync")]
        public async Task PreservesExistingDescriptionWhenNoTemplateMatches()
        {
            // Arrange
            var renderer = new StubRenderer(new Dictionary<string, string>());

            var transformer = CreateTransformer(renderer);
            var operation = new OpenApiOperation { Description = "original" };
            var context = CreateContext();

            // Act
            await transformer.TransformAsync(operation, context, CancellationToken.None);

            // Assert
            Assert.AreEqual("original", operation.Description);
        }

        [TestMethod, TestCategory("SwazorOperationTransformer"), TestCategory("TransformAsync")]
        public async Task DoesNotThrowWhenRendererReturnsNull()
        {
            // Arrange
            var renderer = new StubRenderer(new Dictionary<string, string> { ["Products_GetById"] = null! });

            var transformer = CreateTransformer(renderer);

            var operation = new OpenApiOperation { Description = "original" };
            var context = CreateContext(controller: "Products", action: "GetById");

            // Act
            await transformer.TransformAsync(operation, context, CancellationToken.None);

            // Assert
            Assert.AreEqual("original", operation.Description);
        }
    }

    private sealed class StubRenderer(Dictionary<string, string> templates) : IDescriptionRenderer
    {
        public bool TemplateExists(string templateKey) => templates.ContainsKey(templateKey);

        public Task<string?> RenderAsync(string templateKey, OperationDescriptionContext context, CancellationToken cancellationToken = default)
            => Task.FromResult(templates.GetValueOrDefault(templateKey));
    }

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }
}