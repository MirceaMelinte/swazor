namespace Swazor.Testing.Unit.Rendering;

using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swazor.Infrastructure;
using Swazor.Rendering;

public class TemplateKeyResolverTest
{
    private static OpenApiOperationTransformerContext CreateContext(
        IDictionary<string, string?>? routeValues = null,
        IList<object>? endpointMetadata = null)
    {
        var actionDescriptor = new ActionDescriptor
        {
            RouteValues = routeValues ?? new Dictionary<string, string?>(),
            EndpointMetadata = endpointMetadata ?? []
        };

        return new OpenApiOperationTransformerContext
        {
            DocumentName = "v1",
            Description = new ApiDescription { ActionDescriptor = actionDescriptor },
            ApplicationServices = new EmptyServiceProvider()
        };
    }

    private static OpenApiOperation CreateOperation(string? operationId = null) => new() { OperationId = operationId };

    [TestClass]
    public class Resolve : TemplateKeyResolverTest
    {
        [TestMethod, TestCategory("TemplateKeyResolver"), TestCategory("Resolve")]
        public void ReturnsControllerUnderscoreActionForUnderscoreConvention()
        {
            // Arrange
            var context = CreateContext(new Dictionary<string, string?>
            {
                ["controller"] = "Products",
                ["action"] = "GetById"
            });

            var options = new SwazorOptions { NamingConvention = TemplateNamingConvention.Underscore };

            // Act
            var result = TemplateKeyResolver.Resolve(CreateOperation(), context, options);

            // Assert
            Assert.AreEqual("Products_GetById", result);
        }

        [TestMethod, TestCategory("TemplateKeyResolver"), TestCategory("Resolve")]
        public void ReturnsControllerSlashActionForSubdirectoryConvention()
        {
            // Arrange
            var context = CreateContext(new Dictionary<string, string?>
            {
                ["controller"] = "Products",
                ["action"] = "GetById"
            });

            var options = new SwazorOptions { NamingConvention = TemplateNamingConvention.Subdirectory };

            // Act
            var result = TemplateKeyResolver.Resolve(CreateOperation(), context, options);

            // Assert
            Assert.AreEqual("Products/GetById", result);
        }

        [TestMethod, TestCategory("TemplateKeyResolver"), TestCategory("Resolve")]
        public void PrefersAttributeOverOperationId()
        {
            // Arrange
            var context = CreateContext(
                routeValues: new Dictionary<string, string?>
                {
                    ["controller"] = "Products",
                    ["action"] = "GetById"
                },
                endpointMetadata: [new SwazorTemplateAttribute("custom/key")]);

            var operation = CreateOperation(operationId: "GetProductById");

            // Act
            var result = TemplateKeyResolver.Resolve(operation, context, new SwazorOptions());

            // Assert
            Assert.AreEqual("custom/key", result);
        }

        [TestMethod, TestCategory("TemplateKeyResolver"), TestCategory("Resolve")]
        public void ReturnsOperationIdWhenSetAndNoAttribute()
        {
            // Arrange
            var context = CreateContext();

            var operation = CreateOperation(operationId: "GetProductById");

            // Act
            var result = TemplateKeyResolver.Resolve(operation, context, new SwazorOptions());

            // Assert
            Assert.AreEqual("GetProductById", result);
        }

        [TestMethod, TestCategory("TemplateKeyResolver"), TestCategory("Resolve")]
        public void ReturnsNullWhenNoRouteValuesAndNoOperationId()
        {
            // Arrange
            var context = CreateContext();

            // Act
            var result = TemplateKeyResolver.Resolve(CreateOperation(), context, new SwazorOptions());

            // Assert
            Assert.IsNull(result);
        }
    }

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }
}