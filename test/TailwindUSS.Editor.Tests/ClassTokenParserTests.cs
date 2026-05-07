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

            Assert.That(tokens.Select(token => token.OriginalToken), Is.EqualTo(new[] { "flex", "px-4", "text-white" }));
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

            Assert.That(tokens.Select(token => token.OriginalToken), Is.EqualTo(new[] { "flex", "text-sm" }));
            Assert.That(diagnostics, Is.Empty);
        }

        [Test]
        public void Parse_SplitsVariantChainAndBaseToken()
        {
            var parser = new ClassTokenParser();
            var diagnostics = new List<TailwindUssDiagnostic>();

            var tokens = parser.Parse("hover:bg-blue-500 hover:bg-blue-500 focus:text-white hover:focus:bg-blue-500", "Assets/UI/Main.uxml", 8, "Button", diagnostics);

            Assert.That(tokens.Select(token => token.OriginalToken), Is.EqualTo(new[] { "hover:bg-blue-500", "focus:text-white", "hover:focus:bg-blue-500" }));
            Assert.That(tokens[0].VariantChain, Is.EqualTo(new[] { "hover" }));
            Assert.That(tokens[0].BaseToken, Is.EqualTo("bg-blue-500"));
            Assert.That(tokens[1].VariantChain, Is.EqualTo(new[] { "focus" }));
            Assert.That(tokens[1].BaseToken, Is.EqualTo("text-white"));
            Assert.That(tokens[2].VariantChain, Is.EqualTo(new[] { "hover", "focus" }));
            Assert.That(tokens[2].BaseToken, Is.EqualTo("bg-blue-500"));
            Assert.That(diagnostics, Has.Count.EqualTo(1));
            Assert.That(diagnostics[0].IssueKind, Is.EqualTo(TokenIssueKind.Duplicate));
            Assert.That(diagnostics[0].Token, Is.EqualTo("hover:bg-blue-500"));
        }
    }
}
