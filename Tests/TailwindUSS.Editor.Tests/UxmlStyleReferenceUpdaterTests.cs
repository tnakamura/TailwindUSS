using System.Collections.Generic;
using System.IO;

namespace TailwindUSS.Editor.Tests
{
    public sealed class UxmlStyleReferenceUpdaterTests
    {
        [Test]
        public void EnsureStyleReference_AddsStyleElementWhenMissing()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement /></ui:UXML>");
            var diagnostics = new List<TailwindUssDiagnostic>();

            var updater = new UxmlStyleReferenceUpdater();

            updater.EnsureStyleReference(scope.RootPath, new[] { "Assets/UI/Main.uxml" }, "Assets/Generated/TailwindUSS.generated.uss", diagnostics);

            var updated = File.ReadAllText(scope.GetAssetPath("UI", "Main.uxml"));
            Assert.That(updated, Does.Contain("<ui:Style src=\"Assets/Generated/TailwindUSS.generated.uss\" />"));
            Assert.That(diagnostics, Is.Empty);
        }

        [Test]
        public void EnsureStyleReference_DoesNotAddDuplicateStyleElement()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:Style src=\"Assets/Generated/TailwindUSS.generated.uss\" /><ui:VisualElement /></ui:UXML>");
            var diagnostics = new List<TailwindUssDiagnostic>();

            var updater = new UxmlStyleReferenceUpdater();

            updater.EnsureStyleReference(scope.RootPath, new[] { "Assets/UI/Main.uxml" }, "Assets/Generated/TailwindUSS.generated.uss", diagnostics);

            var updated = File.ReadAllText(scope.GetAssetPath("UI", "Main.uxml"));
            Assert.That(updated.Split("TailwindUSS.generated.uss"), Has.Length.EqualTo(2));
            Assert.That(diagnostics, Is.Empty);
        }

        [Test]
        public void EnsureStyleReference_ReportsErrorsForBrokenXml()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Broken.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement>");
            var diagnostics = new List<TailwindUssDiagnostic>();

            var updater = new UxmlStyleReferenceUpdater();

            updater.EnsureStyleReference(scope.RootPath, new[] { "Assets/UI/Broken.uxml" }, "Assets/Generated/TailwindUSS.generated.uss", diagnostics);

            Assert.That(diagnostics, Has.Count.EqualTo(1));
            Assert.That(diagnostics[0].Severity, Is.EqualTo(DiagnosticSeverity.Error));
            Assert.That(diagnostics[0].Message, Does.StartWith("Failed to update UXML style reference:"));
        }
    }
}
