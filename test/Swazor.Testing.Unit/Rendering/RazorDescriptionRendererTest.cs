namespace Swazor.Testing.Unit.Rendering;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swazor.Abstractions;
using Swazor.Infrastructure;
using Swazor.Rendering;
using Swazor.Testing.Unit.Infrastructure;

public class RazorDescriptionRendererTest
{
    private static OperationDescriptionContext CreateContext(
        string httpMethod = "GET",
        string relativePath = "/api/products",
        string documentName = "v1",
        string? summary = null) =>
        new()
        {
            HttpMethod = httpMethod,
            RelativePath = relativePath,
            DocumentName = documentName,
            Summary = summary
        };

    private static RazorDescriptionRenderer CreateRenderer(IRazorViewRenderer viewRenderer, string descriptionsPath = "Descriptions")
    {
        var options = Options.Create(new SwazorOptions { DescriptionsPath = descriptionsPath });

        return new RazorDescriptionRenderer(viewRenderer, options, NullLogger<RazorDescriptionRenderer>.Instance);
    }

    private static RazorDescriptionRenderer CreateRealRenderer(string descriptionsPath = "Descriptions")
    {
        var services = new ServiceCollection();
        services.AddRazorViewRendering();

        var viewRenderer = services.BuildServiceProvider().GetRequiredService<IRazorViewRenderer>();

        return CreateRenderer(viewRenderer, descriptionsPath);
    }

    [TestClass]
    public class RenderAsync : RazorDescriptionRendererTest
    {
        [TestMethod, TestCategory("RazorDescriptionRenderer"), TestCategory("RenderAsync")]
        public async Task RendersCompiledViewWithStronglyTypedModel()
        {
            // Arrange
            var renderer = CreateRealRenderer();
            var context = CreateContext(httpMethod: "GET", relativePath: "/api/products");

            // Act
            var (templateExists, html) = await renderer.RenderAsync("Typed", context);

            // Assert
            Assert.IsTrue(templateExists);
            Assert.IsNotNull(html);
            Assert.IsTrue(html.Contains("<p>GET /api/products</p>"));
        }

        [TestMethod, TestCategory("RazorDescriptionRenderer"), TestCategory("RenderAsync")]
        public async Task RendersViewInSubdirectory()
        {
            // Arrange
            var renderer = CreateRealRenderer();
            var context = CreateContext(documentName: "v1");

            // Act
            var (templateExists, html) = await renderer.RenderAsync("Sub/Nested", context);

            // Assert
            Assert.IsTrue(templateExists);
            Assert.IsNotNull(html);
            Assert.IsTrue(html.Contains("nested v1"));
        }

        [TestMethod, TestCategory("RazorDescriptionRenderer"), TestCategory("RenderAsync")]
        public async Task HtmlEncodesModelValues()
        {
            // Arrange
            var renderer = CreateRealRenderer();
            var context = CreateContext(summary: "<script>alert('xss')</script>");

            // Act
            var (templateExists, html) = await renderer.RenderAsync("Encode", context);

            // Assert
            Assert.IsTrue(templateExists);
            Assert.IsNotNull(html);
            Assert.IsFalse(html.Contains("<script>"));
            Assert.IsTrue(html.Contains("&lt;script&gt;"));
        }

        [TestMethod, TestCategory("RazorDescriptionRenderer"), TestCategory("RenderAsync")]
        public async Task ReportsTemplateMissingForUnknownKey()
        {
            // Arrange
            var renderer = CreateRealRenderer();

            // Act
            var (templateExists, html) = await renderer.RenderAsync("DoesNotExist", CreateContext());

            // Assert
            Assert.IsFalse(templateExists);
            Assert.IsNull(html);
        }

        [TestMethod, TestCategory("RazorDescriptionRenderer"), TestCategory("RenderAsync")]
        public async Task MapsTemplateKeyToConfiguredViewPath()
        {
            // Arrange
            var viewRenderer = new StubViewRenderer("<p>ok</p>");
            var renderer = CreateRenderer(viewRenderer, descriptionsPath: "Docs/Api");

            // Act
            await renderer.RenderAsync("Products_GetById", CreateContext());

            // Assert
            Assert.AreEqual("/Docs/Api/Products_GetById.cshtml", viewRenderer.LastViewPath);
        }

        [TestMethod, TestCategory("RazorDescriptionRenderer"), TestCategory("RenderAsync")]
        public async Task ReportsTemplateExistsButNullHtmlWhenRenderThrows()
        {
            // Arrange
            var viewRenderer = new StubViewRenderer(_ => throw new InvalidOperationException("boom"));
            var renderer = CreateRenderer(viewRenderer);

            // Act
            var (templateExists, html) = await renderer.RenderAsync("Typed", CreateContext());

            // Assert
            Assert.IsTrue(templateExists);
            Assert.IsNull(html);
        }
    }
}