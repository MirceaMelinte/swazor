namespace Swazor.Testing.Unit.Rendering;

using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Swazor.Infrastructure;
using Swazor.Rendering;

[TestClass]
public class RazorDescriptionRendererTest
{
    private string tempDir = null!;
    private string descriptionsDir = null!;

    [TestInitialize]
    public void SetUp()
    {
        tempDir = Path.Combine(Path.GetTempPath(), $"swazor_renderer_{Guid.NewGuid():N}");
        descriptionsDir = Path.Combine(tempDir, "Descriptions");
        Directory.CreateDirectory(descriptionsDir);
    }

    [TestCleanup]
    public void TearDown()
    {
        if (Directory.Exists(tempDir))
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    private void WriteTemplate(string relativePath, string content)
    {
        var fullPath = Path.Combine(descriptionsDir, relativePath);
        var dir = Path.GetDirectoryName(fullPath)!;
        Directory.CreateDirectory(dir);
        File.WriteAllText(fullPath, content);
    }

    private RazorDescriptionRenderer CreateRenderer()
    {
        var options = Options.Create(new SwazorOptions { DescriptionsPath = "Descriptions" });
        var environment = new StubWebHostEnvironment(tempDir);
        var logger = NullLogger<RazorDescriptionRenderer>.Instance;

        return new RazorDescriptionRenderer(options, environment, logger);
    }

    private static OperationDescriptionContext CreateContext(
        string httpMethod = "GET",
        string relativePath = "/api/test",
        string documentName = "v1") =>
        new()
        {
            HttpMethod = httpMethod,
            RelativePath = relativePath,
            DocumentName = documentName
        };

    [TestClass]
    public class RenderAsync : RazorDescriptionRendererTest
    {
        [TestMethod, TestCategory("RazorDescriptionRenderer"), TestCategory("RenderAsync")]
        public async Task ReturnsRenderedHtmlForValidTemplate()
        {
            // Arrange
            WriteTemplate("Test.cshtml", "<p>Test description</p>");

            var renderer = CreateRenderer();

            // Act
            var result = await renderer.RenderAsync("Test", CreateContext());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("<p>Test description</p>"));
        }

        [TestMethod, TestCategory("RazorDescriptionRenderer"), TestCategory("RenderAsync")]
        public async Task ReturnsNullForNonExistentTemplate()
        {
            // Arrange
            var renderer = CreateRenderer();

            // Act
            var result = await renderer.RenderAsync("Missing", CreateContext());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod, TestCategory("RazorDescriptionRenderer"), TestCategory("RenderAsync")]
        public async Task HandlesModelDynamicTemplate()
        {
            // Arrange
            WriteTemplate("DynModel.cshtml", "<p>@Model.HttpMethod @Model.RelativePath</p>");

            var renderer = CreateRenderer();
            var context = CreateContext(httpMethod: "POST", relativePath: "/api/items");

            // Act
            var result = await renderer.RenderAsync("DynModel", context);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("<p>POST /api/items</p>"));
        }

        [TestMethod, TestCategory("RazorDescriptionRenderer"), TestCategory("RenderAsync")]
        public async Task HandlesStronglyTypedModelTemplate()
        {
            // Arrange
            WriteTemplate(
                "TypedModel.cshtml",
                "@model Swazor.Rendering.OperationDescriptionContext\n<p>@Model.HttpMethod</p>");
            var renderer = CreateRenderer();
            var context = CreateContext(httpMethod: "DELETE");

            // Act
            var result = await renderer.RenderAsync("TypedModel", context);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("<p>DELETE</p>"));
        }

        [TestMethod, TestCategory("RazorDescriptionRenderer"), TestCategory("RenderAsync")]
        public async Task ModelPropertiesAccessibleInOutput()
        {
            // Arrange
            WriteTemplate(
                "Props.cshtml",
                "<ul><li>@Model.HttpMethod</li><li>@Model.RelativePath</li><li>@Model.DocumentName</li></ul>");
            var renderer = CreateRenderer();
            var context = CreateContext(httpMethod: "PUT", relativePath: "/api/widgets", documentName: "v2");

            // Act
            var result = await renderer.RenderAsync("Props", context);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("<li>PUT</li>"));
            Assert.IsTrue(result.Contains("<li>/api/widgets</li>"));
            Assert.IsTrue(result.Contains("<li>v2</li>"));
        }
    }

    [TestClass]
    public class TemplateExists : RazorDescriptionRendererTest
    {
        [TestMethod, TestCategory("RazorDescriptionRenderer"), TestCategory("TemplateExists")]
        public void ReturnsTrueForExistingTemplate()
        {
            // Arrange
            WriteTemplate("Present.cshtml", "<p>Here</p>");
            var renderer = CreateRenderer();

            // Act
            var result = renderer.TemplateExists("Present");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod, TestCategory("RazorDescriptionRenderer"), TestCategory("TemplateExists")]
        public void ReturnsFalseForMissingTemplate()
        {
            // Arrange
            var renderer = CreateRenderer();

            // Act
            var result = renderer.TemplateExists("Gone");

            // Assert
            Assert.IsFalse(result);
        }
    }

    private sealed class StubWebHostEnvironment(string contentRootPath) : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ApplicationName { get; set; } = "TestApp";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = contentRootPath;
        public string EnvironmentName { get; set; } = "Testing";
    }
}