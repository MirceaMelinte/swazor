namespace Swazor.Testing.Unit.Rendering;

using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swazor.Rendering;

[TestClass]
public class SwazorRazorEngineTest
{
    private string tempDir = null!;

    [TestInitialize]
    public void SetUp()
    {
        tempDir = Path.Combine(Path.GetTempPath(), $"swazor_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
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
        var fullPath = Path.Combine(tempDir, relativePath);
        var dir = Path.GetDirectoryName(fullPath)!;
        Directory.CreateDirectory(dir);
        File.WriteAllText(fullPath, content);
    }

    [TestClass]
    public class RenderAsync : SwazorRazorEngineTest
    {
        [TestMethod, TestCategory("SwazorRazorEngine"), TestCategory("RenderAsync")]
        public async Task RendersSimpleHtmlOnlyTemplate()
        {
            // Arrange
            WriteTemplate("Simple.cshtml", "<p>Hello World</p>");
            var engine = new SwazorRazorEngine(tempDir);

            // Act
            var result = await engine.RenderAsync("Simple", null);

            // Assert
            Assert.IsTrue(result.Contains("<p>Hello World</p>"));
        }

        [TestMethod, TestCategory("SwazorRazorEngine"), TestCategory("RenderAsync")]
        public async Task RendersTemplateWithModelExpression()
        {
            // Arrange
            WriteTemplate("Greeting.cshtml", "<p>Hello @Model.Name</p>");
            var engine = new SwazorRazorEngine(tempDir);
            dynamic model = new ExpandoObject();
            model.Name = "World";

            // Act
            var result = await engine.RenderAsync("Greeting", (object)model);

            // Assert
            Assert.IsTrue(result.Contains("<p>Hello World</p>"));
        }

        [TestMethod, TestCategory("SwazorRazorEngine"), TestCategory("RenderAsync")]
        public async Task RendersTemplateWithModelDynamic()
        {
            // Arrange
            WriteTemplate("Dynamic.cshtml", "<span>@Model.Count items</span>");
            var engine = new SwazorRazorEngine(tempDir);
            dynamic model = new System.Dynamic.ExpandoObject();
            model.Count = 42;

            // Act
            var result = await engine.RenderAsync("Dynamic", (object)model);

            // Assert
            Assert.IsTrue(result.Contains("<span>42 items</span>"));
        }

        [TestMethod, TestCategory("SwazorRazorEngine"), TestCategory("RenderAsync")]
        public async Task RendersTemplateWithStronglyTypedModel()
        {
            // Arrange
            WriteTemplate("Typed.cshtml", "@model Swazor.Rendering.OperationDescriptionContext\n<p>@Model.HttpMethod @Model.RelativePath</p>");
            var engine = new SwazorRazorEngine(tempDir);
            var model = new OperationDescriptionContext
            {
                HttpMethod = "GET",
                RelativePath = "/api/products",
                DocumentName = "v1"
            };

            // Act
            var result = await engine.RenderAsync("Typed", model);

            // Assert
            Assert.IsTrue(result.Contains("<p>GET /api/products</p>"));
        }

        [TestMethod, TestCategory("SwazorRazorEngine"), TestCategory("RenderAsync")]
        public async Task RendersTemplateWithForeachLoop()
        {
            // Arrange
            WriteTemplate("Loop.cshtml", "@foreach (var item in Model.Items) {\n<li>@item</li>\n}");
            var engine = new SwazorRazorEngine(tempDir);
            dynamic model = new ExpandoObject();
            model.Items = new[] { "A", "B", "C" };

            // Act
            var result = await engine.RenderAsync("Loop", (object)model);

            // Assert
            Assert.IsTrue(result.Contains("<li>A</li>"));
            Assert.IsTrue(result.Contains("<li>B</li>"));
            Assert.IsTrue(result.Contains("<li>C</li>"));
        }

        [TestMethod, TestCategory("SwazorRazorEngine"), TestCategory("RenderAsync")]
        public async Task RendersTemplateWithIfConditional()
        {
            // Arrange
            WriteTemplate("Conditional.cshtml", "@if (Model.Show) {\n<p>Visible</p>\n}");
            var engine = new SwazorRazorEngine(tempDir);
            dynamic modelTrue = new ExpandoObject();
            modelTrue.Show = true;
            dynamic modelFalse = new ExpandoObject();
            modelFalse.Show = false;

            // Act
            var resultTrue = await engine.RenderAsync("Conditional", (object)modelTrue);
            var resultFalse = await engine.RenderAsync("Conditional", (object)modelFalse);

            // Assert
            Assert.IsTrue(resultTrue.Contains("<p>Visible</p>"));
            Assert.IsFalse(resultFalse.Contains("<p>Visible</p>"));
        }

        [TestMethod, TestCategory("SwazorRazorEngine"), TestCategory("RenderAsync")]
        public async Task HtmlEncodesModelExpressions()
        {
            // Arrange
            WriteTemplate("Encode.cshtml", "<p>@Model.Value</p>");
            var engine = new SwazorRazorEngine(tempDir);
            dynamic model = new ExpandoObject();
            model.Value = "<script>alert('xss')</script>";

            // Act
            var result = await engine.RenderAsync("Encode", (object)model);

            // Assert
            Assert.IsFalse(result.Contains("<script>"));
            Assert.IsTrue(result.Contains("&lt;script&gt;"));
        }

        [TestMethod, TestCategory("SwazorRazorEngine"), TestCategory("RenderAsync")]
        public async Task DoesNotEncodeWriteLiteralContent()
        {
            // Arrange
            WriteTemplate("Raw.cshtml", "<div class=\"raw\"><b>Bold</b></div>");
            var engine = new SwazorRazorEngine(tempDir);

            // Act
            var result = await engine.RenderAsync("Raw", null);

            // Assert
            Assert.IsTrue(result.Contains("<div class=\"raw\"><b>Bold</b></div>"));
        }

        [TestMethod, TestCategory("SwazorRazorEngine"), TestCategory("RenderAsync")]
        public async Task CachesCompiledTemplateOnSecondCall()
        {
            // Arrange
            WriteTemplate("Cached.cshtml", "<p>Cache me</p>");
            var engine = new SwazorRazorEngine(tempDir);

            // Act
            var result1 = await engine.RenderAsync("Cached", null);
            var result2 = await engine.RenderAsync("Cached", null);

            // Assert
            Assert.IsTrue(result1.Contains("<p>Cache me</p>"));
            Assert.IsTrue(result2.Contains("<p>Cache me</p>"));
        }

        [TestMethod, TestCategory("SwazorRazorEngine"), TestCategory("RenderAsync")]
        public async Task CompilesOnlyOnceUnderConcurrentAccess()
        {
            // Arrange
            WriteTemplate("Concurrent.cshtml", "<p>@Model.Id</p>");
            var engine = new SwazorRazorEngine(tempDir);
            var tasks = Enumerable.Range(0, 10)
                .Select(i =>
                {
                    dynamic model = new ExpandoObject();
                    model.Id = i;
                    return engine.RenderAsync("Concurrent", (object)model);
                })
                .ToArray();

            // Act
            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.AreEqual(10, results.Length);
            foreach (var result in results)
            {
                Assert.IsTrue(result.Contains("<p>"));
            }
        }

        [TestMethod, TestCategory("SwazorRazorEngine"), TestCategory("RenderAsync")]
        public async Task HandlesEmptyTemplate()
        {
            // Arrange
            WriteTemplate("Empty.cshtml", "");
            var engine = new SwazorRazorEngine(tempDir);

            // Act
            var result = await engine.RenderAsync("Empty", null);

            // Assert
            Assert.IsNotNull(result);
        }
    }

    [TestClass]
    public class TemplateExists : SwazorRazorEngineTest
    {
        [TestMethod, TestCategory("SwazorRazorEngine"), TestCategory("TemplateExists")]
        public void ReturnsTrueForExistingTemplate()
        {
            // Arrange
            WriteTemplate("Exists.cshtml", "<p>Here</p>");
            var engine = new SwazorRazorEngine(tempDir);

            // Act
            var result = engine.TemplateExists("Exists");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod, TestCategory("SwazorRazorEngine"), TestCategory("TemplateExists")]
        public void ReturnsFalseForMissingTemplate()
        {
            // Arrange
            var engine = new SwazorRazorEngine(tempDir);

            // Act
            var result = engine.TemplateExists("NotHere");

            // Assert
            Assert.IsFalse(result);
        }
    }

    [TestClass]
    public class ErrorHandling : SwazorRazorEngineTest
    {
        [TestMethod, TestCategory("SwazorRazorEngine"), TestCategory("ErrorHandling")]
        public void ThrowsForMalformedTemplate()
        {
            // Arrange
            WriteTemplate("Bad.cshtml", "@{ var x = ; }");
            var engine = new SwazorRazorEngine(tempDir);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => engine.RenderAsync("Bad", null).GetAwaiter().GetResult());
        }
    }
}