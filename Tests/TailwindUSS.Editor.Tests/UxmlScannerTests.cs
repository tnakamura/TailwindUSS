using System.Linq;

namespace TailwindUSS.Editor.Tests
{
    public sealed class UxmlScannerTests
    {
        [Test]
        public void Scan_FindsTokensInMatchingFilesAndReportsDuplicates()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"flex px-4 px-4\"><ui:Label class=\"text-white\" /></ui:VisualElement></ui:UXML>");
            scope.WriteAssetFile("Ignored/Secondary.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"hidden\" /></ui:UXML>");

            var scanner = new UxmlScanner();

            var result = scanner.Scan(scope.RootPath, new[] { "Assets/UI/**/*.uxml" });

            Assert.That(result.MatchedFiles, Is.EqualTo(new[] { "Assets/UI/Main.uxml" }));
            Assert.That(result.Occurrences.Select(occurrence => occurrence.OriginalToken), Is.EqualTo(new[] { "flex", "px-4", "text-white" }));
            Assert.That(result.Diagnostics, Has.Count.EqualTo(1));
            Assert.That(result.Diagnostics[0].IssueKind, Is.EqualTo(TokenIssueKind.Duplicate));
            Assert.That(result.Diagnostics[0].LineNumber, Is.GreaterThan(0));
        }

        [Test]
        public void Scan_UsesDefaultGlobWhenAllConfiguredGlobsAreBlank()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"flex\" /></ui:UXML>");

            var scanner = new UxmlScanner();

            var result = scanner.Scan(scope.RootPath, new[] { string.Empty, "  " });

            Assert.That(result.MatchedFiles, Is.EqualTo(new[] { "Assets/UI/Main.uxml" }));
            Assert.That(result.Occurrences.Select(occurrence => occurrence.OriginalToken), Is.EqualTo(new[] { "flex" }));
        }

        [Test]
        public void Scan_SupportsSingleCharacterWildcards()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Menu1.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"flex\" /></ui:UXML>");
            scope.WriteAssetFile("UI/Menu12.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"hidden\" /></ui:UXML>");

            var scanner = new UxmlScanner();

            var result = scanner.Scan(scope.RootPath, new[] { "Assets/UI/Menu?.uxml" });

            Assert.That(result.MatchedFiles, Is.EqualTo(new[] { "Assets/UI/Menu1.uxml" }));
        }

        [Test]
        public void Scan_ReportsParseErrors()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Broken.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"flex\"></ui:UXML>");

            var scanner = new UxmlScanner();

            var result = scanner.Scan(scope.RootPath, new[] { "Assets/**/*.uxml" });

            Assert.That(result.Diagnostics, Has.Count.EqualTo(1));
            Assert.That(result.Diagnostics[0].Severity, Is.EqualTo(DiagnosticSeverity.Error));
            Assert.That(result.Diagnostics[0].Message, Does.StartWith("Failed to parse UXML:"));
            Assert.That(result.Occurrences, Is.Empty);
        }
    }
}
