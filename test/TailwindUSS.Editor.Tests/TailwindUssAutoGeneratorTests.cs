using System.IO;
using System.Reflection;

namespace TailwindUSS.Editor.Tests
{
    /// <summary>
    /// Represents tests for automatic USS generation on UXML save.
    /// </summary>
    public sealed class TailwindUssAutoGeneratorTests
    {
        /// <summary>
        /// Tests that auto generation is skipped when the setting is disabled.
        /// </summary>
        [Test]
        public void AutoGenerateOnUxmlSave_DoesNothingWhenDisabled()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, "{\"inputGlobs\":[\"Assets/UI/**/*.uxml\"],\"outputUssPath\":\"Assets/Generated/TailwindUSS.generated.uss\",\"autoGenerateOnUxmlSave\":false}");
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"flex\" /></ui:UXML>");

            InvokeOnPostprocessAllAssets(
                new[] { "Assets/UI/Main.uxml" },
                System.Array.Empty<string>(),
                System.Array.Empty<string>(),
                System.Array.Empty<string>());

            Assert.That(File.Exists(scope.GetAssetPath("Generated", "TailwindUSS.generated.uss")), Is.False);
            Assert.That(Debug.Entries, Is.Empty);
        }

        /// <summary>
        /// Tests that auto generation runs when the setting is enabled.
        /// </summary>
        [Test]
        public void AutoGenerateOnUxmlSave_GeneratesUssWhenEnabled()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, "{\"inputGlobs\":[\"Assets/UI/**/*.uxml\"],\"outputUssPath\":\"Assets/Generated/TailwindUSS.generated.uss\",\"autoGenerateOnUxmlSave\":true}");
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"flex\" /></ui:UXML>");

            InvokeOnPostprocessAllAssets(
                new[] { "Assets/UI/Main.uxml" },
                System.Array.Empty<string>(),
                System.Array.Empty<string>(),
                System.Array.Empty<string>());

            var output = File.ReadAllText(scope.GetAssetPath("Generated", "TailwindUSS.generated.uss")).Replace("\r\n", "\n");
            Assert.That(output, Does.Contain(".flex {\n    display: flex;\n}"));
            Assert.That(Debug.Entries[^1].Message, Does.Contain("TailwindUSS generation finished."));
        }

        private static void InvokeOnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            var method = typeof(TailwindUssAutoGenerator).GetMethod("OnPostprocessAllAssets", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(null, new object[] { importedAssets, deletedAssets, movedAssets, movedFromAssetPaths });
        }
    }
}
