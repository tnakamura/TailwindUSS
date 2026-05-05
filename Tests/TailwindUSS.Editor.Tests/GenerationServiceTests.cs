using System.IO;
using System.Linq;

namespace TailwindUSS.Editor.Tests
{
    public sealed class GenerationServiceTests
    {
        [SetUp]
        public void SetUp()
        {
            AssetDatabase.Reset();
            Debug.Reset();
        }

        [Test]
        public void Generate_LoadFailureReturnsError()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, "{ invalid json }");

            var result = new GenerationService().Generate();

            Assert.That(result.ErrorCount, Is.EqualTo(1));
            Assert.That(result.GeneratedUtilityCount, Is.EqualTo(0));
            Assert.That(Debug.Entries.Last().Level, Is.EqualTo("Error"));
            Assert.That(Debug.Entries.Last().Message, Does.Contain("Failed to load"));
        }

        [Test]
        public void Generate_WritesOutputAndAutoAttachesStyles()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, "{\"inputGlobs\":[\"Assets/UI/**/*.uxml\"],\"outputUssPath\":\"Assets/Generated/TailwindUSS.generated.uss\",\"autoAttachGeneratedUss\":true}");
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"flex flex unknown p-7 bg-blue-500\"><ui:Label class=\"text-white\" /></ui:VisualElement></ui:UXML>");
            scope.WriteAssetFile("UI/Secondary.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"rounded\" /></ui:UXML>");

            var result = new GenerationService().Generate();

            var outputPath = scope.GetAssetPath("Generated", "TailwindUSS.generated.uss");
            var output = File.ReadAllText(outputPath).Replace("\r\n", "\n");
            var updatedMain = File.ReadAllText(scope.GetAssetPath("UI", "Main.uxml"));

            Assert.That(result.GeneratedUtilityCount, Is.EqualTo(4));
            Assert.That(result.WarningCount, Is.EqualTo(3));
            Assert.That(result.ErrorCount, Is.EqualTo(0));
            Assert.That(result.OutputAssetPath, Is.EqualTo("Assets/Generated/TailwindUSS.generated.uss"));
            Assert.That(output, Does.Contain(".bg-blue-500"));
            Assert.That(output, Does.Contain(".flex"));
            Assert.That(output, Does.Contain(".rounded"));
            Assert.That(updatedMain, Does.Contain("<ui:Style src=\"Assets/Generated/TailwindUSS.generated.uss\" />"));
            Assert.That(AssetDatabase.RefreshCallCount, Is.EqualTo(1));
            Assert.That(Debug.Entries.Any(entry => entry.Level == "Warning" && entry.Message.Contains("Duplicate token 'flex'")), Is.True);
            Assert.That(Debug.Entries.Any(entry => entry.Level == "Warning" && entry.Message.Contains("Invalid utility token 'p-7'")), Is.True);
            Assert.That(Debug.Entries.Any(entry => entry.Level == "Warning" && entry.Message.Contains("Unsupported utility token 'unknown'")), Is.True);
            Assert.That(Debug.Entries.Last().Message, Does.Contain("TailwindUSS generation finished."));
        }

        [Test]
        public void Generate_UsesDefaultConfigAndLogsWarningWhenConfigIsMissing()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"flex\" /></ui:UXML>");

            var result = new GenerationService().Generate();

            Assert.That(result.GeneratedUtilityCount, Is.EqualTo(1));
            Assert.That(Debug.Entries.First().Level, Is.EqualTo("Warning"));
            Assert.That(Debug.Entries.First().Message, Does.Contain("config was not found"));
        }

        [Test]
        public void LogDiagnostic_LoadsAssetContextForAssetPaths()
        {
            var diagnostic = new TailwindUssDiagnostic(
                DiagnosticSeverity.Warning,
                TokenIssueKind.Unsupported,
                "Unsupported utility token 'grid'.",
                "Assets/UI/Main.uxml",
                7,
                "VisualElement",
                "grid");

            GenerationService.LogDiagnostic(diagnostic);

            Assert.That(AssetDatabase.LoadedAssetPaths, Is.EqualTo(new[] { "Assets/UI/Main.uxml" }));
            Assert.That(Debug.Entries.Last().Message, Does.Contain("Assets/UI/Main.uxml:7, VisualElement"));
            Assert.That(Debug.Entries.Last().Context, Is.Not.Null);
        }

        [Test]
        public void LogDiagnostic_DoesNotLoadContextForNonAssetPaths()
        {
            var diagnostic = new TailwindUssDiagnostic(
                DiagnosticSeverity.Error,
                null,
                "Failed to load config.",
                "tailwinduss.config.json",
                0,
                string.Empty,
                string.Empty);

            GenerationService.LogDiagnostic(diagnostic);

            Assert.That(AssetDatabase.LoadedAssetPaths, Is.Empty);
            Assert.That(Debug.Entries.Last().Level, Is.EqualTo("Error"));
            Assert.That(Debug.Entries.Last().Message, Is.EqualTo("Failed to load config. (tailwinduss.config.json)"));
        }
    }
}
