namespace Swazor.Testing.Unit.Rendering;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swazor.Rendering;

[TestClass]
public class SwazorTemplateBaseTest
{
    private sealed class TestableTemplate : SwazorTemplateBase
    {
        public override Task ExecuteAsync() => Task.CompletedTask;

        public new void WriteLiteral(string? literal) => base.WriteLiteral(literal);

        public new void Write(object? value) => base.Write(value);
    }

    [TestClass]
    public class WriteLiteral : SwazorTemplateBaseTest
    {
        [TestMethod, TestCategory("SwazorTemplateBase"), TestCategory("WriteLiteral")]
        public void AppendsRawHtmlUnchanged()
        {
            // Arrange
            var template = new TestableTemplate();

            // Act
            template.WriteLiteral("<div class=\"test\">Hello</div>");

            // Assert
            Assert.AreEqual("<div class=\"test\">Hello</div>", template.GetOutput());
        }

        [TestMethod, TestCategory("SwazorTemplateBase"), TestCategory("WriteLiteral")]
        public void WritesNothingWhenNull()
        {
            // Arrange
            var template = new TestableTemplate();

            // Act
            template.WriteLiteral(null);

            // Assert
            Assert.AreEqual(string.Empty, template.GetOutput());
        }

        [TestMethod, TestCategory("SwazorTemplateBase"), TestCategory("WriteLiteral")]
        public void AccumulatesMultipleCalls()
        {
            // Arrange
            var template = new TestableTemplate();

            // Act
            template.WriteLiteral("<h1>");
            template.WriteLiteral("Hello");
            template.WriteLiteral("</h1>");

            // Assert
            Assert.AreEqual("<h1>Hello</h1>", template.GetOutput());
        }
    }

    [TestClass]
    public class Write : SwazorTemplateBaseTest
    {
        [TestMethod, TestCategory("SwazorTemplateBase"), TestCategory("Write")]
        public void HtmlEncodesOutput()
        {
            // Arrange
            var template = new TestableTemplate();

            // Act
            template.Write("<script>alert('xss')</script>");

            // Assert
            Assert.AreEqual("&lt;script&gt;alert(&#x27;xss&#x27;)&lt;/script&gt;", template.GetOutput());
        }

        [TestMethod, TestCategory("SwazorTemplateBase"), TestCategory("Write")]
        public void WritesNothingWhenNull()
        {
            // Arrange
            var template = new TestableTemplate();

            // Act
            template.Write(null);

            // Assert
            Assert.AreEqual(string.Empty, template.GetOutput());
        }

        [TestMethod, TestCategory("SwazorTemplateBase"), TestCategory("Write")]
        public void EncodesSpecialCharacters()
        {
            // Arrange
            var template = new TestableTemplate();

            // Act
            template.Write("a & b < c > d \"e\" f'g");

            // Assert
            Assert.AreEqual("a &amp; b &lt; c &gt; d &quot;e&quot; f&#x27;g", template.GetOutput());
        }
    }

    [TestClass]
    public class GetOutput : SwazorTemplateBaseTest
    {
        [TestMethod, TestCategory("SwazorTemplateBase"), TestCategory("GetOutput")]
        public void ReturnsAccumulatedContent()
        {
            // Arrange
            var template = new TestableTemplate();
            template.WriteLiteral("<p>");
            template.Write("encoded & value");
            template.WriteLiteral("</p>");

            // Act
            var result = template.GetOutput();

            // Assert
            Assert.AreEqual("<p>encoded &amp; value</p>", result);
        }

        [TestMethod, TestCategory("SwazorTemplateBase"), TestCategory("GetOutput")]
        public void ReturnsEmptyStringWhenNothingWritten()
        {
            // Arrange
            var template = new TestableTemplate();

            // Act
            var result = template.GetOutput();

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod, TestCategory("SwazorTemplateBase"), TestCategory("GetOutput")]
        public void GenericVariantExposesTypedModel()
        {
            // Arrange
            var template = new TypedTestableTemplate();
            template.Model = new TestModel { Name = "Test" };

            // Act
            var result = template.Model.Name;

            // Assert
            Assert.AreEqual("Test", result);
        }
    }

    private sealed class TestModel
    {
        public string Name { get; init; } = string.Empty;
    }

    private sealed class TypedTestableTemplate : SwazorTemplateBase<TestModel>
    {
        public override Task ExecuteAsync() => Task.CompletedTask;
    }
}