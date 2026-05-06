using System.Collections.Generic;
using System.Linq;

namespace TailwindUSS.Editor.Tests
{
    public sealed class ClassTokenParserTests
    {
        [Test]
        public void Parse_ReturnsDistinctTokensAndReportsDuplicates()
        {
            var parser = new ClassTokenParser();
            var diagnostics = new List<TailwindUssDiagnostic>();

            var tokens = parser.Parse("flex px-4 flex text-white px-4", "Assets/UI/Main.uxml", 12, "VisualElement", diagnostics);

            Assert.That(tokens, Is.EqualTo(new[] { "flex", "px-4", "text-white" }));
            Assert.That(diagnostics, Has.Count.EqualTo(2));
            Assert.That(diagnostics.Select(diagnostic => diagnostic.IssueKind), Is.All.EqualTo(TokenIssueKind.Duplicate));
            Assert.That(diagnostics.Select(diagnostic => diagnostic.Token), Is.EqualTo(new[] { "flex", "px-4" }));
        }

        [Test]
        public void Parse_IgnoresExtraWhitespace()
        {
            var parser = new ClassTokenParser();
            var diagnostics = new List<TailwindUssDiagnostic>();

            var tokens = parser.Parse("  flex\t\ntext-sm  ", "Assets/UI/Main.uxml", 3, "Label", diagnostics);

            Assert.That(tokens, Is.EqualTo(new[] { "flex", "text-sm" }));
            Assert.That(diagnostics, Is.Empty);
        }
    }
}
