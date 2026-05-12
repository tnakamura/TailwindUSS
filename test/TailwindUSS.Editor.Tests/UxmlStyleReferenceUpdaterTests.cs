using System.Collections.Generic;
using System.IO;

namespace TailwindUSS.Editor.Tests
{
    /// <summary>
    /// Represents the uxml style reference updater tests.
    /// </summary>
    public sealed class UxmlStyleReferenceUpdaterTests
    {
        /// <summary>
        /// Tests that ensure style reference adds style element when missing.
        /// </summary>
        [Test]
        public void EnsureStyleReference_AddsStyleElementWhenMissing()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement /></ui:UXML>");
            var diagnostics = new List<TailwindUssDiagnostic>();

            var updater = new UxmlStyleReferenceUpdater();

            updater.EnsureStyleReference(scope.RootPath, new[] { "Assets/UI/Main.uxml" }, "Assets/Generated/TailwindUSS.generated.uss", diagnostics);

            var updated = File.ReadAllText(scope.GetAssetPath("UI", "Main.uxml"));
            Assert.That(updated, Does.Contain("<ui:Style src=\"project://database/Assets/Generated/TailwindUSS.generated.uss\" />"));
            Assert.That(diagnostics, Is.Empty);
        }

        /// <summary>
        /// Tests that ensure style reference does not add duplicate style element.
        /// </summary>
        [Test]
        public void EnsureStyleReference_DoesNotAddDuplicateStyleElement()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:Style src=\"project://database/Assets/Generated/TailwindUSS.generated.uss\" /><ui:VisualElement /></ui:UXML>");
            var diagnostics = new List<TailwindUssDiagnostic>();

            var updater = new UxmlStyleReferenceUpdater();

            updater.EnsureStyleReference(scope.RootPath, new[] { "Assets/UI/Main.uxml" }, "Assets/Generated/TailwindUSS.generated.uss", diagnostics);

            var updated = File.ReadAllText(scope.GetAssetPath("UI", "Main.uxml"));
            Assert.That(updated.Split("TailwindUSS.generated.uss"), Has.Length.EqualTo(2));
            Assert.That(diagnostics, Is.Empty);
        }

        /// <summary>
        /// Tests that ensure style reference rewrites legacy asset path reference.
        /// </summary>
        [Test]
        public void EnsureStyleReference_RewritesLegacyAssetPathReference()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:Style src=\"Assets/Generated/TailwindUSS.generated.uss\" /><ui:VisualElement /></ui:UXML>");
            var diagnostics = new List<TailwindUssDiagnostic>();

            var updater = new UxmlStyleReferenceUpdater();

            updater.EnsureStyleReference(scope.RootPath, new[] { "Assets/UI/Main.uxml" }, "Assets/Generated/TailwindUSS.generated.uss", diagnostics);

            var updated = File.ReadAllText(scope.GetAssetPath("UI", "Main.uxml"));
            Assert.That(updated, Does.Not.Contain("src=\"Assets/Generated/TailwindUSS.generated.uss\""));
            Assert.That(updated, Does.Contain("src=\"project://database/Assets/Generated/TailwindUSS.generated.uss\""));
            Assert.That(diagnostics, Is.Empty);
        }

        /// <summary>
        /// Tests that ensure style reference reports errors for broken xml.
        /// </summary>
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
