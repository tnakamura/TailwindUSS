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
        public void Generate_WritesPhaseOneLayoutUtilities()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, "{\"inputGlobs\":[\"Assets/UI/**/*.uxml\"],\"outputUssPath\":\"Assets/Generated/TailwindUSS.generated.uss\",\"autoAttachGeneratedUss\":false}");
            scope.WriteAssetFile("UI/Layout.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"absolute top-0 right-0 opacity-50 overflow-hidden\" /></ui:UXML>");

            var result = new GenerationService().Generate();

            var output = File.ReadAllText(scope.GetAssetPath("Generated", "TailwindUSS.generated.uss")).Replace("\r\n", "\n");

            Assert.That(result.GeneratedUtilityCount, Is.EqualTo(5));
            Assert.That(result.WarningCount, Is.EqualTo(0));
            Assert.That(output, Does.Contain(".absolute {\n    position: absolute;\n}"));
            Assert.That(output, Does.Contain(".top-0 {\n    top: 0px;\n}"));
            Assert.That(output, Does.Contain(".right-0 {\n    right: 0px;\n}"));
            Assert.That(output, Does.Contain(".opacity-50 {\n    opacity: 0.5;\n}"));
            Assert.That(output, Does.Contain(".overflow-hidden {\n    overflow: hidden;\n}"));
        }

        [Test]
        public void Generate_WritesPhaseOneFlexAndTypographyUtilities()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, "{\"inputGlobs\":[\"Assets/UI/**/*.uxml\"],\"outputUssPath\":\"Assets/Generated/TailwindUSS.generated.uss\",\"autoAttachGeneratedUss\":false}");
            scope.WriteAssetFile("UI/Text.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"flex-wrap gap-x-4 gap-y-2 mt-3 self-center basis-4 order-2\" /><ui:Label class=\"truncate tracking-wide leading-6 text-xl uppercase break-all\" /></ui:UXML>");

            var result = new GenerationService().Generate();

            var output = File.ReadAllText(scope.GetAssetPath("Generated", "TailwindUSS.generated.uss")).Replace("\r\n", "\n");

            Assert.That(result.GeneratedUtilityCount, Is.EqualTo(13));
            Assert.That(result.WarningCount, Is.EqualTo(0));
            Assert.That(output, Does.Contain(".basis-4 {\n    flex-basis: 16px;\n}"));
            Assert.That(output, Does.Contain(".break-all {\n    word-break: break-all;\n}"));
            Assert.That(output, Does.Contain(".flex-wrap {\n    flex-wrap: wrap;\n}"));
            Assert.That(output, Does.Contain(".gap-x-4 {\n    column-gap: 16px;\n}"));
            Assert.That(output, Does.Contain(".gap-y-2 {\n    row-gap: 8px;\n}"));
            Assert.That(output, Does.Contain(".leading-6 {\n    line-height: 24px;\n}"));
            Assert.That(output, Does.Contain(".mt-3 {\n    margin-top: 12px;\n}"));
            Assert.That(output, Does.Contain(".order-2 {\n    order: 2;\n}"));
            Assert.That(output, Does.Contain(".self-center {\n    align-self: center;\n}"));
            Assert.That(output, Does.Contain(".tracking-wide {\n    letter-spacing: 0.025em;\n}"));
            Assert.That(output, Does.Contain(".text-xl {\n    font-size: 20px;\n}"));
            Assert.That(output, Does.Contain(".truncate {\n    overflow: hidden;\n    text-overflow: ellipsis;\n    white-space: nowrap;\n}"));
            Assert.That(output, Does.Contain(".uppercase {\n    text-transform: uppercase;\n}"));
        }

        [Test]
        public void Generate_WritesPhaseTwoBorderAndBackgroundUtilities()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, "{\"inputGlobs\":[\"Assets/UI/**/*.uxml\"],\"outputUssPath\":\"Assets/Generated/TailwindUSS.generated.uss\",\"autoAttachGeneratedUss\":false}");
            scope.WriteAssetFile("UI/Decorated.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"border-4 border-t-sky-500 rounded-t-lg rounded-bl-full bg-slate-500 bg-cover bg-center bg-no-repeat bg-none\" /></ui:UXML>");

            var result = new GenerationService().Generate();

            var output = File.ReadAllText(scope.GetAssetPath("Generated", "TailwindUSS.generated.uss")).Replace("\r\n", "\n");

            Assert.That(result.GeneratedUtilityCount, Is.EqualTo(9));
            Assert.That(result.WarningCount, Is.EqualTo(0));
            Assert.That(output, Does.Contain(".bg-center {\n    background-position: center center;\n}"));
            Assert.That(output, Does.Contain(".bg-cover {\n    background-size: cover;\n}"));
            Assert.That(output, Does.Contain(".bg-no-repeat {\n    background-repeat: no-repeat;\n}"));
            Assert.That(output, Does.Contain(".bg-none {\n    background-image: none;\n}"));
            Assert.That(output, Does.Contain(".bg-slate-500 {\n    background-color: #64748B;\n}"));
            Assert.That(output, Does.Contain(".border-4 {\n    border-top-width: 4px;\n    border-right-width: 4px;\n    border-bottom-width: 4px;\n    border-left-width: 4px;\n}"));
            Assert.That(output, Does.Contain(".border-t-sky-500 {\n    border-top-color: #0EA5E9;\n}"));
            Assert.That(output, Does.Contain(".rounded-bl-full {\n    border-bottom-left-radius: 9999px;\n}"));
            Assert.That(output, Does.Contain(".rounded-t-lg {\n    border-top-left-radius: 8px;\n    border-top-right-radius: 8px;\n}"));
        }

        [Test]
        public void Generate_WritesVariantSelectorsAndReportsUnsupportedVariants()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, "{\"inputGlobs\":[\"Assets/UI/**/*.uxml\"],\"outputUssPath\":\"Assets/Generated/TailwindUSS.generated.uss\",\"autoAttachGeneratedUss\":false}");
            scope.WriteAssetFile("UI/Interactive.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:Button class=\"hover:bg-blue-500 focus:text-white hover:focus:bg-blue-500 group-hover:bg-blue-500\" /></ui:UXML>");

            var result = new GenerationService().Generate();

            var output = File.ReadAllText(scope.GetAssetPath("Generated", "TailwindUSS.generated.uss")).Replace("\r\n", "\n");

            Assert.That(result.GeneratedUtilityCount, Is.EqualTo(3));
            Assert.That(result.WarningCount, Is.EqualTo(1));
            Assert.That(result.ErrorCount, Is.EqualTo(0));
            Assert.That(output, Does.Contain(".focus\\:text-white:focus {\n    color: #FFFFFF;\n}"));
            Assert.That(output, Does.Contain(".hover\\:bg-blue-500:hover {\n    background-color: #3B82F6;\n}"));
            Assert.That(output, Does.Contain(".hover\\:focus\\:bg-blue-500:hover:focus {\n    background-color: #3B82F6;\n}"));
            Assert.That(Debug.Entries.Any(entry => entry.Level == "Warning" && entry.Message.Contains("Unsupported variant in utility token 'group-hover:bg-blue-500': Unsupported variant 'group-hover'.")), Is.True);
        }

        [Test]
        public void Generate_WritesPhaseFourTransformTransitionAndCursorUtilities()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, "{\"inputGlobs\":[\"Assets/UI/**/*.uxml\"],\"outputUssPath\":\"Assets/Generated/TailwindUSS.generated.uss\",\"autoAttachGeneratedUss\":false}");
            scope.WriteAssetFile("UI/Animated.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:Button class=\"hover:scale-105 rotate-45 translate-x-4 origin-top-left transition duration-150 delay-75 ease-out cursor-pointer\" /></ui:UXML>");

            var result = new GenerationService().Generate();

            var output = File.ReadAllText(scope.GetAssetPath("Generated", "TailwindUSS.generated.uss")).Replace("\r\n", "\n");

            Assert.That(result.GeneratedUtilityCount, Is.EqualTo(9));
            Assert.That(result.WarningCount, Is.EqualTo(0));
            Assert.That(result.ErrorCount, Is.EqualTo(0));
            Assert.That(output, Does.Contain(".cursor-pointer {\n    cursor: pointer;\n}"));
            Assert.That(output, Does.Contain(".delay-75 {\n    transition-delay: 75ms;\n}"));
            Assert.That(output, Does.Contain(".duration-150 {\n    transition-duration: 150ms;\n}"));
            Assert.That(output, Does.Contain(".ease-out {\n    transition-timing-function: ease-out;\n}"));
            Assert.That(output, Does.Contain(".hover\\:scale-105:hover {\n    scale: 1.05;\n}"));
            Assert.That(output, Does.Contain(".origin-top-left {\n    transform-origin: 0% 0%;\n}"));
            Assert.That(output, Does.Contain(".rotate-45 {\n    rotate: 45deg;\n}"));
            Assert.That(output, Does.Contain(".transition {\n    transition-property: all;\n}"));
            Assert.That(output, Does.Contain(".translate-x-4 {\n    translate: 16px 0;\n}"));
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
